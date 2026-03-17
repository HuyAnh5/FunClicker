using UnityEngine;

namespace FunClicker.Core
{
    public class FrameRateController : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool disableVSync = true;

        private void Awake()
        {
            ApplyFrameRate();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            ApplyFrameRate();
        }

        private void ApplyFrameRate()
        {
            if (disableVSync)
                QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = Mathf.Max(30, targetFrameRate);
        }
    }
}
