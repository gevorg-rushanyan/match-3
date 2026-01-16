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
            FitCameraToField(size.x, size.y, blockSize.x, blockSize.y);
        }

        private void FitCameraToField(int width, int height, float blockWidth, float blockHeight)
        {
            float fieldWidth  = width * blockWidth;
            float fieldHeight = height * blockHeight;

            float aspect = (float)Screen.width / Screen.height;
            
            float maxSize = Mathf.Max(fieldWidth, fieldHeight) / (2f * aspect);

            // float sizeByHeight = fieldHeight / 2f;
            // float sizeByWidth  = fieldWidth / (2f * aspect);
            _camera.orthographicSize = maxSize;

            var position = transform.position;
            position.y = fieldHeight / 2f;
            transform.position = position;
        }

        public Vector2Int size1;
        [ContextMenu("TestSiz")]
        public void Test()
        {
            OnBoardSizeChanged(size1);
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