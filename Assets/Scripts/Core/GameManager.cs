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
            var levelsConfig = _resourceProviderService.GetLevelsConfig();
            if (levelsConfig != null)
            {
                _boardController.CreateBoard(levelsConfig.Levels[0]);
            }
        }
    }
}