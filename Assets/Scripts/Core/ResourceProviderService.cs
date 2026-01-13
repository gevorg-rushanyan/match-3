using Configs;
using UnityEngine;

namespace Core
{
    public class ResourceProviderService
    {
        private const string LevelsConfigPath = "Levels/LevelsConfig";
        
        private LevelsConfig _levelsConfig;
        
        public LevelsConfig GetLevelsConfig()
        {
            if (_levelsConfig != null)
            {
                return _levelsConfig;
            }

            var result = Resources.Load<LevelsConfig>(LevelsConfigPath);
            if (result == null)
            {
                Debug.LogError("Load levels config failed");
                return null;
            }
                
            _levelsConfig = result;

            return _levelsConfig;
        }
    }
}