using System;
using UnityEngine;

namespace FunClicker.Core
{
    public class PointManager : MonoBehaviour
    {
        public static PointManager Instance { get; private set; }

        [SerializeField] private long currentPoints = 0;
        [SerializeField] private long baseScorePerClick = 1;
        [SerializeField] private long scorePerSecond = 0;

        public long CurrentPoints => currentPoints;
        public long BaseScorePerClick => baseScorePerClick;
        public long ScorePerSecond => scorePerSecond;

        public event Action<long> OnPointsChanged;
        public event Action<long, long> OnPointStatsChanged;

        private double passivePointProgress;

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
            OnPointsChanged?.Invoke(currentPoints);
            OnPointStatsChanged?.Invoke(baseScorePerClick, scorePerSecond);
        }

        private void Update()
        {
            if (scorePerSecond <= 0)
                return;

            passivePointProgress += scorePerSecond * Time.deltaTime;

            long wholePoints = (long)passivePointProgress;
            if (wholePoints <= 0)
                return;

            passivePointProgress -= wholePoints;
            AddPoints(wholePoints);
        }

        public void AddPoints(long amount)
        {
            if (amount <= 0) return;

            currentPoints += amount;
            OnPointsChanged?.Invoke(currentPoints);
        }

        public void SetPoints(long value)
        {
            currentPoints = Math.Max(0, value);
            OnPointsChanged?.Invoke(currentPoints);
        }

        public void SetBaseScorePerClick(long value)
        {
            baseScorePerClick = Math.Max(0, value);
            OnPointStatsChanged?.Invoke(baseScorePerClick, scorePerSecond);
        }

        public void SetScorePerSecond(long value)
        {
            scorePerSecond = Math.Max(0, value);
            passivePointProgress = 0;
            OnPointStatsChanged?.Invoke(baseScorePerClick, scorePerSecond);
        }

        public bool CanAfford(long amount)
        {
            return currentPoints >= amount;
        }

        public bool TrySpendPoints(long amount)
        {
            if (amount <= 0) return false;
            if (currentPoints < amount) return false;

            currentPoints -= amount;
            OnPointsChanged?.Invoke(currentPoints);
            return true;
        }
    }
}
