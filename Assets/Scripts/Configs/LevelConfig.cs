using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Level", menuName = "Configs/Level")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private List<BlockConfig> _blocks = new List<BlockConfig>();
        
        public int Width => _width;
        public int Height => _height;
        public IReadOnlyList<BlockConfig> Blocks => _blocks;
        
#if UNITY_EDITOR
        [ContextMenu("Fill Matrix")]
        private void FillMatrix()
        {
            _blocks.Clear();
            for (int i = 1; i < _width - 1; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _blocks.Add(new BlockConfig(BlockType.Fire, new Vector2Int(i, j)));
                }
            }
        }
#endif
    }
}