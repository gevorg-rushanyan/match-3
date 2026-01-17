using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Board
{
    public class MatchFinder
    {
        private readonly BoardModel _model;
        private readonly int _matchCount;
        
        // Cached arrays to avoid GC allocations
        private bool[,] _visited;
        private int _cachedWidth;
        private int _cachedHeight;
        
        public MatchFinder(BoardModel model, int matchCount)
        {
            _model = model;
            _matchCount = matchCount;
        }
        
        public HashSet<Vector2Int> FindAllMatches()
        {
            var result = new HashSet<Vector2Int>();
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

                    var connectedBlocks = FindConnectedBlocks(new Vector2Int(x, y));
                    if (connectedBlocks == null || connectedBlocks.Count == 0)
                    {
                        continue;
                    }

                    foreach (var p in connectedBlocks)
                    {
                        _visited[p.x, p.y] = true;
                    }
                    
                    var matches = FindMatches(connectedBlocks);
                    if (matches && connectedBlocks.Count > 0)
                    {
                        foreach (var point in connectedBlocks)
                        {
                            result.Add(point);
                        }
                    }
                }
            }

            return result;
        }
        
        private HashSet<Vector2Int> FindConnectedBlocks(Vector2Int start)
        {
            var startBlock = _model.Get(start.x, start.y);
            if (startBlock == null)
            {
                return null;
            }

            var type = startBlock.Type;
            var result = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();
            
            EnsureVisitedCache();
            ClearVisited();

            stack.Push(start);

            while (stack.Count > 0)
            {
                var pos = stack.Pop();

                if (!_model.InBounds(pos.x, pos.y))
                {
                    continue;
                }

                if (_visited[pos.x, pos.y])
                {
                    continue;
                }

                var block = _model.Get(pos.x, pos.y);
                if (block == null || block.Type != type)
                {
                    continue;
                }

                _visited[pos.x, pos.y] = true;
                result.Add(pos);

                stack.Push(pos + Vector2Int.up);
                stack.Push(pos + Vector2Int.down);
                stack.Push(pos + Vector2Int.left);
                stack.Push(pos + Vector2Int.right);
            }

            return result;
        }
        
        private bool FindMatches(HashSet<Vector2Int> blocks)
        {
            var byRow = blocks.GroupBy(p => p.y);
            foreach (var row in byRow)
            {
                if (HasConsecutive(row.Select(p => p.x)))
                {
                    return true;
                }
            }
            
            var byColumn = blocks.GroupBy(p => p.x);
            foreach (var col in byColumn)
            {
                if (HasConsecutive(col.Select(p => p.y)))
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasConsecutive(IEnumerable<int> values)
        {
            var ordered = values.OrderBy(v => v).ToList();

            int count = 1;
            for (int i = 1; i < ordered.Count; i++)
            {
                if (ordered[i] == ordered[i - 1] + 1)
                {
                    count++;
                    if (count >= _matchCount)
                    {
                        return true;
                    }
                }
                else
                {
                    count = 1;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Finds only horizontal/vertical blocks which have minCount consecutive
        /// </summary>
        /// <param name="connectedBlocks"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        public HashSet<Vector2Int> FindMatches2(HashSet<Vector2Int> connectedBlocks, int minCount)
        {
            var result = new HashSet<Vector2Int>();

            foreach (var block in connectedBlocks)
            {
                // Horizontal
                int hCount = 1;

                var left = block + Vector2Int.left;
                while (connectedBlocks.Contains(left))
                {
                    hCount++;
                    left += Vector2Int.left;
                }

                var right = block + Vector2Int.right;
                while (connectedBlocks.Contains(right))
                {
                    hCount++;
                    right += Vector2Int.right;
                }

                if (hCount >= minCount)
                {
                    result.Add(block);
                }

                // Vertical
                int vCount = 1;

                var down = block + Vector2Int.down;
                while (connectedBlocks.Contains(down))
                {
                    vCount++;
                    down += Vector2Int.down;
                }

                var up = block + Vector2Int.up;
                while (connectedBlocks.Contains(up))
                {
                    vCount++;
                    up += Vector2Int.up;
                }

                if (vCount >= minCount)
                {
                    result.Add(block);
                }
            }

            return result;
        }
        
        private void EnsureVisitedCache()
        {
            if (_visited == null || _cachedWidth != _model.Width || _cachedHeight != _model.Height)
            {
                _visited = new bool[_model.Width, _model.Height];
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
