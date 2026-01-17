using System.Collections.Generic;
using Core.Persistence;

namespace Core.Board
{
    public class BoardModel
    {
        private readonly int _width;
        private readonly int _height;
        private readonly BlockData[,] _grid;
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