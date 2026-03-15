using FunClicker.Core;
using TMPro;
using UnityEngine;

namespace FunClicker.UI
{
    public class PointTextView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointText;

        private void Reset()
        {
            pointText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (pointText == null)
                pointText = GetComponent<TextMeshProUGUI>();

            if (PointManager.Instance == null)
            {
                Debug.LogError("Không tìm thấy PointManager trong scene.");
                return;
            }

            PointManager.Instance.OnPointsChanged += UpdateView;
            UpdateView(PointManager.Instance.CurrentPoints);
        }

        private void OnDestroy()
        {
            if (PointManager.Instance != null)
            {
                PointManager.Instance.OnPointsChanged -= UpdateView;
            }
        }

        private void UpdateView(long points)
        {
            if (pointText == null) return;
            pointText.text = $"Score\n{points}";
        }
    }
}