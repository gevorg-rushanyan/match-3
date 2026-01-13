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

        public void MoveBlock(Vector2Int from, Vector2Int to)
        {
            var block = _model.Get(from.x, from.y);
            if (block == null)
            {
                return;
            }

            _model.Set(to.x, to.y, block);
            _model.Remove(from.x, from.y);
        }
    }
}