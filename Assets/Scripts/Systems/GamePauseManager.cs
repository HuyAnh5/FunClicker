using UnityEngine;

namespace FunClicker.Systems
{
    public class GamePauseManager : MonoBehaviour
    {
        public static GamePauseManager Instance { get; private set; }

        private static readonly float DefaultFixedDeltaTime = 0.02f;

        private int pauseRequestCount;
        private float cachedFixedDeltaTime = DefaultFixedDeltaTime;

        public bool IsPaused => pauseRequestCount > 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            cachedFixedDeltaTime = Time.fixedDeltaTime > 0f ? Time.fixedDeltaTime : DefaultFixedDeltaTime;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                ResumeAll();
                Instance = null;
            }
        }

        public void PauseGame()
        {
            pauseRequestCount++;
            ApplyPauseState();
        }

        public void ResumeGame()
        {
            if (pauseRequestCount <= 0)
                return;

            pauseRequestCount--;
            ApplyPauseState();
        }

        public void ResumeAll()
        {
            pauseRequestCount = 0;
            ApplyPauseState();
        }

        private void ApplyPauseState()
        {
            bool shouldPause = pauseRequestCount > 0;

            if (shouldPause)
            {
                if (Time.timeScale > 0f)
                    cachedFixedDeltaTime = Time.fixedDeltaTime;

                Time.timeScale = 0f;
                Time.fixedDeltaTime = 0f;
                AudioListener.pause = true;
                return;
            }

            Time.timeScale = 1f;
            Time.fixedDeltaTime = cachedFixedDeltaTime > 0f ? cachedFixedDeltaTime : DefaultFixedDeltaTime;
            AudioListener.pause = false;
        }
    }
}
