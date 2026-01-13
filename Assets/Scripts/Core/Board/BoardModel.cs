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
    }
}