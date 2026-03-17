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

            if (isVisible && !ExclusivePanelCoordinator.TryOpen(panelRoot))
                return;

            panelRoot.SetActive(isVisible);

            if (!isVisible)
            {
                ExclusivePanelCoordinator.Close(panelRoot);
                return;
            }

            if (upgradeListView != null)
                upgradeListView.Rebuild();
        }
    }

    internal static class ExclusivePanelCoordinator
    {
        private static GameObject activePanelRoot;

        public static bool HasOpenPanel => activePanelRoot != null && activePanelRoot.activeSelf;

        public static bool TryOpen(GameObject panelRoot)
        {
            if (panelRoot == null)
                return false;

            if (activePanelRoot == null || activePanelRoot == panelRoot || !activePanelRoot.activeSelf)
            {
                activePanelRoot = panelRoot;
                return true;
            }

            return false;
        }

        public static void Close(GameObject panelRoot)
        {
            if (panelRoot == null)
                return;

            if (activePanelRoot == panelRoot)
                activePanelRoot = null;
        }
    }
}
