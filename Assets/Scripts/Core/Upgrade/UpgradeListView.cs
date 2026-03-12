using System.Collections.Generic;
using FunClicker.Upgrades;
using UnityEngine;

namespace FunClicker.UI
{
    public class UpgradeListView : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] private UpgradeItemView itemPrefab;

        private readonly List<UpgradeItemView> spawnedItems = new();

        private void Start()
        {
            Rebuild();

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnUpgradeDataChanged += RefreshAll;
            }
        }

        private void OnDestroy()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnUpgradeDataChanged -= RefreshAll;
            }
        }

        public void Rebuild()
        {
            ClearItems();

            if (UpgradeManager.Instance == null || contentRoot == null || itemPrefab == null)
                return;

            foreach (var upgrade in UpgradeManager.Instance.Upgrades)
            {
                if (upgrade == null) continue;

                var item = Instantiate(itemPrefab, contentRoot);
                int count = UpgradeManager.Instance.GetPurchasedCount(upgrade);
                item.Setup(upgrade, count);
                spawnedItems.Add(item);
            }
        }

        private void RefreshAll()
        {
            if (UpgradeManager.Instance == null) return;

            for (int i = 0; i < spawnedItems.Count; i++)
            {
                var upgrade = UpgradeManager.Instance.Upgrades[i];
                int count = UpgradeManager.Instance.GetPurchasedCount(upgrade);
                spawnedItems[i].Setup(upgrade, count);
            }
        }

        private void ClearItems()
        {
            for (int i = spawnedItems.Count - 1; i >= 0; i--)
            {
                if (spawnedItems[i] != null)
                    Destroy(spawnedItems[i].gameObject);
            }

            spawnedItems.Clear();
        }
    }
}