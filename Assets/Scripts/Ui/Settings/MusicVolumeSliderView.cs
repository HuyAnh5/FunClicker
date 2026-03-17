using FunClicker.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class MusicVolumeSliderView : MonoBehaviour
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private bool wholePercent = true;

        private void Awake()
        {
            if (volumeSlider == null)
                volumeSlider = GetComponent<Slider>();

            if (volumeSlider == null)
                return;

            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
        }

        private void Start()
        {
            if (volumeSlider == null)
                return;

            if (BackgroundMusicController.Instance != null)
                volumeSlider.SetValueWithoutNotify(BackgroundMusicController.Instance.CurrentVolume);

            RefreshValueText(volumeSlider.value);

            volumeSlider.onValueChanged.AddListener(HandleSliderChanged);
        }

        private void OnDestroy()
        {
            if (volumeSlider != null)
                volumeSlider.onValueChanged.RemoveListener(HandleSliderChanged);
        }

        private void HandleSliderChanged(float value)
        {
            if (BackgroundMusicController.Instance != null)
                BackgroundMusicController.Instance.SetVolume(value);

            RefreshValueText(value);
        }

        private void RefreshValueText(float value)
        {
            if (valueText == null)
                return;

            float percent = value * 100f;
            valueText.text = wholePercent
                ? $"{Mathf.RoundToInt(percent)}%"
                : $"{percent:0.#}%";
        }
    }
}
