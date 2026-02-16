using System;
using System.Collections.Generic;
using Core.Board;
using Enums;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct BlockVisualConfig
    {
        [SerializeField] private BlockType _type;
        [SerializeField] private BlockVisual _prefab;
        public BlockType Type => _type;
        public BlockVisual Prefab => _prefab;
    }

    [CreateAssetMenu(fileName = "CommonConfigs", menuName = "Configs/CommonConfigs")]
    public class CommonConfigs : ScriptableObject
    {
        [SerializeField] private int _targetFPS;
        [SerializeField] private int _maxBlockCountToCreateInOneFrame;
        [Header("Minimum block count for match ")]
        [SerializeField] private int _matchCount;
        [Space(10)]
        [Header("Block configurations")]
        [SerializeField] private Vector2 _blockSize;
        [SerializeField] private float _blockZPositionOffset;
        [SerializeField] private List<BlockVisualConfig> _blockVisualConfigs = new ();
        
        public int TargetFPS => _targetFPS;
        public int MaxBlockCountToCreateInOneFrame => _maxBlockCountToCreateInOneFrame;
        public int MatchCount => _matchCount;
        public Vector2 BlockSize => _blockSize;
        public float BlockZPositionOffset => _blockZPositionOffset;
        public IReadOnlyList<BlockVisualConfig> BlockVisualConfigs => _blockVisualConfigs;
    }
}