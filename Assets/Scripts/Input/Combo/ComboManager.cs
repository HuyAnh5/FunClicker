using System;
using UnityEngine;

namespace FunClicker.Core
{
    public class ComboManager : MonoBehaviour
    {
        public static ComboManager Instance { get; private set; }

        [Header("Combo Value")]
        [SerializeField, Range(0f, 1f)] private float comboValue = 0f;
        [SerializeField] private float comboGainPerClick = 0.08f;
        [SerializeField] private float comboDecayPerSecond = 0.20f;

        [Header("Threshold")]
        [SerializeField, Range(0f, 1f)] private float x2Threshold = 0.5f;
        [SerializeField, Range(0f, 1f)] private float x4Threshold = 0.75f;

        public float ComboValue => comboValue;
        public int CurrentMultiplier => GetMultiplier(comboValue);

        public event Action<float, int> OnComboChanged;
        public event Action<int> OnMultiplierChanged;

        private int lastMultiplier = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            lastMultiplier = CurrentMultiplier;
            NotifyComboChanged();
        }

        private void Update()
        {
            if (comboValue <= 0f)
                return;

            float oldValue = comboValue;
            comboValue -= comboDecayPerSecond * Time.deltaTime;
            comboValue = Mathf.Clamp01(comboValue);

            if (!Mathf.Approximately(oldValue, comboValue))
            {
                CheckMultiplierChanged();
                NotifyComboChanged();
            }
        }

        public int RegisterClickAndGetMultiplier()
        {
            comboValue += comboGainPerClick;
            comboValue = Mathf.Clamp01(comboValue);

            CheckMultiplierChanged();
            NotifyComboChanged();

            return CurrentMultiplier;
        }

        public void ResetCombo()
        {
            comboValue = 0f;
            CheckMultiplierChanged();
            NotifyComboChanged();
        }

        private int GetMultiplier(float value)
        {
            if (value >= x4Threshold) return 4;
            if (value >= x2Threshold) return 2;
            return 1;
        }

        private void CheckMultiplierChanged()
        {
            int newMultiplier = CurrentMultiplier;
            if (newMultiplier != lastMultiplier)
            {
                lastMultiplier = newMultiplier;
                OnMultiplierChanged?.Invoke(newMultiplier);
            }
        }

        private void NotifyComboChanged()
        {
            OnComboChanged?.Invoke(comboValue, CurrentMultiplier);
        }
    }
}