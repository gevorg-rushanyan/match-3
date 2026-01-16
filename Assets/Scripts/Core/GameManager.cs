using Configs;
using Core.Board;
using Core.Input;
using Core.Persistence;
using Core.UI;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamePlayController _gamePlayController;
        [SerializeField] private InputController _inputController;
        [SerializeField] private BoardVisual _boardVisual;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private CameraController _cameraController;
        private ResourceProviderService _resourceProviderService;
        private SaveSystem _saveSystem;
        private GameStats _gameStats;
        private CommonConfigs _commonConfigs;
        private LevelsConfig _levelsConfig;
        private UIConfigs _uiConfigs;

        private void Start()
        {
            _resourceProviderService = new ResourceProviderService();
            if (!TryLoadConfigs())
            {
                return;
            }
            
            _cameraController.Init(_gamePlayController, _commonConfigs);
            _gameStats = new GameStats();
            _uiManager.Init(_uiConfigs, _gameStats);
            _boardVisual.Init(_commonConfigs);
            _saveSystem = new SaveSystem();
            _gamePlayController.Init(_levelsConfig, _boardVisual, _inputController, _saveSystem, _uiManager, _gameStats);
            
            _gamePlayController.StartGame();
        }

        private bool TryLoadConfigs()
        {
            _commonConfigs = _resourceProviderService.GetCommonConfigs();
            if (_commonConfigs == null)
            {
                Debug.LogError("Common config load FAILED");
                return false;
            }
            
            _levelsConfig = _resourceProviderService.GetLevelsConfig();
            if (_levelsConfig == null || _levelsConfig.Levels.Count == 0)
            {
                Debug.LogError("Levels config load FAILED");
                return false;
            }
            
            _uiConfigs = _resourceProviderService.GetUIConfigs();
            if (_uiConfigs == null || _uiConfigs.Views.Count == 0)
            {
                Debug.LogError("UI configs load FAILED");
                return false;
            }
            
            return true;
        }
    }
}