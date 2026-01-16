using Configs;
using UnityEngine;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
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

            float aspect = (float)Screen.width / Screen.height;
            float multiplier;
            
            // TODO magic numbers, must be improved
            // For big aspect (mainly for tablets) using another multiplayer
            if (aspect < 0.6f)
            {
                multiplier = 1.4f;
            }
            else
            {
                multiplier = 1.67f;
            }
            
            _camera.orthographicSize = Mathf.Max(fieldHeight / multiplier, _commonConfigs.MinOrthographicSize);

            var position = transform.position;
            position.y = fieldHeight / 2f;
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