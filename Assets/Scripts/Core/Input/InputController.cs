using System;
using Core.Board;
using Enums;
using UnityEngine;

namespace Core.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _blockLayer;
        [SerializeField] private SwipeInput _swipeInput;

        private BoardVisual _boardVisual;
        
        public event Action<Vector2Int, Vector2Int, Vector2Int> OnSwipe;
        
        private void Awake()
        {
            if (_swipeInput != null)
            {
                _swipeInput.OnSwipe += HandleSwipe;
            }
        }

        public void Init(BoardVisual boardVisual)
        {
            _boardVisual = boardVisual;
        }

        private void HandleSwipe(Vector2 screenPos, SwipeDirection swipeDirection)
        {
            var block = GetBlockUnderScreen(screenPos);
            if (block == null)
            {
                return;
            }

            if (block.State != BlockVisualState.Idle)
            {
                return;
            }

            Vector2Int from = block.GridPosition;
            Vector2Int direction = DirectionToOffset(swipeDirection);
            Vector2Int to = from + direction;

            OnSwipe?.Invoke(from, to, direction);
        }

        private BlockVisual GetBlockUnderScreen(Vector2 screenPos)
        {
            Vector2 worldPos = _camera.ScreenToWorldPoint(screenPos);
            return _boardVisual.GetBlock(worldPos);
        }

        private Vector2Int DirectionToOffset(SwipeDirection direction)
        {
            return direction switch
            {
                SwipeDirection.Up => Vector2Int.up,
                SwipeDirection.Down => Vector2Int.down,
                SwipeDirection.Left => Vector2Int.left,
                SwipeDirection.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
        
        private void OnDestroy()
        {
            if (_swipeInput != null)
            {
                _swipeInput.OnSwipe -= HandleSwipe;
            }
        }
    }
}