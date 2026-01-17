using System.Collections.Generic;
using System.Linq;
using Core.Persistence;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardModel
    {
        private int _width;
        private int _height;
        private BlockData[,] _grid;
        private int _blockCount;
        
        public int Width => _width;
        public int Height => _height;
        public bool IsEmpty => _blockCount == 0;

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
            if (_grid[x, y] == null && block != null)
            {
                _blockCount++;
            }
            else if (_grid[x, y] != null && block == null)
            {
                _blockCount--;
            }

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
            
            if (_grid[x, y] != null)
            {
                _grid[x, y] = null;
                _blockCount--;
            }
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
        
        public HashSet<Vector2Int> FindConnectedBlocks(Vector2Int start)
        {
            var startBlock = Get(start.x, start.y);
            if (startBlock == null)
            {
                return null;
            }

            BlockType type = startBlock.Type;
            var result = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();
            var visited = new bool[Width][];
            for (int index = 0; index < Width; index++)
            {
                visited[index] = new bool[Height];
            }

            stack.Push(start);

            while (stack.Count > 0)
            {
                var pos = stack.Pop();

                if (!InBounds(pos.x, pos.y))
                {
                    continue;
                }

                if (visited[pos.x][pos.y])
                {
                    continue;
                }

                var block = Get(pos.x, pos.y);
                if (block == null || block.Type != type)
                {
                    continue;
                }

                visited[pos.x][pos.y] = true;
                result.Add(pos);

                stack.Push(pos + Vector2Int.up);
                stack.Push(pos + Vector2Int.down);
                stack.Push(pos + Vector2Int.left);
                stack.Push(pos + Vector2Int.right);
            }

            return result;
        }
        
        public HashSet<Vector2Int> FindMatches(HashSet<Vector2Int> blocks, int matchCount)
        {
            var byRow = blocks.GroupBy(p => p.y);
            foreach (var row in byRow)
            {
                if (HasConsecutive(row.Select(p => p.x), matchCount))
                {
                    return blocks;
                }
            }
            
            var byColumn = blocks.GroupBy(p => p.x);
            foreach (var col in byColumn)
            {
                if (HasConsecutive(col.Select(p => p.y), matchCount))
                {
                    return blocks;
                }
            }

            return null;
        }
        
        private bool HasConsecutive(IEnumerable<int> values, int matchCount)
        {
            var ordered = values.OrderBy(v => v).ToList();

            int count = 1;
            for (int i = 1; i < ordered.Count; i++)
            {
                if (ordered[i] == ordered[i - 1] + 1)
                {
                    count++;
                    if (count >= matchCount)
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
        
        public BoardSaveData ToSaveData()
        {
            if (_blockCount == 0)
            {
                return null;
            }

            List<BlockSaveData> blocks = new List<BlockSaveData>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var block = Get(x, y);
                    if (block == null)
                    {
                        continue;
                    }

                    blocks.Add(new BlockSaveData(x, y, block.Type));
                }
            }

            return new BoardSaveData(_width, _height, blocks);
        }
    }
}