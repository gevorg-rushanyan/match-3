using Configs;
using UnityEngine;

namespace Core.Background
{
    public class BalloonsController : MonoBehaviour
    {
        [SerializeField] private Balloon _balloonPrefab;
        [SerializeField] private Sprite[] _balloonSprites;
        [SerializeField] private int _maxBalloons = 3;
        [SerializeField] private float _minSpawnInterval = 2f;
        [SerializeField] private float _maxSpawnInterval = 5f;
        [SerializeField] private float _minSpeed = 0.5f;
        [SerializeField] private float _maxSpeed = 2f;
        [SerializeField] private float _minAmplitude = 0.3f;
        [SerializeField] private float _maxAmplitude = 1f;
        [SerializeField] private float _minFrequency = 1f;
        [SerializeField] private float _maxFrequency = 3f;
        
        private GamePlayController _gamePlayController;
        private CommonConfigs _commonConfigs;
        
        private BalloonPool _balloonPool;
        private int _activeBalloons;
        private float _spawnTimer;
        private float _nextSpawnTime;
        private float _leftBoundary;
        private float _rightBoundary;
        private float _bottomBoundary;
        private float _topBoundary;
        private bool _isInitialized;
        
        public void Init(GamePlayController gamePlayController, CommonConfigs commonConfigs)
        {
            _gamePlayController = gamePlayController;
            _commonConfigs = commonConfigs;
            _gamePlayController.BoardSizeChanged += OnBoardSizeChanged;
            
            if (_balloonPrefab != null)
            {
                _balloonPool = new BalloonPool(_balloonPrefab, transform);
            }
            
            _nextSpawnTime = _minSpawnInterval;
            _isInitialized = true;
        }
        
        private void OnBoardSizeChanged(Vector2Int size)
        {
            UpdateBoundaries(size);
        }
        
        private void UpdateBoundaries(Vector2Int size)
        {
            float halfWidth = (size.x * _commonConfigs.BlockSize.x) / 2f;
            float height = size.y * _commonConfigs.BlockSize.y;
            
            _leftBoundary = -halfWidth;
            _rightBoundary = halfWidth;
            _bottomBoundary = height / 1.5f;
            _topBoundary = height;
        }
        
        private void Update()
        {
            if (!_isInitialized || _balloonPool == null)
            {
                return;
            }

            if (_activeBalloons >= _maxBalloons)
            {
                return;
            }

            if (_balloonSprites == null || _balloonSprites.Length == 0)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _nextSpawnTime)
            {
                SpawnBalloon();
                _spawnTimer = 0f;
                _nextSpawnTime = Random.Range(_minSpawnInterval, _maxSpawnInterval);
            }
        }
        
        private void SpawnBalloon()
        {
            Balloon balloon = _balloonPool.Get();
            
            Sprite sprite = _balloonSprites[Random.Range(0, _balloonSprites.Length)];
            float speed = Random.Range(_minSpeed, _maxSpeed);
            float direction = Random.value > 0.5f ? 1f : -1f;
            float baseY = Random.Range(_bottomBoundary + 1f, _topBoundary - 1f);
            float amplitude = Random.Range(_minAmplitude, _maxAmplitude);
            float frequency = Random.Range(_minFrequency, _maxFrequency);
            
            balloon.Setup(sprite, speed, direction, baseY, amplitude, frequency, _leftBoundary, _rightBoundary);
            
            balloon.OnOutOfBounds += OnBalloonOutOfBounds;
            _activeBalloons++;
        }
        
        private void OnBalloonOutOfBounds(Balloon balloon)
        {
            balloon.OnOutOfBounds -= OnBalloonOutOfBounds;
            _balloonPool.Return(balloon);
            _activeBalloons--;
        }
        
        private void OnDestroy()
        {
            if (_gamePlayController != null)
            {
                _gamePlayController.BoardSizeChanged -= OnBoardSizeChanged;
                _gamePlayController = null;
            }
        }
    }
}