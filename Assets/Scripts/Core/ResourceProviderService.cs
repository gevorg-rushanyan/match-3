using Configs;
using UnityEngine;

namespace Core
{
    public class ResourceProviderService
    {
        private const string LevelsConfigPath = "Configs/LevelsConfig";
        private const string CommonConfigPath = "Configs/CommonConfigs";
        
        private LevelsConfig _levelsConfig;
        private CommonConfigs _commonConfig;
        
        public LevelsConfig GetLevelsConfig()
        {
            if (_levelsConfig != null)
            {
                return _levelsConfig;
            }

            var result = Resources.Load<LevelsConfig>(LevelsConfigPath);
            if (result == null)
            {
                Debug.LogError("Levels config load FAILED");
                return null;
            }
                
            _levelsConfig = result;

            return _levelsConfig;
        }

        public CommonConfigs GetCommonConfigs()
        {
            if (_commonConfig != null)
            {
                return _commonConfig;
            }

            var result = Resources.Load<CommonConfigs>(CommonConfigPath);
            if (result == null)
            {
                Debug.LogError("Common configs load FAILED");
                return null;
            }
                
            _commonConfig = result;

            return _commonConfig;
        }
    }
}