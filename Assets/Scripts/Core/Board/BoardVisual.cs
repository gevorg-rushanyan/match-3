using System.Collections.Generic;
using Configs;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardVisual : MonoBehaviour
    {
        [SerializeField] private Transform _blocksContainer;
        
        private Vector2 _blockSize;
        private float _blockZPositionOffset;
        private Dictionary<BlockType, BlockVisualConfig> _blockVisualConfigs = new ();
        private int _width; 
        private int _height;
        private bool _isInitialized;
        private int _activeAnimations;
        private bool _isAnimating;
        private BlockVisual[,] _visualGrid;
        
        public bool IsAnimating => _isAnimating;

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
                visual.OnAnimationStarted += AnimationStarted;
                visual.OnAnimationFinished += AnimationFinished;
            }

            float delta = ((1.0f * _width) / 2) * _blockSize.x;
            var position = _blocksContainer.position;
            position.x = -delta;
            _blocksContainer.position = position; 
        }

        public void MoveVisual(Vector2Int from, Vector2Int to)
        {
            if (_visualGrid == null)
            {
                return;
            }
            
            if (!InBounds(from) || !InBounds(to))
            {
                return;
            }
            
            if (_visualGrid[to.x, to.y] != null)
            {
                Debug.LogError("MoveVisual called but target is not empty, Need to use SwapVisual method");
                return;
            }
            
            var block = _visualGrid[from.x, from.y];
            if (block == null)
            {
                return;
            }

            _visualGrid[from.x, from.y] = null;
            _visualGrid[to.x, to.y] = block;
            
            block.SetGridPosition(to);
            block.MoveTo(BlockPosition(to.x, to.y));
        }
        
        public void SwapVisual(Vector2Int a, Vector2Int b)
        {
            if (_visualGrid == null)
            {
                return;
            }
            
            if (!InBounds(a) || !InBounds(b))
            {
                return;
            }
            
            var blockA = _visualGrid[a.x, a.y];
            var blockB = _visualGrid[b.x, b.y];

            if (blockA == null || blockB == null)
            {
                return;
            }

            _visualGrid[a.x, a.y] = blockB;
            _visualGrid[b.x, b.y] = blockA;

            blockA.SetGridPosition(b);
            blockB.SetGridPosition(a);

            blockA.MoveTo(BlockPosition(b.x, b.y));
            blockB.MoveTo(BlockPosition(a.x, a.y));
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
        
        // TODO rename to GridToWordldPosition
        private Vector3 BlockPosition(int x, int y)
        {
            return new Vector3(x * _blockSize.x, y * _blockSize.y, x * _blockZPositionOffset + y * _blockZPositionOffset);
        }
        
        private void ClearBoard()
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
                        block.OnAnimationStarted -= AnimationStarted;
                        block.OnAnimationFinished -= AnimationFinished;
                        Destroy(block.gameObject);
                        _visualGrid[x, y] = null;
                    }
                }
            }

            _visualGrid = null;
        }
        
        private bool InBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
        }
        
        private void AnimationStarted()
        {
            _activeAnimations++;
            if (_activeAnimations == 1)
            {
                _isAnimating = true;
            }
        }
        
        private void AnimationFinished()
        {
            _activeAnimations--;
            if (_activeAnimations <= 0)
            {
                _activeAnimations = 0;
                _isAnimating = false;
            }
        }
    }
}