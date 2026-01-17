using System.Collections.Generic;
using UnityEngine;

namespace Core.Background
{
    public interface IPoolItem
    {
        void Activate();
        void Deactivate();
    }
    
    public class BalloonPool
    {
        private readonly Balloon _prefab;
        private readonly Transform _parent;
        private readonly Stack<Balloon> _pool = new();
        
        public BalloonPool(Balloon prefab, Transform parent, int initialSize = 3)
        {
            _prefab = prefab;
            _parent = parent;
            
            for (int i = 0; i < initialSize; i++)
            {
                var balloon = CreateNew();
                balloon.Deactivate();
                _pool.Push(balloon);
            }
        }
        
        public Balloon Get()
        {
            Balloon balloon = _pool.Count > 0 ? _pool.Pop() : CreateNew();
            balloon.Activate();
            return balloon;
        }
        
        public void Return(Balloon balloon)
        {
            balloon.Deactivate();
            _pool.Push(balloon);
        }
        
        private Balloon CreateNew()
        {
            return Object.Instantiate(_prefab, _parent);
        }
    }
}
