using Configs;
using Enums;
using UnityEngine;

namespace Core
{
    public class BoardController : MonoBehaviour
    {
        public Transform firePrefab;
        public Transform waterPrefab;
        public float zPositionOffset = 0.01f;
        public float yPositionOffset = 0.01f;
        public Vector2 blockSize = new Vector2 (0.89f, 0.8f);
        
        [SerializeField] private Transform _blocksContainer;
        
        public void CreateBoard(LevelConfig levelConfig)
        {
            int width = levelConfig.Width;
            int height = levelConfig.Height;

            foreach (var blockConfig in levelConfig.Blocks)
            {
                CreateBlock(blockConfig);
            }

            float delta = ((1.0f* width) / 2) * blockSize.x;

            var position = transform.position;
            position.x = -delta;
            transform.position = position; 
        }

        private void CreateBlock(BlockConfig blockConfig)
        {
            var position = BlockPosition(blockConfig.Position.x, blockConfig.Position.y);
            Transform item = null;
            if (blockConfig.Type == BlockType.Fire)
            {
                item = Instantiate(firePrefab, _blocksContainer);
            }
            else if (blockConfig.Type == BlockType.Water)
            {
                item = Instantiate(waterPrefab, _blocksContainer);
            }

            if (item == null)
            {
                return;
            }

            item.gameObject.SetActive(true);
            item.localPosition = position;
        }
        
        private Vector3 BlockPosition(int x, int y)
        {
            return new Vector3(x * blockSize.x, y * blockSize.y, x * zPositionOffset + y * zPositionOffset);
        }
    }
}