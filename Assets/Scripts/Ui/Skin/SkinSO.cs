using Sirenix.OdinInspector;
using UnityEngine;

namespace FunClicker.Data
{
    [CreateAssetMenu(fileName = "Skin_", menuName = "FunClicker/Skin Data", order = 0)]
    public class SkinSO : SerializedScriptableObject
    {
        [Title("Skin Info")]
        [HorizontalGroup("Info", Width = 90)]
        [PreviewField(80, ObjectFieldAlignment.Center), HideLabel]
        public Sprite icon;

        [VerticalGroup("Info/Right")]
        [LabelWidth(90)]
        public string skinId;

        [VerticalGroup("Info/Right")]
        [LabelWidth(90)]
        public string displayName;

        [Space(10)]
        [Title("Character Sprites")]
        [InlineProperty, HideLabel]
        public ComboSpriteSet character;

        [Space(10)]
        [Title("Background Sprites")]
        [InlineProperty, HideLabel]
        public ComboSpriteSet background;

        public Sprite GetCharacterSprite(int multiplier)
        {
            return character.GetByMultiplier(multiplier);
        }

        public Sprite GetBackgroundSprite(int multiplier)
        {
            return background.GetByMultiplier(multiplier);
        }
    }

    [System.Serializable]
    public class ComboSpriteSet
    {
        [BoxGroup("Combo States")]
        [HorizontalGroup("Combo States/Row")]
        [VerticalGroup("Combo States/Row/X1")]
        [PreviewField(100, ObjectFieldAlignment.Center), HideLabel]
        [LabelText("x1")]
        public Sprite comboX1;

        [VerticalGroup("Combo States/Row/X2")]
        [PreviewField(100, ObjectFieldAlignment.Center), HideLabel]
        [LabelText("x2")]
        public Sprite comboX2;

        [VerticalGroup("Combo States/Row/X4")]
        [PreviewField(100, ObjectFieldAlignment.Center), HideLabel]
        [LabelText("x4")]
        public Sprite comboX4;

        public Sprite GetByMultiplier(int multiplier)
        {
            switch (multiplier)
            {
                case 4: return comboX4 != null ? comboX4 : comboX2 != null ? comboX2 : comboX1;
                case 2: return comboX2 != null ? comboX2 : comboX1;
                default: return comboX1;
            }
        }
    }
}
