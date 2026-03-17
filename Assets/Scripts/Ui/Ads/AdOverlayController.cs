using FunClicker.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class AdOverlayController : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject adPanelRoot;
        [SerializeField] private bool hideOnStart = true;

        [Header("Buttons")]
        [SerializeField] private Button openAdButton;
        [SerializeField] private Button closeAdButton;

        private CanvasGroup adCanvasGroup;
        private bool isShowingAd;

        private void Awake()
        {
            if (adPanelRoot != null)
            {
                adCanvasGroup = adPanelRoot.GetComponent<CanvasGroup>();
                if (adCanvasGroup == null)
                    adCanvasGroup = adPanelRoot.AddComponent<CanvasGroup>();
            }

            if (openAdButton != null)
                openAdButton.onClick.AddListener(ShowAd);

            if (closeAdButton != null)
                closeAdButton.onClick.AddListener(CloseAd);

            if (hideOnStart)
                SetAdVisible(false);
        }

        private void OnDisable()
        {
            if (isShowingAd)
                ReleaseAdPause();
        }

        private void OnDestroy()
        {
            if (openAdButton != null)
                openAdButton.onClick.RemoveListener(ShowAd);

            if (closeAdButton != null)
                closeAdButton.onClick.RemoveListener(CloseAd);

            if (isShowingAd)
                ReleaseAdPause();
        }

        public void ShowAd()
        {
            if (adPanelRoot == null || isShowingAd)
                return;

            if (!ExclusivePanelCoordinator.TryOpen(adPanelRoot))
                return;

            isShowingAd = true;
            SetAdVisible(true);

            if (GamePauseManager.Instance != null)
                GamePauseManager.Instance.PauseGame();
        }

        public void CloseAd()
        {
            if (!isShowingAd)
                return;

            ReleaseAdPause();
        }

        private void ReleaseAdPause()
        {
            isShowingAd = false;
            SetAdVisible(false);
            ExclusivePanelCoordinator.Close(adPanelRoot);

            if (GamePauseManager.Instance != null)
                GamePauseManager.Instance.ResumeGame();
        }

        private void SetAdVisible(bool visible)
        {
            if (adPanelRoot == null)
                return;

            adPanelRoot.SetActive(visible);

            if (adCanvasGroup == null)
                return;

            adCanvasGroup.alpha = visible ? 1f : 0f;
            adCanvasGroup.interactable = visible;
            adCanvasGroup.blocksRaycasts = visible;
        }
    }
}
