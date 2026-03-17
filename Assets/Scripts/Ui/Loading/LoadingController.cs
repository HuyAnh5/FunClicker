using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FunClicker.UI
{
    public class LoadingController : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string targetSceneName = "SampleScene";
        [SerializeField] private float minimumLoadingTime = 1f;
        [SerializeField] private bool autoActivateWhenReady = true;
        [SerializeField] private bool waitForInputWhenReady = false;

        [Header("UI")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI continueText;

        [Header("Labels")]
        [SerializeField] private string loadingLabel = "Loading...";
        [SerializeField] private string readyLabel = "Tap to continue";

        private AsyncOperation loadingOperation;
        private bool isReadyToActivate;

        private void Start()
        {
            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
                progressSlider.value = 0f;
            }

            if (continueText != null)
                continueText.gameObject.SetActive(false);

            StartCoroutine(LoadSceneRoutine());
        }

        private void Update()
        {
            if (!isReadyToActivate || !waitForInputWhenReady)
                return;

            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.anyKeyDown)
                ActivateLoadedScene();
        }

        public void ActivateLoadedScene()
        {
            if (!isReadyToActivate || loadingOperation == null)
                return;

            loadingOperation.allowSceneActivation = true;
            isReadyToActivate = false;
        }

        private IEnumerator LoadSceneRoutine()
        {
            if (string.IsNullOrWhiteSpace(targetSceneName))
            {
                Debug.LogError("LoadingController thiếu targetSceneName.");
                yield break;
            }

            if (statusText != null)
                statusText.text = loadingLabel;

            loadingOperation = SceneManager.LoadSceneAsync(targetSceneName);
            if (loadingOperation == null)
            {
                Debug.LogError($"Không thể load scene '{targetSceneName}'.");
                yield break;
            }

            loadingOperation.allowSceneActivation = false;

            float elapsed = 0f;

            while (!loadingOperation.isDone)
            {
                elapsed += Time.unscaledDeltaTime;

                float rawProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
                float timeProgress = minimumLoadingTime > 0f
                    ? Mathf.Clamp01(elapsed / minimumLoadingTime)
                    : 1f;
                float displayProgress = Mathf.Min(rawProgress, timeProgress);

                UpdateProgressUI(displayProgress);

                bool finishedLoading = rawProgress >= 1f;
                bool finishedMinTime = elapsed >= minimumLoadingTime;

                if (!isReadyToActivate && finishedLoading && finishedMinTime)
                {
                    isReadyToActivate = true;
                    UpdateProgressUI(1f);

                    if (statusText != null)
                        statusText.text = waitForInputWhenReady ? readyLabel : loadingLabel;

                    if (continueText != null)
                        continueText.gameObject.SetActive(waitForInputWhenReady);

                    if (autoActivateWhenReady && !waitForInputWhenReady)
                        ActivateLoadedScene();
                }

                yield return null;
            }
        }

        private void UpdateProgressUI(float progress)
        {
            if (progressSlider != null)
                progressSlider.value = progress;

            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }
    }
}
