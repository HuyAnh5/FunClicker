using System;
using System.Collections.Generic;
using FunClicker.Core;
using UnityEngine;

namespace FunClicker.Upgrades
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Upgrade Definitions")]
        [SerializeField] private List<UpgradeSO> upgrades = new();

        [Header("Runtime")]
        [SerializeField] private List<UpgradeRuntimeData> runtimeData = new();

        public IReadOnlyList<UpgradeSO> Upgrades => upgrades;
        public IReadOnlyList<UpgradeRuntimeData> RuntimeData => runtimeData;

        public event Action<UpgradeSO, int> OnUpgradePurchased;
        public event Action OnUpgradeDataChanged;

        private Dictionary<string, UpgradeRuntimeData> runtimeLookup = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildRuntimeData();
        }

        private void BuildRuntimeData()
        {
            runtimeLookup.Clear();

            foreach (var upgrade in upgrades)
            {
                if (upgrade == null || string.IsNullOrEmpty(upgrade.upgradeId))
                    continue;

                UpgradeRuntimeData existing = runtimeData.Find(x => x.definition == upgrade);
                if (existing == null)
                {
                    existing = new UpgradeRuntimeData(upgrade);
                    runtimeData.Add(existing);
                }

                runtimeLookup[upgrade.upgradeId] = existing;
            }
        }

        public int GetPurchasedCount(UpgradeSO upgrade)
        {
            if (upgrade == null || string.IsNullOrEmpty(upgrade.upgradeId))
                return 0;

            if (runtimeLookup.TryGetValue(upgrade.upgradeId, out var data))
                return data.purchasedCount;

            return 0;
        }

        public bool CanBuy(UpgradeSO upgrade)
        {
            if (upgrade == null || PointManager.Instance == null)
                return false;

            return PointManager.Instance.CurrentPoints >= upgrade.cost;
        }

        public bool TryBuy(UpgradeSO upgrade)
        {
            if (upgrade == null || PointManager.Instance == null)
                return false;

            if (PointManager.Instance.CurrentPoints < upgrade.cost)
                return false;

            bool spent = PointManager.Instance.TrySpendPoints(upgrade.cost);
            if (!spent)
                return false;

            ApplyUpgrade(upgrade);

            if (!runtimeLookup.TryGetValue(upgrade.upgradeId, out var data))
            {
                data = new UpgradeRuntimeData(upgrade);
                runtimeData.Add(data);
                runtimeLookup[upgrade.upgradeId] = data;
            }

            data.purchasedCount++;

            OnUpgradePurchased?.Invoke(upgrade, data.purchasedCount);
            OnUpgradeDataChanged?.Invoke();

            return true;
        }

        private void ApplyUpgrade(UpgradeSO upgrade)
        {
            switch (upgrade.upgradeType)
            {
                case UpgradeType.PointPerClick:
                    PointManager.Instance.SetBaseScorePerClick(
                        PointManager.Instance.BaseScorePerClick + upgrade.bonusAmount
                    );
                    break;

                case UpgradeType.PointPerSecond:
                    PointManager.Instance.SetScorePerSecond(
                        PointManager.Instance.ScorePerSecond + upgrade.bonusAmount
                    );
                    break;
            }
        }
    }
}