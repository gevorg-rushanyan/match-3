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

            // Block Move Up if top is empty
            if (isTargetEmpty && direction == Vector2Int.up)
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
    }
}