using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainView : UIView
    {
        [SerializeField] private Button _playButton;
        
        public event Action PlaySelected;

        private void Start()
        {
            _playButton.onClick.AddListener(() => { PlaySelected?.Invoke(); });
        }
    }
}