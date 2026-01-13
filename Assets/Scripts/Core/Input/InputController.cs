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

        private void HandleSwipe(Vector2 screenPos, SwipeDirection direction)
        {
            var block = GetBlockUnderScreen(screenPos);
            if (block == null)
            {
                return;
            }

            Vector2Int from = block.GridPosition;
            Vector2Int to = from + DirectionToOffset(direction);

            _boardSystem.MoveBlock(from, to);
        }

        private BlockVisual GetBlockUnderScreen(Vector2 screenPos)
        {
            Ray ray = _camera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _blockLayer))
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