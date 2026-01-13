using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardController _boardController;
        private ResourceProviderService _resourceProviderService;

        public ResourceProviderService ResourceProviderService => _resourceProviderService;
        
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
            _boardController.Init(commonConfigs);
            
            var levelsConfig = _resourceProviderService.GetLevelsConfig();
            if (levelsConfig == null)
            {
                return;
            }
            _boardController.CreateBoard(levelsConfig.Levels[0]);
        }
    }
}