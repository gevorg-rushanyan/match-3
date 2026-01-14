using Core.Board;
using Core.Input;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private BoardVisual _boardVisual;
        private ResourceProviderService _resourceProviderService;
        private GamePlayController _gamePlayController;
        
        private void Awake()
        {
            _resourceProviderService = new ResourceProviderService();
        }

        private void Start()
        {
            var commonConfigs = _resourceProviderService.GetCommonConfigs();
            if (commonConfigs == null)
            {
                return;
            }
            
            var levelsConfig = _resourceProviderService.GetLevelsConfig();
            if (levelsConfig == null || levelsConfig.Levels.Count == 0)
            {
                return;
            }
            
            _boardVisual.Init(commonConfigs);
            _gamePlayController = new GamePlayController(levelsConfig, _boardVisual, _inputController);
            _gamePlayController.StartGame();
        }

        private void OnDestroy()
        {
            if (_gamePlayController != null)
            {
                _gamePlayController.Dispose();
                _gamePlayController = null;
            }
        }
    }
}