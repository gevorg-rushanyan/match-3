using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardModel
    {
        private int _width;
        private int _height;
        private BlockData[,] _grid;
        
        public int Width => _width;
        public int Height => _height;

        public BoardModel(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new BlockData[width, height];
        }

        public BlockData Get(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return null;
            }
            return _grid[x, y];
        }

        public void Set(int x, int y, BlockData block)
        {
            _grid[x, y] = block;
            if (block != null)
            {
                block.SetPosition(x, y);
            }
        }

        public void Remove(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return;
            }
            _grid[x, y] = null;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
        
        public bool IsEmpty(int x, int y)
        {
            return InBounds(x, y) && _grid[x, y] == null;
        }
        
        public List<Vector2Int> GetConnectedArea(Vector2Int start)
        {
            var startBlock = Get(start.x, start.y);
            if (startBlock == null)
            {
                return null;
            }

            BlockType type = startBlock.Type;
            var result = new List<Vector2Int>();
            var stack = new Stack<Vector2Int>();
            var visited = new bool[Width, Height];
            stack.Push(start);

            while (stack.Count > 0)
            {
                var pos = stack.Pop();

                if (!InBounds(pos.x, pos.y))
                {
                    continue;
                }

                if (visited[pos.x, pos.y])
                {
                    continue;
                }

                var block = Get(pos.x, pos.y);
                if (block == null || block.Type != type)
                {
                    continue;
                }

                visited[pos.x, pos.y] = true;
                result.Add(pos);

                stack.Push(pos + Vector2Int.up);
                stack.Push(pos + Vector2Int.down);
                stack.Push(pos + Vector2Int.left);
                stack.Push(pos + Vector2Int.right);
            }

            return result;
        }
        
        public bool HasMatchLine(List<Vector2Int> area)
        {
            // группируем по Y → горизонтали
            var byRow = area.GroupBy(p => p.y);
            foreach (var row in byRow)
            {
                if (HasConsecutive(row.Select(p => p.x)))
                {
                    return true;
                }
            }

            // группируем по X → вертикали
            var byColumn = area.GroupBy(p => p.x);
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
                    if (count >= 3)
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
    }
}