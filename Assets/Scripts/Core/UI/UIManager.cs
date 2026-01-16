using System;
using System.Collections.Generic;
using Configs;
using Core.Persistence;
using Enums;
using UnityEngine;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        private GameStats _gameStats;
        private Dictionary<UIViewType, UIView> _viewPrefabs = new ();
        private int _level;
        private List<KeyValuePair<UIViewType, UIView>> _openViews = new();
        
        public event Action PlaySelected;
        public event Action NextLevelSelected;
        public event Action RestartSelected;

        public void Init(UIConfigs configs, GameStats gameStats)
        {
            _gameStats = gameStats;
            foreach (var window in configs.Views)
            {
                _viewPrefabs[window.Type] = window.Prefab;
            }
        }
        
        public void Show(UIViewType type, bool closePreviousViews = true)
        {
            if (!_viewPrefabs.TryGetValue(type, out var window))
            {
                return;
            }

            if (closePreviousViews)
            {
                CloseOpenViews();
            }
            
            var view = Instantiate(window, _root);
            _openViews.Add(new KeyValuePair<UIViewType, UIView>(type, view));
            if (type == UIViewType.Main && view is MainView main)
            {
                main.Init(_gameStats);
                main.PlaySelected += OnPlaySelected;
            }
            
            if (type == UIViewType.Gameplay && view is GameplayView gameplay)
            {
                gameplay.NextLevelSelected += OnNextLevelSelected;
                gameplay.RestartSelected += OnRestartSelected;
            }
        }

        public void CloseView(UIViewType type)
        {
            var index = _openViews.FindIndex(x => x.Key == type);
            if (index != -1)
            {
                var keyValuePair = _openViews[index];
                Destroy(keyValuePair.Value.gameObject);
                _openViews.RemoveAt(index);
            }
        }

        private void CloseOpenViews()
        {
            foreach (var openView in _openViews)
            {
                Destroy(openView.Value.gameObject);
            }
            _openViews.Clear();
        }

        private void OnPlaySelected()
        {
            PlaySelected?.Invoke();
        }

        private void OnNextLevelSelected()
        {
            NextLevelSelected?.Invoke();
        }

        private void OnRestartSelected()
        {
            RestartSelected?.Invoke();
        }
    }
}