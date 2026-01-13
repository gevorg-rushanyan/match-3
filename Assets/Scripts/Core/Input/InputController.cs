using Core.Board;
using Enums;
using UnityEngine;

namespace Core.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private SwipeInput _swipeInput;
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _blockLayer;

        private BoardSystem _boardSystem;
        
        private void Awake()
        {
            _swipeInput.OnSwipe += HandleSwipe;
        }

        public void Init(BoardSystem boardSystem)
        {
            _boardSystem = boardSystem;
        }

        private void HandleSwipe(Vector2 screenPos, SwipeDirection swipeDirection)
        {
            var block = GetBlockUnderScreen(screenPos);
            if (block == null)
            {
                return;
            }

            Vector2Int from = block.GridPosition;
            Vector2Int direction = DirectionToOffset(swipeDirection);
            Vector2Int to = from + direction;

            _boardSystem.TryMoveBlock(from, to, direction);
        }

        private BlockVisual GetBlockUnderScreen(Vector2 screenPos)
        {
            Vector2 worldPos = _camera.ScreenToWorldPoint(screenPos);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, _blockLayer);

            if (hit.collider != null)
            {
                return hit.collider.GetComponent<BlockVisual>();
            }

            return null;
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
            _swipeInput.OnSwipe -= HandleSwipe;
        }
    }
}