using System;
using System.Collections.Generic;

namespace FunClicker.Systems
{
    [Serializable]
    public class PlayerProgressData
    {
        public long currentPoints;
        public long baseScorePerClick = 1;
        public long scorePerSecond;
        public List<UpgradeProgressData> upgrades = new();
    }

    [Serializable]
    public class UpgradeProgressData
    {
        public string upgradeId;
        public int purchasedCount;

        public UpgradeProgressData(string upgradeId, int purchasedCount)
        {
            this.upgradeId = upgradeId;
            this.purchasedCount = purchasedCount;
        }
    }
}
