using Core.Board;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardVisual _boardVisual;
        private ResourceProviderService _resourceProviderService;
        private BoardModel _boardModel;
        private BoardSystem _boardSystem;

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
            _boardVisual.Init(commonConfigs);
            
            var levelsConfig = _resourceProviderService.GetLevelsConfig();
            if (levelsConfig == null || levelsConfig.Levels.Count == 0)
            {
                return;
            }
            
            var level = levelsConfig.Levels[0];
            
            _boardModel = new BoardModel(level.Width, level.Height);
            foreach (var block in level.Blocks)
            {
                var data = new BlockData(block.Type);
                _boardModel.Set(block.Position.x, block.Position.y, data);
            }
            
            _boardVisual.CreateBoard(level);
        }
    }
}