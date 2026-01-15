using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class GameplayView : UIView
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;

        public event Action NextLevelSelected;
        public event Action RestartSelected;
        
        private void Start()
        {
            _nextLevelButton.onClick.AddListener(() => { NextLevelSelected?.Invoke(); });
            _restartButton.onClick.AddListener(() => { RestartSelected?.Invoke(); });
        }
    }
}