using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardSystem
    {
        private readonly BoardModel _model;
        private readonly int _matchCount;
        public event Action<Vector2Int, Vector2Int> Move;
        public event Action<Vector2Int, Vector2Int> Swap;
        public event Action<Vector2Int> Destroy;

        public BoardSystem(BoardModel model, int matchCount)
        {
            _model = model;
            _matchCount = matchCount;
        }
        
        public MoveType TryMoveBlock(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (!_model.InBounds(from.x, from.y) || !_model.InBounds(to.x, to.y))
            {
                return MoveType.None;
            }

            var fromBlock = _model.Get(from.x, from.y);
            if (fromBlock == null)
            {
                return MoveType.None;
            }

            bool isTargetEmpty = _model.Get(to.x, to.y) == null;

            // Block Move Up or Down to empty place 
            if (isTargetEmpty && (direction == Vector2Int.up || direction == Vector2Int.down))
            {
                return MoveType.None;
            }
            
            if (!isTargetEmpty)
            {
                var toBlock = _model.Get(to.x, to.y);
                _model.Set(to.x, to.y, fromBlock);
                _model.Set(from.x, from.y, toBlock);
                Swap?.Invoke(from, to);

                return MoveType.Swap;
            }

            // Move to free space
            if (direction == Vector2Int.left || direction == Vector2Int.right)
            {
                _model.Set(to.x, to.y, fromBlock);
                _model.Remove(from.x, from.y);
                Move?.Invoke(from, to);

                return MoveType.Move;
            }

            return MoveType.None;
        }

        public bool ApplyGravity()
        {
            bool anyMoved = false;

            for (int x = 0; x < _model.Width; x++)
            {
                for (int y = 0; y < _model.Height - 1; y++)
                {
                    if (_model.Get(x, y) != null)
                    {
                        continue;
                    }
                    
                    int aboveY = y + 1;

                    // Finding block to apply gravity 
                    while (aboveY < _model.Height && _model.Get(x, aboveY) == null)
                    {
                        aboveY++;
                    }

                    if (aboveY < _model.Height)
                    {
                        var block = _model.Get(x, aboveY);
                        _model.Set(x, y, block);
                        _model.Remove(x, aboveY);
                            
                        Move?.Invoke(new Vector2Int(x, aboveY), new Vector2Int(x, y));

                        anyMoved = true;
                    }
                }
            }

            return anyMoved;
        }
        
        public HashSet<Vector2Int> FindMatches()
        {
            var result = new HashSet<Vector2Int>();
            var visited = new bool[_model.Width][];
            for (int index = 0; index < _model.Width; index++)
            {
                visited[index] = new bool[_model.Height];
            }

            for (int x = 0; x < _model.Width; x++)
            {
                for (int y = 0; y < _model.Height; y++)
                {
                    if (visited[x][y])
                    {
                        continue;
                    }

                    var block = _model.Get(x, y);
                    if (block == null)
                    {
                        continue;
                    }

                    var connectedBlocks = _model.FindConnectedBlocks(new Vector2Int(x, y));
                    if (connectedBlocks == null || connectedBlocks.Count == 0)
                    {
                        continue;
                    }

                    foreach (var p in connectedBlocks)
                    {
                        visited[p.x][p.y] = true;
                    }
                    
                    var matches = _model.FindMatches(connectedBlocks, _matchCount);
                    if (matches != null && matches.Count > 0)
                    {
                        foreach (var point in matches)
                        {
                            result.Add(point);
                        }
                    }
                }
            }

            return result;
        }
        
        public bool DestroyBlocks(HashSet<Vector2Int> blocks)
        {
            if (blocks == null || blocks.Count == 0)
            {
                return false;
            }

            foreach (var block in blocks)
            {
                _model.Remove(block.x, block.y);
                Destroy?.Invoke(block);
            }

            return true;
        }
        
        public bool IsLevelCompleted()
        {
            return _model.IsEmpty;
        }
    }
}