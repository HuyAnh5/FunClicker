using FunClicker.Core;
using TMPro;
using UnityEngine;

namespace FunClicker.UI
{
    public class PointStatsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statsText;

        private void Reset()
        {
            statsText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (statsText == null)
                statsText = GetComponent<TextMeshProUGUI>();

            if (PointManager.Instance == null)
            {
                Debug.LogError("Không tìm thấy PointManager trong scene.");
                return;
            }

            PointManager.Instance.OnPointStatsChanged += UpdateView;
            UpdateView(PointManager.Instance.BaseScorePerClick, PointManager.Instance.ScorePerSecond);
        }

        private void OnDestroy()
        {
            if (PointManager.Instance != null)
            {
                PointManager.Instance.OnPointStatsChanged -= UpdateView;
            }
        }

        private void UpdateView(long basePerClick, long perSecond)
        {
            if (statsText == null) return;

            statsText.text =
                $"SPC: {basePerClick}\n" +
                $"SPS: {perSecond}";
        }
    }
}