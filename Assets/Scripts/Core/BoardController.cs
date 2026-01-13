using System.Collections.Generic;
using Configs;
using Enums;
using UnityEngine;

namespace Core
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Transform _blocksContainer;
        
        private Vector2 _blockSize;
        private float _blockZPositionOffset;
        private Dictionary<BlockType, BlockVisualConfig> _blockVisualConfigs = new ();
        private int _width; 
        private int _height;
        private bool _isInitialized;
        
        private BlockVisual[,] _visualGrid;

        public void Init(CommonConfigs commonConfigs)
        {
            _blockSize = commonConfigs.BlockSize;
            _blockZPositionOffset = commonConfigs.BlockZPositionOffset;
            
            _blockVisualConfigs.Clear();
            foreach (var visualConfig in commonConfigs.BlockVisualConfigs)
            {
                if (!_blockVisualConfigs.TryAdd(visualConfig.Type, visualConfig))
                {
                    Debug.LogError($"Duplicate, could not add visual {visualConfig.Type}");
                    return;
                }
            }

            _isInitialized = true;
        }

        public void CreateBoard(LevelConfig levelConfig)
        {
            if (!_isInitialized)
            {
                Debug.LogError($"Cannot create board, not initialized");
                return;
            }

            ClearBoard();
            _width = levelConfig.Width;
            _height = levelConfig.Height;
            _visualGrid = new BlockVisual[_width, _height];

            foreach (var blockConfig in levelConfig.Blocks)
            {
                var visual = TryCreateBlock(blockConfig);
                if (visual == null)
                {
                    Debug.LogError("Create block failed");
                    return;
                }
                _visualGrid[blockConfig.Position.x, blockConfig.Position.y] = visual;
            }

            float delta = ((1.0f * _width) / 2) * _blockSize.x;
            var position = _blocksContainer.position;
            position.x = -delta;
            _blocksContainer.position = position; 
        }

        private BlockVisual TryCreateBlock(BlockConfig blockConfig)
        {
            var position = BlockPosition(blockConfig.Position.x, blockConfig.Position.y);

            if (!_blockVisualConfigs.TryGetValue(blockConfig.Type, out var visualConfig))
            {
                return null;
            }

            if (visualConfig.Prefab == null)
            {
                return null;
            }

            BlockVisual blockVisual = Instantiate(visualConfig.Prefab, _blocksContainer);
            blockVisual.Init(blockConfig.Type, blockConfig.Position, position);
            return blockVisual;
        }
        
        private Vector3 BlockPosition(int x, int y)
        {
            return new Vector3(x * _blockSize.x, y * _blockSize.y, x * _blockZPositionOffset + y * _blockZPositionOffset);
        }
        
        public void ClearBoard()
        {
            if (_visualGrid == null)
            {
                return;
            }

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var block = _visualGrid[x, y];
                    if (block != null)
                    {
                        Destroy(block.gameObject);
                        _visualGrid[x, y] = null;
                    }
                }
            }

            _visualGrid = null;
        }
    }
}