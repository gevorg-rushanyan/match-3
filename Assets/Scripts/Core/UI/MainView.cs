using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainView : UIView
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Button _playButton;
        
        public event Action PlaySelected;

        private void Start()
        {
            _playButton.onClick.AddListener(() => { PlaySelected?.Invoke(); });
        }

        public void SetLevel(int level)
        {
            _levelText.text = $"Level {level}";
        }
    }
}