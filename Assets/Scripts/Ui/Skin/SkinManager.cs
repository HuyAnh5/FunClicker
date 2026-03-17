using System;
using System.Collections.Generic;
using FunClicker.Core;
using FunClicker.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FunClicker.UI
{
    public class SkinManager : MonoBehaviour
    {
        private const string SelectedSkinPrefKey = "skins.selected_id";
        private const string HighestPointsPrefKey = "skins.highest_points";

        public static SkinManager Instance { get; private set; }

        [Title("Skin Unlock Order")]
        [TableList(AlwaysExpanded = true)]
        [SerializeField] private List<SkinUnlockEntry> skinEntries = new();

        [Title("Visual")]
        [SerializeField] private SkinVisualController skinVisualController;

        public IReadOnlyList<SkinUnlockEntry> SkinEntries => skinEntries;
        public SkinSO SelectedSkin { get; private set; }
        public long HighestPointsReached { get; private set; }

        public event Action<SkinSO> OnSelectedSkinChanged;
        public event Action OnSkinDataChanged;
        public event Action<SkinSO, int> OnSkinUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (skinVisualController == null)
                skinVisualController = FindFirstObjectByType<SkinVisualController>();

            HighestPointsReached = LoadHighestPoints();
            ResolveSelectedSkin();
            EnsureSelectedSkinIsUnlocked();
            ApplySelectedSkin();
        }

        private void OnValidate()
        {
            for (int i = 0; i < skinEntries.Count; i++)
            {
                if (skinEntries[i] != null)
                    skinEntries[i].Level = i + 1;
            }
        }

        private void Start()
        {
            if (PointManager.Instance != null)
            {
                PointManager.Instance.OnPointsChanged += HandlePointsChanged;
                HandlePointsChanged(PointManager.Instance.CurrentPoints);
            }

            OnSkinDataChanged?.Invoke();
        }

        private void OnDestroy()
        {
            if (PointManager.Instance != null)
                PointManager.Instance.OnPointsChanged -= HandlePointsChanged;
        }

        public bool SelectSkin(SkinSO skin)
        {
            int index = GetSkinIndex(skin);
            if (index < 0 || !IsSkinUnlocked(index))
                return false;

            if (SelectedSkin == skin)
                return true;

            SelectedSkin = skin;
            ApplySelectedSkin();
            SaveSelectedSkin(SelectedSkin);

            OnSelectedSkinChanged?.Invoke(SelectedSkin);
            OnSkinDataChanged?.Invoke();
            return true;
        }

        public int GetSkinCount()
        {
            return skinEntries.Count;
        }

        public SkinUnlockEntry GetEntry(int index)
        {
            if (index < 0 || index >= skinEntries.Count)
                return null;

            return skinEntries[index];
        }

        public int GetSkinIndex(SkinSO skin)
        {
            if (skin == null)
                return -1;

            for (int i = 0; i < skinEntries.Count; i++)
            {
                if (skinEntries[i] != null && skinEntries[i].skin == skin)
                    return i;
            }

            return -1;
        }

        public int GetSkinLevel(int index)
        {
            return index + 1;
        }

        public bool IsSkinUnlocked(int index)
        {
            SkinUnlockEntry entry = GetEntry(index);
            return entry != null && HighestPointsReached >= entry.unlockAtPoints;
        }

        public bool IsSkinUnlocked(SkinSO skin)
        {
            int index = GetSkinIndex(skin);
            return IsSkinUnlocked(index);
        }

        public string GetUnlockText(int index)
        {
            SkinUnlockEntry entry = GetEntry(index);
            if (entry == null)
                return string.Empty;

            return entry.GetUnlockText();
        }

        [Button(ButtonSizes.Medium)]
        public void ResetSkinUnlockProgress()
        {
            HighestPointsReached = 0;
            PlayerPrefs.DeleteKey(HighestPointsPrefKey);
            PlayerPrefs.DeleteKey(SelectedSkinPrefKey);
            PlayerPrefs.Save();

            SelectedSkin = FindFirstUnlockedSkin();
            ApplySelectedSkin();

            OnSelectedSkinChanged?.Invoke(SelectedSkin);
            OnSkinDataChanged?.Invoke();
        }

        private void HandlePointsChanged(long currentPoints)
        {
            long previousHighest = HighestPointsReached;
            if (currentPoints <= HighestPointsReached)
                return;

            HighestPointsReached = currentPoints;
            SaveHighestPoints();
            OnSkinDataChanged?.Invoke();

            for (int i = 0; i < skinEntries.Count; i++)
            {
                SkinUnlockEntry entry = skinEntries[i];
                if (entry == null || entry.skin == null)
                    continue;

                bool wasLocked = entry.unlockAtPoints > previousHighest;
                bool nowUnlocked = entry.unlockAtPoints <= HighestPointsReached;
                if (wasLocked && nowUnlocked)
                    OnSkinUnlocked?.Invoke(entry.skin, i);
            }
        }

        private void ResolveSelectedSkin()
        {
            if (skinEntries.Count == 0)
            {
                SelectedSkin = null;
                return;
            }

            string savedSkinId = PlayerPrefs.GetString(SelectedSkinPrefKey, string.Empty);
            SelectedSkin = FindSkinById(savedSkinId);

            if (SelectedSkin == null)
                SelectedSkin = FindFirstValidSkin();
        }

        private void EnsureSelectedSkinIsUnlocked()
        {
            if (SelectedSkin != null && IsSkinUnlocked(SelectedSkin))
                return;

            SelectedSkin = FindFirstUnlockedSkin();

            if (SelectedSkin != null)
                SaveSelectedSkin(SelectedSkin);
        }

        private SkinSO FindSkinById(string skinId)
        {
            if (string.IsNullOrWhiteSpace(skinId))
                return null;

            foreach (var entry in skinEntries)
            {
                SkinSO skin = entry?.skin;
                if (skin == null)
                    continue;

                string candidateId = string.IsNullOrWhiteSpace(skin.skinId) ? skin.name : skin.skinId;
                if (string.Equals(candidateId, skinId, StringComparison.Ordinal))
                    return skin;
            }

            return null;
        }

        private SkinSO FindFirstValidSkin()
        {
            foreach (var entry in skinEntries)
            {
                if (entry != null && entry.skin != null)
                    return entry.skin;
            }

            return null;
        }

        private SkinSO FindFirstUnlockedSkin()
        {
            for (int i = 0; i < skinEntries.Count; i++)
            {
                if (IsSkinUnlocked(i))
                    return skinEntries[i].skin;
            }

            return FindFirstValidSkin();
        }

        private void SaveSelectedSkin(SkinSO skin)
        {
            if (skin == null)
                return;

            string skinId = string.IsNullOrWhiteSpace(skin.skinId) ? skin.name : skin.skinId;
            PlayerPrefs.SetString(SelectedSkinPrefKey, skinId);
            PlayerPrefs.Save();
        }

        private void SaveHighestPoints()
        {
            PlayerPrefs.SetString(HighestPointsPrefKey, HighestPointsReached.ToString());
            PlayerPrefs.Save();
        }

        private long LoadHighestPoints()
        {
            string savedValue = PlayerPrefs.GetString(HighestPointsPrefKey, "0");
            if (long.TryParse(savedValue, out long parsedValue))
                return Math.Max(0L, parsedValue);

            return 0L;
        }

        private void ApplySelectedSkin()
        {
            if (SelectedSkin == null || skinVisualController == null)
                return;

            skinVisualController.SetSkin(SelectedSkin);
        }
    }

    [Serializable]
    public class SkinUnlockEntry
    {
        [TableColumnWidth(60, Resizable = false)]
        [ShowInInspector, ReadOnly]
        public string LevelLabel => $"Lv {Level}";

        [HideInInspector]
        public int Level;

        [AssetsOnly]
        [TableColumnWidth(180)]
        public SkinSO skin;

        [MinValue(0)]
        [LabelText("Unlock Points")]
        public long unlockAtPoints;

        public string GetUnlockText()
        {
            return unlockAtPoints <= 0
                ? "Unlocked by default"
                : $"Unlock at {FunClicker.Utils.NumberFormatter.Format(unlockAtPoints)} score";
        }
    }
}
