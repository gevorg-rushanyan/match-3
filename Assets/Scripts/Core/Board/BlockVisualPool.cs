using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BlockVisualPool
    {
        private readonly Dictionary<BlockType, Stack<BlockVisual>> _pools = new();
        private readonly Dictionary<BlockType, BlockVisual> _prefabs = new();
        private readonly Transform _parent;
        
        public BlockVisualPool(Transform parent)
        {
            _parent = parent;
        }
        
        public void RegisterPrefab(BlockType type, BlockVisual prefab)
        {
            if (!_prefabs.ContainsKey(type))
            {
                _prefabs[type] = prefab;
                _pools[type] = new Stack<BlockVisual>();
            }
        }
        
        public BlockVisual Get(BlockType type)
        {
            if (!_pools.TryGetValue(type, out var pool))
            {
                Debug.LogError($"No pool registered for BlockType: {type}");
                return null;
            }
            
            BlockVisual block;
            
            if (pool.Count > 0)
            {
                block = pool.Pop();
            }
            else
            {
                if (!_prefabs.TryGetValue(type, out var prefab))
                {
                    Debug.LogError($"No prefab registered for BlockType: {type}");
                    return null;
                }
                
                block = Object.Instantiate(prefab, _parent);
            }
            
            block.Activate();
            return block;
        }
        
        public void Return(BlockVisual block)
        {
            if (block == null)
            {
                return;
            }
            
            block.Deactivate();
            
            var type = block.Type;
            if (_pools.TryGetValue(type, out var pool))
            {
                pool.Push(block);
            }
        }
        
        public void Clear()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var block = pool.Pop();
                    if (block != null)
                    {
                        Object.Destroy(block.gameObject);
                    }
                }
            }
            _pools.Clear();
            _prefabs.Clear();
        }
    }
}
