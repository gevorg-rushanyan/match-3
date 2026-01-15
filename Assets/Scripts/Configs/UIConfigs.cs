using System;
using System.Collections.Generic;
using Core.UI;
using Enums;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct UIViewConfig
    {
        [SerializeField] private UIViewType _type;
        [SerializeField] private UIView _prefab;
        
        public UIViewType Type => _type;
        public UIView Prefab => _prefab;
    }
    
    [CreateAssetMenu(fileName = "UIConfigs", menuName = "Configs/UIConfigs")]
    public class UIConfigs : ScriptableObject
    {
        [SerializeField] private List<UIViewConfig> _views;
        
        public IReadOnlyList<UIViewConfig> Views => _views;
    }
}