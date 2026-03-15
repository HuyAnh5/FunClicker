using UnityEngine;

namespace FunClicker.UI
{
    public class UpgradePanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private UpgradeListView upgradeListView;
        [SerializeField] private bool hideOnStart = true;

        private void Awake()
        {
            if (upgradeListView == null && panelRoot != null)
                upgradeListView = panelRoot.GetComponentInChildren<UpgradeListView>(true);

            if (hideOnStart && panelRoot != null)
                panelRoot.SetActive(false);
        }

        public void OpenPanel()
        {
            SetPanelVisible(true);
        }

        public void ClosePanel()
        {
            SetPanelVisible(false);
        }

        public void TogglePanel()
        {
            if (panelRoot == null)
                return;

            SetPanelVisible(!panelRoot.activeSelf);
        }

        private void SetPanelVisible(bool isVisible)
        {
            if (panelRoot == null)
                return;

            panelRoot.SetActive(isVisible);

            if (isVisible && upgradeListView != null)
                upgradeListView.Rebuild();
        }
    }
}
