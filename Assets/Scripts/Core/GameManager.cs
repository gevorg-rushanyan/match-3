using Core.Board;
using Core.Input;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamePlayController _gamePlayController;
        [SerializeField] private InputController _inputController;
        [SerializeField] private BoardVisual _boardVisual;
        private ResourceProviderService _resourceProviderService;
        
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
            _gamePlayController.Init(levelsConfig, _boardVisual, _inputController);
            _gamePlayController.StartGame();
        }
    }
}