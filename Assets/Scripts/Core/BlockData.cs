using Enums;
using UnityEngine;

namespace Core
{
    public class BlockData
    {
        private readonly BlockType _type;
        private Vector2Int _position;
        
        public BlockType Type => _type;
        public Vector2Int Position => _position;
        
        public BlockData(BlockType type)
        {
            _type = type;
        }

        public void SetPosition(int x, int y)
        {
            _position = new Vector2Int(x, y);
        }
    }
}