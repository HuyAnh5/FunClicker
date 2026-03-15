using DG.Tweening;
using FunClicker.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class ComboBarView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform fillArea;
        [SerializeField] private RectTransform fillRect;
        [SerializeField] private Image fillGraphic;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private Transform multiplierVisual;

        [Header("Colors")]
        [SerializeField] private Color x1Color = Color.green;
        [SerializeField] private Color x2Color = Color.yellow;
        [SerializeField] private Color x4Color = new Color(1f, 0.45f, 0.1f);

        [Header("Animation")]
        [SerializeField] private float fillTweenDuration = 0.08f;

        private Tween fillTween;
        private Tween pulseTween;

        private void Start()
        {
            if (ComboManager.Instance == null)
            {
                Debug.LogError("Không tìm thấy ComboManager trong scene.");
                return;
            }

            ComboManager.Instance.OnComboChanged += UpdateComboView;
            ComboManager.Instance.OnMultiplierChanged += PlayMultiplierPulse;

            UpdateComboView(
                ComboManager.Instance.ComboValue,
                ComboManager.Instance.CurrentMultiplier
            );
        }

        private void OnDestroy()
        {
            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.OnComboChanged -= UpdateComboView;
                ComboManager.Instance.OnMultiplierChanged -= PlayMultiplierPulse;
            }
        }

        private void UpdateComboView(float comboValue, int multiplier)
        {
            UpdateFillHeight(comboValue);

            if (fillGraphic != null)
            {
                fillGraphic.color = GetColor(multiplier);
            }

            if (multiplierText != null)
            {
                multiplierText.text = $"x{multiplier}";
                multiplierText.color = GetColor(multiplier);
            }
        }

        private void UpdateFillHeight(float normalizedValue)
        {
            if (fillArea == null || fillRect == null)
                return;

            float targetHeight = fillArea.rect.height * Mathf.Clamp01(normalizedValue);

            fillTween?.Kill();

            Vector2 currentSize = fillRect.sizeDelta;
            Vector2 targetSize = new Vector2(currentSize.x, targetHeight);

            fillTween = fillRect.DOSizeDelta(targetSize, fillTweenDuration).SetEase(Ease.OutQuad);
        }

        private void PlayMultiplierPulse(int multiplier)
        {
            if (multiplierVisual == null)
                multiplierVisual = multiplierText != null ? multiplierText.transform : null;

            if (multiplierVisual == null) return;

            pulseTween?.Kill();
            multiplierVisual.localScale = Vector3.one;

            pulseTween = multiplierVisual
                .DOPunchScale(Vector3.one * 0.2f, 0.18f, 1, 0.5f)
                .SetUpdate(true);
        }

        private Color GetColor(int multiplier)
        {
            switch (multiplier)
            {
                case 4: return x4Color;
                case 2: return x2Color;
                default: return x1Color;
            }
        }
    }
}