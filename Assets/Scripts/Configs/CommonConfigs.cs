using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct BlockVisualConfig
    {
        [SerializeField] private BlockType _type;
        [SerializeField] private Transform _prefab;
        public BlockType Type => _type;
        public Transform Prefab => _prefab;
    }

    [CreateAssetMenu(fileName = "CommonConfigs", menuName = "Configs/CommonConfigs")]
    public class CommonConfigs : ScriptableObject
    {
        [SerializeField] private Vector2 _blockSize;
        [SerializeField] private float _blockZPositionOffset;
        [SerializeField] private List<BlockVisualConfig> _blockVisualConfigs = new ();
        
        public Vector2 BlockSize => _blockSize;
        public float BlockZPositionOffset => _blockZPositionOffset;
        public IReadOnlyList<BlockVisualConfig> BlockVisualConfigs => _blockVisualConfigs;
    }
}