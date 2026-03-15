using System;
using UnityEngine;

namespace FunClicker.Upgrades
{
    [Serializable]
    public class UpgradeRuntimeData
    {
        public UpgradeSO definition;
        public int purchasedCount;

        public UpgradeRuntimeData(UpgradeSO definition)
        {
            this.definition = definition;
            purchasedCount = 0;
        }
    }
}