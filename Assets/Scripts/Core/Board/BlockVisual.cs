using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BlockVisual : MonoBehaviour
    {
        private BlockType _type;
        private Vector2Int _gridPosition;
        
        public Vector2Int GridPosition => _gridPosition;
        
        public void Init(BlockType type, Vector2Int gridPosition, Vector3 worldPosition)
        {
            _type = type;
            _gridPosition = gridPosition;
            transform.localPosition = worldPosition;
            gameObject.SetActive(true);
        }

        public void SetGridPosition(Vector2Int pos)
        {
            _gridPosition = pos;
        }

        public void MoveTo(Vector3 worldPos)
        {
            transform.localPosition = worldPos;

            // Later
            // StartCoroutine(MoveCoroutine(worldPos));
        }
    }
}