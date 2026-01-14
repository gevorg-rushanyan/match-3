using UnityEngine;

namespace Core.Board
{
    public class BoardSystem
    {
        private BoardModel _model;
        private BoardVisual _view;

        public BoardSystem(BoardModel model, BoardVisual view)
        {
            _model = model;
            _view = view;
        }
        
        public bool TryMoveBlock(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (!_model.InBounds(from.x, from.y) || !_model.InBounds(to.x, to.y))
            {
                return false;
            }

            var fromBlock = _model.Get(from.x, from.y);
            if (fromBlock == null)
            {
                return false;
            }

            bool isTargetEmpty = _model.Get(to.x, to.y) == null;

            // Block Move Up or Down to empty place 
            if (isTargetEmpty && (direction == Vector2Int.up || direction == Vector2Int.down))
            {
                return false;
            }
            
            if (!isTargetEmpty)
            {
                var toBlock = _model.Get(to.x, to.y);
                _model.Set(to.x, to.y, fromBlock);
                _model.Set(from.x, from.y, toBlock);

                _view.SwapVisual(from, to);

                return true;
            }

            // Move to free space
            if (direction == Vector2Int.left || direction == Vector2Int.right)
            {
                _model.Set(to.x, to.y, fromBlock);
                _model.Remove(from.x, from.y);
                _view.MoveVisual(from, to);

                return true;
            }

            return false;
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
                            
                        _view.MoveVisual(new Vector2Int(x, aboveY), new Vector2Int(x, y));

                        anyMoved = true;
                    }
                }
            }

            return anyMoved;
        }
    }
}