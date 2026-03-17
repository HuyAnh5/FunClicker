using FunClicker.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class SkinSelectionPanelController : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private bool hideOnStart = true;

        [Header("Preview")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI requirementText;
        [SerializeField] private TextMeshProUGUI buttonLabel;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        [Header("Labels")]
        [SerializeField] private string selectedLabel = "Selected";
        [SerializeField] private string selectLabel = "Select";
        [SerializeField] private string lockedLabel = "Locked";
        [SerializeField] private string fallbackSkinName = "Skin";

        private int previewIndex;

        private void Awake()
        {
            if (hideOnStart && panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void Start()
        {
            SyncPreviewIndexToSelectedSkin();
            RefreshView();

            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.OnSelectedSkinChanged += HandleSkinDataChanged;
                SkinManager.Instance.OnSkinDataChanged += HandleSkinDataChanged;
            }
        }

        private void OnDestroy()
        {
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.OnSelectedSkinChanged -= HandleSkinDataChanged;
                SkinManager.Instance.OnSkinDataChanged -= HandleSkinDataChanged;
            }
        }

        public void OpenPanel()
        {
            SyncPreviewIndexToSelectedSkin();
            SetPanelVisible(true);
            RefreshView();
        }

        public void ClosePanel()
        {
            SetPanelVisible(false);
        }

        public void TogglePanel()
        {
            if (panelRoot == null)
                return;

            bool isVisible = !panelRoot.activeSelf;
            if (isVisible)
                SyncPreviewIndexToSelectedSkin();

            SetPanelVisible(isVisible);

            if (isVisible)
                RefreshView();
        }

        public void ShowPreviousSkin()
        {
            int skinCount = GetSkinCount();
            if (skinCount == 0)
                return;

            previewIndex = (previewIndex - 1 + skinCount) % skinCount;
            RefreshView();
        }

        public void ShowNextSkin()
        {
            int skinCount = GetSkinCount();
            if (skinCount == 0)
                return;

            previewIndex = (previewIndex + 1) % skinCount;
            RefreshView();
        }

        public void SelectPreviewSkin()
        {
            SkinSO previewSkin = GetPreviewSkin();
            if (previewSkin == null || SkinManager.Instance == null)
                return;

            SkinManager.Instance.SelectSkin(previewSkin);
            RefreshView();
        }

        private void HandleSkinDataChanged(SkinSO _)
        {
            RefreshView();
        }

        private void HandleSkinDataChanged()
        {
            RefreshView();
        }

        private void SetPanelVisible(bool isVisible)
        {
            if (panelRoot == null)
                return;

            if (isVisible && !ExclusivePanelCoordinator.TryOpen(panelRoot))
                return;

            panelRoot.SetActive(isVisible);

            if (!isVisible)
                ExclusivePanelCoordinator.Close(panelRoot);
        }

        private void SyncPreviewIndexToSelectedSkin()
        {
            if (SkinManager.Instance == null || SkinManager.Instance.GetSkinCount() == 0)
            {
                previewIndex = 0;
                return;
            }

            int selectedIndex = SkinManager.Instance.GetSkinIndex(SkinManager.Instance.SelectedSkin);
            previewIndex = selectedIndex >= 0 ? selectedIndex : 0;
        }

        private void RefreshView()
        {
            int skinCount = GetSkinCount();
            SkinSO previewSkin = GetPreviewSkin();
            bool hasSkin = previewSkin != null;
            bool isUnlocked = hasSkin && SkinManager.Instance != null && SkinManager.Instance.IsSkinUnlocked(previewIndex);
            bool isSelected = hasSkin && SkinManager.Instance != null && SkinManager.Instance.SelectedSkin == previewSkin;

            if (previewImage != null)
            {
                previewImage.sprite = hasSkin ? previewSkin.GetPreviewSprite() : null;
                previewImage.enabled = previewImage.sprite != null;
            }

            if (nameText != null)
            {
                nameText.text = hasSkin
                    ? $"Level {previewIndex + 1}"
                    : fallbackSkinName;
            }

            if (requirementText != null)
            {
                if (!hasSkin || SkinManager.Instance == null)
                {
                    requirementText.text = string.Empty;
                }
                else if (isUnlocked)
                {
                    requirementText.text = !string.IsNullOrWhiteSpace(previewSkin.displayName)
                        ? previewSkin.displayName
                        : "Unlocked";
                }
                else
                {
                    requirementText.text = SkinManager.Instance.GetUnlockText(previewIndex);
                }
            }

            if (buttonLabel != null)
            {
                if (!hasSkin)
                    buttonLabel.text = lockedLabel;
                else if (!isUnlocked)
                    buttonLabel.text = lockedLabel;
                else if (isSelected)
                    buttonLabel.text = selectedLabel;
                else
                    buttonLabel.text = selectLabel;
            }

            if (selectButton != null)
                selectButton.interactable = hasSkin && isUnlocked && !isSelected;

            bool canNavigate = skinCount > 1;

            if (previousButton != null)
                previousButton.interactable = canNavigate;

            if (nextButton != null)
                nextButton.interactable = canNavigate;
        }

        private int GetSkinCount()
        {
            return SkinManager.Instance != null ? SkinManager.Instance.GetSkinCount() : 0;
        }

        private SkinSO GetPreviewSkin()
        {
            int skinCount = GetSkinCount();
            if (skinCount == 0 || SkinManager.Instance == null)
                return null;

            previewIndex = Mathf.Clamp(previewIndex, 0, skinCount - 1);
            SkinUnlockEntry entry = SkinManager.Instance.GetEntry(previewIndex);
            return entry != null ? entry.skin : null;
        }
    }
}
