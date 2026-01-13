using Enums;
using UnityEngine;

namespace Core
{
    public class BlockVisual : MonoBehaviour
    {
        private BlockType _type;
        private Vector2Int _position;
        
        public void Init(BlockType type, Vector2Int position, Vector3 worldPosition)
        {
            _type = type;
            _position = position;
            transform.localPosition = worldPosition;
            gameObject.SetActive(true);
        }

        public void UpdatePosition(Vector2Int position, Vector3 worldPosition)
        {
            _position = position;
            transform.localPosition = worldPosition;
        }
    }
}