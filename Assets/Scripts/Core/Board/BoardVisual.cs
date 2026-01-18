using System.Collections.Generic;
using Configs;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardVisual : MonoBehaviour
    {
        [SerializeField] private Transform _blocksContainer;
        
        private readonly Dictionary<BlockType, BlockVisualConfig> _blockVisualConfigs = new();
        private BoardSystem _boardSystem;
        private BlockVisualPool _pool;
        private Vector2 _blockSize;
        private float _blockZPositionOffset;
        private int _width; 
        private int _height;
        private bool _isInitialized;
        private int _activeAnimations;
        private BlockVisual[,] _visualGrid;
        
        public bool IsAnimating => _activeAnimations != 0;

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

            InitializePool();
            _isInitialized = true;
        }
        
        private void InitializePool()
        {
            _pool = new BlockVisualPool(_blocksContainer);
            
            foreach (var config in _blockVisualConfigs)
            {
                if (config.Value.Prefab != null)
                {
                    _pool.RegisterPrefab(config.Key, config.Value.Prefab);
                }
            }
        }

        public void CreateBoard(BoardModel model, BoardSystem boardSystem)
        {
            if (!_isInitialized)
            {
                Debug.LogError("BoardVisual not initialized");
                return;
            }

            if (model == null)
            {
                Debug.LogError("BoardModel is null");
                return;
            }
            
            ClearBoard();
            _width = model.Width;
            _height = model.Height;
            _visualGrid = new BlockVisual[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var blockData = model.Get(x, y);
                    if (blockData == null)
                    {
                        continue;
                    }

                    var gridPosition = blockData.Position;
                    var visual = GetBlock(gridPosition, blockData.Type);
                    if (visual == null)
                    {
                        Debug.LogError("Create block failed");
                        return;
                    }
                    _visualGrid[x, y] = visual;
                    visual.AnimationStarted += OnAnimationStarted;
                    visual.AnimationFinished += OnAnimationFinished;
                }
            }
            SetBoardSystem(boardSystem);
            CenterBoardHorizontally();
        }

        private void SetBoardSystem(BoardSystem boardSystem)
        {
            UnsubscribeFromBoardEvents();
            _boardSystem = boardSystem;
            _boardSystem.Move += MoveBlock;
            _boardSystem.Swap += SwapBlocks;
            _boardSystem.Destroy += DestroyBlock;
        }

        private void UnsubscribeFromBoardEvents()
        {
            if (_boardSystem == null)
            {
                return;
            }
            
            _boardSystem.Move -= MoveBlock;
            _boardSystem.Swap -= SwapBlocks;
            _boardSystem.Destroy -= DestroyBlock;
            _boardSystem = null;
        }

        private void MoveBlock(Vector2Int from, Vector2Int to)
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
            block.MoveTo(GridToWorldPosition(to.x, to.y));
        }
        
        private void SwapBlocks(Vector2Int a, Vector2Int b)
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

            blockA.MoveTo(GridToWorldPosition(b.x, b.y));
            blockB.MoveTo(GridToWorldPosition(a.x, a.y));
        }
        
        private void DestroyBlock(Vector2Int pos)
        {
            var block = _visualGrid[pos.x, pos.y];
            if (block == null)
            {
                return;
            }

            _visualGrid[pos.x, pos.y] = null;
            block.PlayDestroyAnimation(() =>
            {
                ReturnBlock(block);
            });
        }

        private BlockVisual GetBlock(Vector2Int position, BlockType blockType)
        {
            var worldPosition = GridToWorldPosition(position.x, position.y);

            var block = _pool.Get(blockType);
            if (block == null)
            {
                return null;
            }
            
            block.Init(blockType, position, worldPosition);
            return block;
        }
        
        private void ReturnBlock(BlockVisual block)
        {
            if (block == null)
            {
                return;
            }
            
            block.AnimationStarted -= OnAnimationStarted;
            block.AnimationFinished -= OnAnimationFinished;
            _pool.Return(block);
        }
        
        private Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(
                x * _blockSize.x,
                y * _blockSize.y,
                x * _blockZPositionOffset + y * _blockZPositionOffset);
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
                        ReturnBlock(block);
                        _visualGrid[x, y] = null;
                    }
                }
            }

            _visualGrid = null;
        }
        
        private void CenterBoardHorizontally()
        {
            float delta = (_width * _blockSize.x) / 2f;
            var position = _blocksContainer.position;
            position.x = -delta;
            _blocksContainer.position = position;
        }
        
        private bool InBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
        }
        
        private void OnAnimationStarted()
        {
            _activeAnimations++;
        }
        
        private void OnAnimationFinished()
        {
            _activeAnimations--;
        }

        private void OnDestroy()
        {
            UnsubscribeFromBoardEvents();
            _pool?.Clear();
        }
    }
}
