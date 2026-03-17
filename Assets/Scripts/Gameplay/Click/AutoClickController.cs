using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunClicker.InputSystem
{
    public class AutoClickController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ClickAreaController clickAreaController;
        [SerializeField] private Button triggerButton;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private Camera uiCamera;

        [Header("Duration")]
        [SerializeField] private float autoClickDuration = 30f;
        [SerializeField] private string countdownPrefix = "auto click:";

        [Header("Speed")]
        [SerializeField] private float minClicksPerSecond = 3f;
        [SerializeField] private float maxClicksPerSecond = 5f;

        private CanvasGroup triggerButtonCanvasGroup;
        private Coroutine autoClickRoutine;

        private bool IsRunning => autoClickRoutine != null;

        private void Awake()
        {
            if (triggerButton == null)
                triggerButton = GetComponent<Button>();

            if (triggerButton != null)
            {
                triggerButton.onClick.RemoveListener(ActivateAutoClick);
                triggerButton.onClick.AddListener(ActivateAutoClick);
                triggerButtonCanvasGroup = triggerButton.GetComponent<CanvasGroup>();

                if (triggerButtonCanvasGroup == null)
                    triggerButtonCanvasGroup = triggerButton.gameObject.AddComponent<CanvasGroup>();
            }

            SetCountdownVisible(false);
            SetButtonVisible(true);

            if (countdownText != null)
                countdownText.raycastTarget = false;
        }

        private void OnDestroy()
        {
            if (triggerButton != null)
                triggerButton.onClick.RemoveListener(ActivateAutoClick);
        }

        private void OnDisable()
        {
            if (!IsRunning)
                return;

            StopCoroutine(autoClickRoutine);
            autoClickRoutine = null;
            SetCountdownVisible(false);
            SetButtonVisible(true);
        }

        public void ActivateAutoClick()
        {
            if (IsRunning || clickAreaController == null)
                return;

            autoClickRoutine = StartCoroutine(RunAutoClick());
        }

        private IEnumerator RunAutoClick()
        {
            float remainingTime = Mathf.Max(0.1f, autoClickDuration);

            SetButtonVisible(false);
            SetCountdownVisible(true);

            while (remainingTime > 0f)
            {
                UpdateCountdownText(remainingTime);
                SimulateClick();

                float clicksPerSecond = Random.Range(
                    Mathf.Max(0.01f, minClicksPerSecond),
                    Mathf.Max(minClicksPerSecond, maxClicksPerSecond));

                float delay = 1f / clicksPerSecond;
                yield return new WaitForSeconds(delay);
                remainingTime -= delay;
            }

            UpdateCountdownText(0f);
            SetCountdownVisible(false);
            SetButtonVisible(true);
            autoClickRoutine = null;
        }

        private void SimulateClick()
        {
            if (clickAreaController == null)
                return;

            clickAreaController.HandleClick(GetScreenCenterPosition(), uiCamera);
        }

        private static Vector2 GetScreenCenterPosition()
        {
            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        private void UpdateCountdownText(float remainingTime)
        {
            if (countdownText == null)
                return;

            int displaySeconds = Mathf.CeilToInt(Mathf.Max(0f, remainingTime));
            countdownText.text = $"{countdownPrefix}{displaySeconds}s";
        }

        private void SetCountdownVisible(bool visible)
        {
            if (countdownText == null)
                return;

            countdownText.gameObject.SetActive(visible);
        }

        private void SetButtonVisible(bool visible)
        {
            if (triggerButtonCanvasGroup == null)
                return;

            triggerButtonCanvasGroup.alpha = visible ? 1f : 0f;
            triggerButtonCanvasGroup.interactable = visible;
            triggerButtonCanvasGroup.blocksRaycasts = visible;
        }
    }
}
