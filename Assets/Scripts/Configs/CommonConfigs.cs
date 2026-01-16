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
        /// <summary>
        /// These dimensions correspond to the current background dimensions.
        /// If the matrix will large, based on these dimensions will calculate a new scale.
        /// </summary>
        [Space(5)]
        [Header("These dimensions correspond to the current background dimensions. \n If the matrix will large, based on these dimensions will calculate a new scale.")]
        [SerializeField] private Vector2 _defaultBackgroundSize;
        
        [Space(10)]
        [Header("Block configurations")]
        [SerializeField] private Vector2 _blockSize;
        [SerializeField] private float _blockZPositionOffset;
        [SerializeField] private List<BlockVisualConfig> _blockVisualConfigs = new ();
        
        public Vector2 BlockSize => _blockSize;
        public float BlockZPositionOffset => _blockZPositionOffset;
        public IReadOnlyList<BlockVisualConfig> BlockVisualConfigs => _blockVisualConfigs;
    }
}