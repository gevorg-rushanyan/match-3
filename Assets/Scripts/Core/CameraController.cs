using Configs;
using UnityEngine;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
        [Space(5),Header("Configurations")]
        [SerializeField] private float _aspectRatioLimiter = 0.6f;
        [SerializeField] private float _minOrthographicSizeForBigAspectRatio = 5;
        [SerializeField] private float _minOrthographicSizeForSmallAspectRatio = 4.5f;
        
        private GamePlayController _gamePlayController;
        private CommonConfigs _commonConfigs;

        public void Init(GamePlayController gamePlayController, CommonConfigs commonConfigs)
        {
            _gamePlayController = gamePlayController;
            _commonConfigs = commonConfigs;
            _gamePlayController.BoardSizeChanged += OnBoardSizeChanged;
        }

        private void OnBoardSizeChanged(Vector2Int size)
        {
            var blockSize = _commonConfigs.BlockSize;
            FitCameraToBoard(size, blockSize.x, blockSize.y);
        }

        private void FitCameraToBoard(Vector2Int size, float blockWidth, float blockHeight)
        {
            float fieldWidth  = size.x * blockWidth;
            float fieldHeight = size.y * blockHeight;
            float maxSize = Mathf.Max(fieldWidth, fieldHeight);

            float aspect = (float)Screen.width / Screen.height;
            float multiplier;
            float minOrthographicSize;
            
            // TODO magic numbers, must be improved
            // For big aspect (mainly for tablets) using another multiplayer
            if (aspect < _aspectRatioLimiter)
            {
                multiplier = 1.4f;
                minOrthographicSize = _minOrthographicSizeForSmallAspectRatio;
            }
            else
            {
                multiplier = 1.67f;
                minOrthographicSize = _minOrthographicSizeForBigAspectRatio;
            }
            
            _camera.orthographicSize = Mathf.Max(maxSize / multiplier, minOrthographicSize);

            var position = transform.position;
            position.y = maxSize / 2f;
            transform.position = position;
        }

        private void OnDestroy()
        {
            if (_gamePlayController != null)
            {
                _gamePlayController.BoardSizeChanged -= OnBoardSizeChanged;
            }
        }
    }
}