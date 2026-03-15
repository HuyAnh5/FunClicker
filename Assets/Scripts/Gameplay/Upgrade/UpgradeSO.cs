using Sirenix.OdinInspector;
using UnityEngine;

namespace FunClicker.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_", menuName = "FunClicker/Upgrade", order = 0)]
    public class UpgradeSO : SerializedScriptableObject
    {
        [Title("Info")]
        [HorizontalGroup("Top", Width = 90)]
        [PreviewField(80), HideLabel]
        public Sprite icon;

        [VerticalGroup("Top/Right")]
        [LabelWidth(100)]
        public string upgradeId;

        [VerticalGroup("Top/Right")]
        [LabelWidth(100)]
        public string displayName;

        [VerticalGroup("Top/Right")]
        [LabelWidth(100)]
        public UpgradeType upgradeType;

        [Title("Value")]
        [MinValue(1)]
        public long cost = 10;

        [MinValue(1)]
        public long bonusAmount = 1;

        [ShowInInspector, ReadOnly]
        public string Description
        {
            get
            {
                return upgradeType == UpgradeType.PointPerClick
                    ? $"+{bonusAmount} score/click"
                    : $"+{bonusAmount} score/second";
            }
        }
    }
}