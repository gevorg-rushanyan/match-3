using System;
using Enums;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct BlockConfig
    {
        [SerializeField] private BlockType _type;
        [SerializeField] private Vector2Int _position;
        
        public BlockType Type => _type;
        public Vector2Int Position => _position;
        
        public BlockConfig(BlockType type, Vector2Int position)
        {
            _type = type;
            _position = position;
        }
    }
}