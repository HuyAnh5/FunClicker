using FunClicker.Core;
using FunClicker.Data;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class SkinVisualController : MonoBehaviour
    {
        [SerializeField] private SkinSO currentSkin;
        [SerializeField] private Image characterImage;
        [SerializeField] private Image backgroundImage;

        private void Start()
        {
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.OnMultiplierChanged += ApplyByMultiplier;
                ApplyByMultiplier(ComboManager.Instance.CurrentMultiplier);
            }
            else
            {
                ApplyByMultiplier(1);
            }
        }

        private void OnDestroy()
        {
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.OnMultiplierChanged -= ApplyByMultiplier;
            }
        }

        public void SetSkin(SkinSO newSkin)
        {
            currentSkin = newSkin;

            int multiplier = 1;
            if (ComboManager.Instance != null)
                multiplier = ComboManager.Instance.CurrentMultiplier;

            ApplyByMultiplier(multiplier);
        }

        private void ApplyByMultiplier(int multiplier)
        {
            if (currentSkin == null)
                return;

            if (characterImage != null)
                characterImage.sprite = currentSkin.GetCharacterSprite(multiplier);

            if (backgroundImage != null)
                backgroundImage.sprite = currentSkin.GetBackgroundSprite(multiplier);
        }
    }
}