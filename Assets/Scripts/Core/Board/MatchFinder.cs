using System.Collections.Generic;
using UnityEngine;

namespace Core.Board
{
    public class MatchFinder
    {
        private const int DefaultCapacity = 32;
        private readonly BoardModel _model;
        private readonly int _matchCount;
        
        // Cached arrays to avoid GC allocations
        private bool[,] _visited;
        private bool[,] _floodFillVisited;
        private int _cachedWidth;
        private int _cachedHeight;
        
        // Cached collections to avoid GC allocations
        private readonly Stack<Vector2Int> _stack = new(DefaultCapacity);
        private readonly HashSet<Vector2Int> _connectedBlocks = new(DefaultCapacity);
        private readonly HashSet<Vector2Int> _resultSet = new(DefaultCapacity);
        
        public MatchFinder(BoardModel model, int matchCount)
        {
            _model = model;
            _matchCount = matchCount;
        }
        
        public HashSet<Vector2Int> FindAllMatches()
        {
            _resultSet.Clear();
            EnsureVisitedCache();
            ClearVisited();

            for (int x = 0; x < _model.Width; x++)
            {
                for (int y = 0; y < _model.Height; y++)
                {
                    if (_visited[x, y])
                    {
                        continue;
                    }

                    var block = _model.Get(x, y);
                    if (block == null)
                    {
                        continue;
                    }

                    FindConnectedBlocks(new Vector2Int(x, y));
                    if (_connectedBlocks.Count == 0)
                    {
                        continue;
                    }

                    foreach (var p in _connectedBlocks)
                    {
                        _visited[p.x, p.y] = true;
                    }
                    
                    if (HasMatch(_connectedBlocks))
                    {
                        foreach (var point in _connectedBlocks)
                        {
                            _resultSet.Add(point);
                        }
                    }
                }
            }

            return _resultSet;
        }
        
        private void FindConnectedBlocks(Vector2Int start)
        {
            _connectedBlocks.Clear();
            _stack.Clear();
            
            var startBlock = _model.Get(start.x, start.y);
            if (startBlock == null)
            {
                return;
            }

            var type = startBlock.Type;
            
            EnsureVisitedCache();
            System.Array.Clear(_floodFillVisited, 0, _floodFillVisited.Length);

            _stack.Push(start);

            while (_stack.Count > 0)
            {
                var pos = _stack.Pop();

                if (!_model.InBounds(pos.x, pos.y))
                {
                    continue;
                }

                if (_floodFillVisited[pos.x, pos.y])
                {
                    continue;
                }

                var block = _model.Get(pos.x, pos.y);
                if (block == null || block.Type != type)
                {
                    continue;
                }

                _floodFillVisited[pos.x, pos.y] = true;
                _connectedBlocks.Add(pos);

                _stack.Push(pos + Vector2Int.up);
                _stack.Push(pos + Vector2Int.down);
                _stack.Push(pos + Vector2Int.left);
                _stack.Push(pos + Vector2Int.right);
            }
        }
        
        private bool HasMatch(HashSet<Vector2Int> blocks)
        {
            foreach (var block in blocks)
            {
                // Check horizontal match
                int horizontalCount = GetMatchCountByDirection(block, blocks, new [] { Vector2Int.left, Vector2Int.right });
                
                if (horizontalCount >= _matchCount)
                {
                    return true;
                }
                
                // Check vertical match
                int verticalCount = GetMatchCountByDirection(block, blocks, new [] { Vector2Int.down, Vector2Int.up });
                
                if (verticalCount >= _matchCount)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private int GetMatchCountByDirection(Vector2Int block, HashSet<Vector2Int> blocks, Vector2Int[] directions)
        {
            int count = 1;
            
            foreach (var direction in directions)
            {
                var nextItem = block + direction;
                while (blocks.Contains(nextItem))
                {
                    count++;
                    nextItem += direction;
                }
            }
            
            return count;
        }

        private void EnsureVisitedCache()
        {
            if (_visited == null || _cachedWidth != _model.Width || _cachedHeight != _model.Height)
            {
                _visited = new bool[_model.Width, _model.Height];
                _floodFillVisited = new bool[_model.Width, _model.Height];
                _cachedWidth = _model.Width;
                _cachedHeight = _model.Height;
            }
        }
        
        private void ClearVisited()
        {
            System.Array.Clear(_visited, 0, _visited.Length);
        }
    }
}
