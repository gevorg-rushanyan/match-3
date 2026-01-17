using System;
using UnityEngine;

namespace Core.Background
{
    public class Balloon : MonoBehaviour, IPoolItem
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private float _speed;
        private float _direction; // 1 = right, -1 = left
        private float _amplitude; // Amplitude of a sinusoid
        private float _frequency; // Sine wave frequency
        private float _baseY; // Base height (center of sine wave)
        private float _phaseOffset; // Phase shift for variety
        private float _passedDistance;
        
        private float _leftBoundary;
        private float _rightBoundary;
        
        public event Action<Balloon> OnOutOfBounds;
        
        public void Setup(
            Sprite sprite,
            float speed,
            float direction,
            float baseY,
            float amplitude,
            float frequency,
            float leftBoundary,
            float rightBoundary)
        {
            _spriteRenderer.sprite = sprite;
            _speed = speed;
            _direction = direction;
            _baseY = baseY;
            _amplitude = amplitude;
            _frequency = frequency;
            _leftBoundary = leftBoundary;
            _rightBoundary = rightBoundary;
            _phaseOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            _passedDistance = 0f;
            
            float startX = direction > 0 ? leftBoundary - 1f : rightBoundary + 1f;
            transform.position = new Vector3(startX, baseY, transform.position.z);
        }
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            _passedDistance = 0f;
        }
        
        private void Update()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            // Horizontal movement
            float deltaX = _speed * _direction * Time.deltaTime;
            _passedDistance += Mathf.Abs(deltaX);
            
            // Sinusoidal Y-shift
            float sinOffset = Mathf.Sin(_passedDistance * _frequency + _phaseOffset) * _amplitude;
            
            Vector3 pos = transform.position;
            pos.x += deltaX;
            pos.y = _baseY + sinOffset;
            transform.position = pos;
            
            // Checking for out-of-bounds
            if ((_direction > 0 && pos.x > _rightBoundary + 1f) ||
                (_direction < 0 && pos.x < _leftBoundary - 1f))
            {
                OnOutOfBounds?.Invoke(this);
            }
        }
    }
}
