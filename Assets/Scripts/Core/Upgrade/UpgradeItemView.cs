using FunClicker.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button buyButton;

        private UpgradeSO currentUpgrade;

        public void Setup(UpgradeSO upgrade, int purchasedCount)
        {
            currentUpgrade = upgrade;

            if (iconImage != null)
                iconImage.sprite = upgrade.icon;

            if (nameText != null)
                nameText.text = upgrade.displayName;

            if (descText != null)
                descText.text = upgrade.Description;

            if (costText != null)
                costText.text = upgrade.cost.ToString();

            if (countText != null)
                countText.text = purchasedCount.ToString();

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnClickBuy);
            }
        }

        private void OnClickBuy()
        {
            if (currentUpgrade == null || UpgradeManager.Instance == null)
                return;

            bool success = UpgradeManager.Instance.TryBuy(currentUpgrade);
            if (!success) return;

            int newCount = UpgradeManager.Instance.GetPurchasedCount(currentUpgrade);

            if (countText != null)
                countText.text = newCount.ToString();
        }
    }
}