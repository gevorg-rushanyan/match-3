using System.Collections.Generic;
using Configs;
using Enums;
using UnityEngine;

namespace Core
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Transform _blocksContainer;
        
        private Vector2 _blockSize;
        private float _blockZPositionOffset;
        private Dictionary<BlockType, BlockVisualConfig> _blockVisualConfigs = new ();
        private int _width; 
        private int _height;

        public void Init(CommonConfigs commonConfigs)
        {
            _blockSize = commonConfigs.BlockSize;
            _blockZPositionOffset = commonConfigs.BlockZPositionOffset;
            
            _blockVisualConfigs.Clear();
            foreach (var visualConfig in commonConfigs.BlockVisualConfigs)
            {
                if (!_blockVisualConfigs.TryAdd(visualConfig.Type, visualConfig))
                {
                    return;
                }
            }
        }

        public void CreateBoard(LevelConfig levelConfig)
        {
            _width = levelConfig.Width;
            _height = levelConfig.Height;

            foreach (var blockConfig in levelConfig.Blocks)
            {
                if (!TryCreateBlock(blockConfig))
                {
                    Debug.LogError("Create block failed");
                    return;
                }
            }

            float delta = ((1.0f * _width) / 2) * _blockSize.x;
            var position = _blocksContainer.position;
            position.x = -delta;
            transform.position = position; 
        }

        private bool TryCreateBlock(BlockConfig blockConfig)
        {
            var position = BlockPosition(blockConfig.Position.x, blockConfig.Position.y);
            Transform item = null;

            if (!_blockVisualConfigs.TryGetValue(blockConfig.Type, out var visualConfig))
            {
                return false;
            }

            if (visualConfig.Prefab == null)
            {
                return false;
            }

            item = Instantiate(visualConfig.Prefab, _blocksContainer);
            item.gameObject.SetActive(true);
            item.localPosition = position;

            return true;
        }
        
        private Vector3 BlockPosition(int x, int y)
        {
            return new Vector3(x * _blockSize.x, y * _blockSize.y, x * _blockZPositionOffset + y * _blockZPositionOffset);
        }
    }
}