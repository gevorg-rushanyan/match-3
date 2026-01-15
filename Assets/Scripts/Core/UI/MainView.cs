using System;
using Core.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainView : UIView
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Button _playButton;
        private GameStats _gameStats;
        public event Action PlaySelected;

        private void Start()
        {
            _playButton.onClick.AddListener(() =>
            {
                // TMP
                gameObject.SetActive(false);
                PlaySelected?.Invoke();
            });
        }

        public void Init(GameStats gameStats)
        {
            _gameStats = gameStats;
            _gameStats.OnLevelChanged += OnLevelChanged;
            OnLevelChanged(_gameStats.Level);
        }

        private void OnLevelChanged(int level)
        {
            _levelText.text = $"Level {_gameStats.Level + 1}";
        }

        private void OnDestroy()
        {
            if (_gameStats != null)
            {
                _gameStats.OnLevelChanged -= OnLevelChanged;
                _gameStats = null;
            }
        }
    }
}