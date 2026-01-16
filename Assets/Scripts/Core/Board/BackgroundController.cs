using Configs;
using UnityEngine;

namespace Core.Board
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private Transform _background;
        [Space(5)]
        [Header("These dimensions correspond to the current background dimensions. \n If the matrix will large, based on these dimensions will calculate a new scale.")]
        [SerializeField] private Vector2 _defaultBackgroundSize;
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
            FitToBoard(size, _commonConfigs.BlockSize.x, _commonConfigs.BlockSize.y);
        }

        private void FitToBoard(Vector2Int size, float blockWidth, float blockHeight)
        {
            float width = size.x * blockWidth;
            float height = size.y * blockHeight;
            
            float widthMultiplier = width / _defaultBackgroundSize.x;
            float heightMultiplier = height / _defaultBackgroundSize.y;
            
            float multiplier = Mathf.Max(widthMultiplier, heightMultiplier);
            
            _background.localScale = multiplier <= 1? Vector3.one: Vector3.one * multiplier;
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