using System.Collections.Generic;
using FunClicker.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class SkinUnlockPopupController : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private bool hideOnStart = true;

        [Header("Preview")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button confirmButton;

        [Header("Labels")]
        [SerializeField] private string defaultTitle = "New Skin Unlocked!";
        [SerializeField] private string fallbackName = "Skin";

        private readonly Queue<SkinUnlockPopupData> pendingUnlocks = new();
        private SkinUnlockPopupData? currentPopup;
        private CanvasGroup panelCanvasGroup;

        private void Awake()
        {
            if (panelRoot != null)
            {
                panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
                if (panelCanvasGroup == null)
                    panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();
            }

            if (hideOnStart && panelRoot != null)
                SetPanelVisible(false);

            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmCurrentPopup);
        }

        private void Start()
        {
            if (SkinManager.Instance != null)
                SkinManager.Instance.OnSkinUnlocked += HandleSkinUnlocked;
        }

        private void OnDestroy()
        {
            if (panelRoot != null && panelRoot.activeSelf)
                ExclusivePanelCoordinator.Close(panelRoot);

            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(ConfirmCurrentPopup);

            if (SkinManager.Instance != null)
                SkinManager.Instance.OnSkinUnlocked -= HandleSkinUnlocked;
        }

        private void Update()
        {
            if (!currentPopup.HasValue && pendingUnlocks.Count > 0)
                TryDisplayNextPopup();
        }

        public void ConfirmCurrentPopup()
        {
            currentPopup = null;
            SetPanelVisible(false);
            TryDisplayNextPopup();
        }

        private void HandleSkinUnlocked(SkinSO skin, int index)
        {
            if (skin == null)
                return;

            SkinUnlockPopupData popupData = new SkinUnlockPopupData
            {
                skin = skin,
                level = index + 1
            };

            pendingUnlocks.Enqueue(popupData);
            TryDisplayNextPopup();
        }

        private void TryDisplayNextPopup()
        {
            if (currentPopup.HasValue || pendingUnlocks.Count == 0 || panelRoot == null)
                return;

            if (!ExclusivePanelCoordinator.TryOpen(panelRoot))
                return;

            currentPopup = pendingUnlocks.Dequeue();
            SetPanelVisible(true);
            ApplyPopup(currentPopup.Value);
        }

        private void ApplyPopup(SkinUnlockPopupData popupData)
        {
            if (previewImage != null)
            {
                previewImage.sprite = popupData.skin.GetPreviewSprite();
                previewImage.enabled = previewImage.sprite != null;
            }

            if (titleText != null)
                titleText.text = defaultTitle;

            if (levelText != null)
                levelText.text = $"Level {popupData.level}";

            if (nameText != null)
            {
                nameText.text = !string.IsNullOrWhiteSpace(popupData.skin.displayName)
                    ? popupData.skin.displayName
                    : fallbackName;
            }
        }

        private void SetPanelVisible(bool isVisible)
        {
            if (panelRoot == null)
                return;

            panelRoot.SetActive(isVisible);

            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = isVisible ? 1f : 0f;
                panelCanvasGroup.interactable = isVisible;
                panelCanvasGroup.blocksRaycasts = isVisible;
            }

            if (!isVisible)
                ExclusivePanelCoordinator.Close(panelRoot);
        }

        private struct SkinUnlockPopupData
        {
            public SkinSO skin;
            public int level;
        }
    }
}
