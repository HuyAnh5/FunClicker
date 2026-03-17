using Sirenix.OdinInspector;
using UnityEngine;

namespace FunClicker.Audio
{
    public class BackgroundMusicController : MonoBehaviour
    {
        private const string MusicVolumePrefKey = "settings.music_volume";

        public static BackgroundMusicController Instance { get; private set; }

        [Header("Source")]
        [SerializeField] private AudioSource musicSource;

        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float outputVolumeScale = 1f;
        [SerializeField] private bool useSavedVolume = true;
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool forceLoop = true;

        [ShowInInspector, ReadOnly]
        private float sourceMaxVolume = 1f;

        public float CurrentVolume => currentUserVolume;

        private float currentUserVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (musicSource == null)
                musicSource = GetComponent<AudioSource>();

            if (musicSource != null)
            {
                sourceMaxVolume = Mathf.Clamp01(musicSource.volume);

                if (forceLoop)
                    musicSource.loop = true;
            }

            bool hasSavedVolume = useSavedVolume && PlayerPrefs.HasKey(MusicVolumePrefKey);
            float startVolume = hasSavedVolume
                ? PlayerPrefs.GetFloat(MusicVolumePrefKey)
                : defaultVolume;

            ApplyVolume(startVolume, savePreference: !hasSavedVolume);
        }

        private void Start()
        {
            if (playOnStart)
                PlayMusic();
        }

        public void PlayMusic()
        {
            if (musicSource == null || musicSource.clip == null)
                return;

            if (forceLoop)
                musicSource.loop = true;

            if (!musicSource.isPlaying)
                musicSource.Play();
        }

        public void SetVolume(float volume)
        {
            ApplyVolume(volume, savePreference: true);
        }

        [Button(ButtonSizes.Medium)]
        public void ResetSavedVolumeToDefault()
        {
            ApplyVolume(defaultVolume, savePreference: true);
        }

        [Button(ButtonSizes.Medium)]
        public void ClearSavedVolume()
        {
            PlayerPrefs.DeleteKey(MusicVolumePrefKey);
            PlayerPrefs.Save();
            ApplyVolume(defaultVolume, savePreference: false);
        }

        private void ApplyVolume(float volume, bool savePreference)
        {
            currentUserVolume = Mathf.Clamp01(volume);

            if (musicSource != null)
            {
                float finalVolume = sourceMaxVolume * currentUserVolume * Mathf.Clamp01(outputVolumeScale);
                musicSource.volume = Mathf.Clamp01(finalVolume);
            }

            if (!savePreference)
                return;

            PlayerPrefs.SetFloat(MusicVolumePrefKey, currentUserVolume);
            PlayerPrefs.Save();
        }
    }
}
