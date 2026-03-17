using System;
using DG.Tweening;
using FunClicker.Core;
using FunClicker.UI;
using FunClicker.UI.Effects;
using FunClicker.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FunClicker.InputSystem
{
    public class ClickAreaController : MonoBehaviour, IPointerDownHandler
    {
        [Header("Score")]
        [SerializeField] private long basePointsPerClick = 1;

        [Header("Visual Click Target")]
        [SerializeField] private Transform clickVisual;
        [SerializeField] private float punchScale = 0.12f;
        [SerializeField] private float animDuration = 0.12f;

        [Header("Floating Popup")]
        [SerializeField] private FloatingScorePopupSpawner popupSpawner;

        [Header("Burst Effect")]
        [SerializeField] private ClickParticleBurstSpawner burstEffectSpawner;

        private Tween currentTween;
        private Vector3 originalScale;

        private void Awake()
        {
            if (clickVisual != null)
                originalScale = clickVisual.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HandleClick(eventData.position, eventData.pressEventCamera);
        }

        public void HandleClick(Vector2 screenPosition, Camera eventCamera = null)
        {
            if (PointManager.Instance == null)
                return;

            int multiplier = 1;
            if (ComboManager.Instance != null)
            {
                multiplier = ComboManager.Instance.RegisterClickAndGetMultiplier();
            }

            long finalPoints = PointManager.Instance.BaseScorePerClick * multiplier;

            PointManager.Instance.AddPoints(finalPoints);
            PlayClickAnimation();

            if (popupSpawner != null)
            {
                popupSpawner.SpawnAtScreenPosition(
                    screenPosition,
                    eventCamera,
                    $"+{NumberFormatter.Format(finalPoints)}"
                );
            }

            if (burstEffectSpawner != null)
            {
                burstEffectSpawner.SpawnAtScreenPosition(screenPosition, eventCamera);
            }
        }

        public void SetBasePointsPerClick(long newValue)
        {
            basePointsPerClick = Math.Max(1L, newValue);
            if (PointManager.Instance != null)
                PointManager.Instance.SetBaseScorePerClick(basePointsPerClick);
        }

        private void PlayClickAnimation()
        {
            if (clickVisual == null) return;

            currentTween?.Kill();
            clickVisual.localScale = originalScale;

            currentTween = clickVisual
                .DOPunchScale(Vector3.one * punchScale, animDuration, 1, 0.5f)
                .SetUpdate(true);
        }
    }
}
