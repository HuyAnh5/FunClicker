using System;
using DG.Tweening;
using FunClicker.Core;
using FunClicker.UI;
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

        private Tween currentTween;
        private Vector3 originalScale;

        private void Awake()
        {
            if (clickVisual != null)
                originalScale = clickVisual.localScale;
        }

        private void Start()
        {
            InitializeBaseScorePerClick();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            InitializeBaseScorePerClick();
        }

        public void OnPointerDown(PointerEventData eventData)
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
                    eventData.position,
                    eventData.pressEventCamera,
                    $"+{finalPoints}"
                );
            }
        }

        public void SetBasePointsPerClick(long newValue)
        {
            basePointsPerClick = Math.Max(1L, newValue);
            InitializeBaseScorePerClick();
        }

        private void InitializeBaseScorePerClick()
        {
            if (PointManager.Instance == null) return;
            PointManager.Instance.SetBaseScorePerClick(Math.Max(1L, basePointsPerClick));
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
