using System;
using System.Collections.Generic;
using Configs;
using Enums;
using UnityEngine;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        private Dictionary<UIViewType, UIView> _windows = new ();
        private UIView _current;
        
        public event Action PlaySelected;

        public void Init(UIConfigs configs)
        {
            foreach (var window in configs.Views)
            {
                _windows[window.Type] = window.Prefab;
            }
        }
        
        public void Show(UIViewType type)
        {
            if (!_windows.TryGetValue(type, out var window))
            {
                return;
            }
            
            if (_current != null)
            {
                Destroy(_current.gameObject);
            }
            
            _current = Instantiate(window, _root);

            if (type == UIViewType.Main && _current is MainView main)
            {
                main.PlaySelected += OnPlaySelected;
            }
        }

        private void OnPlaySelected()
        {
            PlaySelected?.Invoke();
        }
    }
}