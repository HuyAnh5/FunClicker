using DG.Tweening;
using TMPro;
using UnityEngine;
using DarkTonic.PoolBoss;

namespace FunClicker.UI
{
    public class FloatingScorePopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI valueText;

        [Header("Animation")]
        [SerializeField] private float riseDistance = 120f;
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private float startScale = 0.9f;
        [SerializeField] private float peakScale = 1.05f;
        [SerializeField] private float scaleUpTime = 0.08f;

        private Sequence sequence;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = transform as RectTransform;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (valueText == null)
                valueText = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        public void Play(string textValue, Vector2 anchoredPos)
        {
            if (rectTransform == null || canvasGroup == null || valueText == null)
            {
                Debug.LogError($"FloatingScorePopup thiếu reference trên {name}");
                return;
            }

            sequence?.Kill();

            // ép lại cho chắc chắn
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            valueText.text = textValue;
            rectTransform.anchoredPosition = anchoredPos;
            rectTransform.localScale = Vector3.one * startScale;
            canvasGroup.alpha = 1f;

            Vector2 targetPos = new Vector2(anchoredPos.x, anchoredPos.y + riseDistance);

            sequence = DOTween.Sequence();

            // scale nhẹ tại chỗ
            sequence.Join(rectTransform.DOScale(peakScale, scaleUpTime).SetEase(Ease.OutQuad));

            // bay thẳng lên hướng 12h
            sequence.Join(rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad));

            // mờ dần
            sequence.Join(canvasGroup.DOFade(0f, duration).SetEase(Ease.Linear));

            sequence.OnComplete(() =>
            {
                transform.Despawn();
            });
        }

        private void OnDisable()
        {
            sequence?.Kill();
        }
    }
}