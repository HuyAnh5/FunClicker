using FunClicker.Audio;
using FunClicker.Core;
using FunClicker.UI;
using FunClicker.Upgrades;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FunClicker.Systems
{
    public class PlayerProgressManager : MonoBehaviour
    {
        private const string SaveKey = "player.progress";
        private const string SkinSelectedKey = "skins.selected_id";
        private const string SkinHighestPointsKey = "skins.highest_points";
        private const string MusicVolumeKey = "settings.music_volume";

        [Header("References")]
        [SerializeField] private PointManager pointManager;
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private ComboManager comboManager;
        [SerializeField] private SkinManager skinManager;
        [SerializeField] private BackgroundMusicController backgroundMusicController;

        [Header("Save")]
        [SerializeField, Min(0.1f)] private float autoSaveInterval = 1f;

        private bool isDirty;
        private bool isApplyingData;
        private float autoSaveTimer;

        private void Awake()
        {
            if (pointManager == null)
                pointManager = FindFirstObjectByType<PointManager>();

            if (upgradeManager == null)
                upgradeManager = FindFirstObjectByType<UpgradeManager>();

            if (comboManager == null)
                comboManager = FindFirstObjectByType<ComboManager>();

            if (skinManager == null)
                skinManager = FindFirstObjectByType<SkinManager>();

            if (backgroundMusicController == null)
                backgroundMusicController = FindFirstObjectByType<BackgroundMusicController>();
        }

        private void Start()
        {
            LoadProgress();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
            SaveIfDirty();
        }

        private void Update()
        {
            if (!isDirty)
                return;

            autoSaveTimer += Time.unscaledDeltaTime;
            if (autoSaveTimer >= autoSaveInterval)
                SaveProgress();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveIfDirty();
        }

        private void OnApplicationQuit()
        {
            SaveIfDirty();
        }

        private void Subscribe()
        {
            if (pointManager != null)
            {
                pointManager.OnPointsChanged += HandleProgressChanged;
                pointManager.OnPointStatsChanged += HandlePointStatsChanged;
            }

            if (upgradeManager != null)
                upgradeManager.OnUpgradeDataChanged += HandleUpgradeDataChanged;
        }

        private void Unsubscribe()
        {
            if (pointManager != null)
            {
                pointManager.OnPointsChanged -= HandleProgressChanged;
                pointManager.OnPointStatsChanged -= HandlePointStatsChanged;
            }

            if (upgradeManager != null)
                upgradeManager.OnUpgradeDataChanged -= HandleUpgradeDataChanged;
        }

        private void HandleProgressChanged(long _)
        {
            MarkDirty();
        }

        private void HandlePointStatsChanged(long _, long __)
        {
            MarkDirty();
        }

        private void HandleUpgradeDataChanged()
        {
            MarkDirty();
        }

        private void MarkDirty()
        {
            if (isApplyingData)
                return;

            isDirty = true;
        }

        private void LoadProgress()
        {
            if (pointManager == null || upgradeManager == null)
                return;

            if (!PlayerPrefs.HasKey(SaveKey))
                return;

            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
                return;

            PlayerProgressData saveData = JsonUtility.FromJson<PlayerProgressData>(json);
            if (saveData == null)
                return;

            isApplyingData = true;

            pointManager.SetPoints(saveData.currentPoints);
            pointManager.SetBaseScorePerClick(saveData.baseScorePerClick);
            pointManager.SetScorePerSecond(saveData.scorePerSecond);
            upgradeManager.LoadProgress(saveData.upgrades);

            isApplyingData = false;
            isDirty = false;
            autoSaveTimer = 0f;
        }

        private void SaveIfDirty()
        {
            if (isDirty)
                SaveProgress();
        }

        private void SaveProgress()
        {
            if (pointManager == null || upgradeManager == null)
                return;

            PlayerProgressData saveData = new PlayerProgressData
            {
                currentPoints = pointManager.CurrentPoints,
                baseScorePerClick = pointManager.BaseScorePerClick,
                scorePerSecond = pointManager.ScorePerSecond,
                upgrades = upgradeManager.CaptureProgress()
            };

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();

            isDirty = false;
            autoSaveTimer = 0f;
        }

        [Button(ButtonSizes.Medium)]
        public void DebugSaveNow()
        {
            SaveProgress();
            Debug.Log($"PlayerProgressManager saved key '{SaveKey}'.");
        }

        [Button(ButtonSizes.Medium)]
        public void DebugLoadNow()
        {
            LoadProgress();
            Debug.Log($"PlayerProgressManager loaded key '{SaveKey}': {PlayerPrefs.HasKey(SaveKey)}");
        }

        [Button(ButtonSizes.Medium)]
        public void DebugClearAllSave()
        {
            isApplyingData = true;

            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.DeleteKey(SkinSelectedKey);
            PlayerPrefs.DeleteKey(SkinHighestPointsKey);
            PlayerPrefs.DeleteKey(MusicVolumeKey);
            PlayerPrefs.Save();

            if (pointManager != null)
            {
                pointManager.SetPoints(0);
                pointManager.SetBaseScorePerClick(1);
                pointManager.SetScorePerSecond(0);
            }

            if (upgradeManager != null)
                upgradeManager.LoadProgress(null);

            if (comboManager != null)
                comboManager.ResetCombo();

            if (skinManager != null)
                skinManager.ResetSkinUnlockProgress();

            if (backgroundMusicController != null)
                backgroundMusicController.ClearSavedVolume();

            isApplyingData = false;
            isDirty = false;
            autoSaveTimer = 0f;

            Debug.Log("PlayerProgressManager cleared player progress, skin progress, and music volume save.");
        }
    }
}
