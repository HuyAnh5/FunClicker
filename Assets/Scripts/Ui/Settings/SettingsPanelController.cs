using UnityEngine;

namespace FunClicker.UI
{
    public class SettingsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private bool hideOnStart = true;

        private void Awake()
        {
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
                ExclusivePanelCoordinator.Close(panelRoot);
        }
    }
}
