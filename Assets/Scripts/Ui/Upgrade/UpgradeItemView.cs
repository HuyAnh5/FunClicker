using FunClicker.Upgrades;
using FunClicker.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private Button itemButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI countText;

        private UpgradeSO currentUpgrade;

        private void Reset()
        {
            itemButton = GetComponent<Button>();
        }

        public void Setup(UpgradeSO upgrade, int purchasedCount)
        {
            currentUpgrade = upgrade;

            if (iconImage != null)
                iconImage.sprite = upgrade.icon;

            if (nameText != null)
                nameText.text = upgrade.displayName;

            if (typeText != null)
                typeText.text = upgrade.upgradeType == UpgradeType.PointPerClick ? "PPC" : "PPS";

            if (descText != null)
                descText.text = upgrade.Description;

            if (costText != null)
                costText.text = $"Cost: {upgrade.cost}";

            if (countText != null)
                countText.text = $"Lv. {purchasedCount}";

            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(OnClickBuy);
            }

            RefreshButtonState();
        }

        private void OnEnable()
        {
            if (PointManager.Instance != null)
                PointManager.Instance.OnPointsChanged += HandlePointsChanged;

            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.OnUpgradeDataChanged += RefreshFromManager;
        }

        private void OnDisable()
        {
            if (PointManager.Instance != null)
                PointManager.Instance.OnPointsChanged -= HandlePointsChanged;

            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.OnUpgradeDataChanged -= RefreshFromManager;
        }

        private void OnClickBuy()
        {
            if (currentUpgrade == null || UpgradeManager.Instance == null)
                return;

            bool success = UpgradeManager.Instance.TryBuy(currentUpgrade);
            if (!success) return;

            int newCount = UpgradeManager.Instance.GetPurchasedCount(currentUpgrade);

            if (countText != null)
                countText.text = $"Lv. {newCount}";

            RefreshButtonState();
        }

        private void HandlePointsChanged(long _)
        {
            RefreshButtonState();
        }

        private void RefreshFromManager()
        {
            if (currentUpgrade == null || UpgradeManager.Instance == null)
                return;

            int purchasedCount = UpgradeManager.Instance.GetPurchasedCount(currentUpgrade);

            if (countText != null)
                countText.text = $"Lv. {purchasedCount}";

            RefreshButtonState();
        }

        private void RefreshButtonState()
        {
            if (itemButton == null || currentUpgrade == null || UpgradeManager.Instance == null)
                return;

            itemButton.interactable = UpgradeManager.Instance.CanBuy(currentUpgrade);
        }
    }
}
