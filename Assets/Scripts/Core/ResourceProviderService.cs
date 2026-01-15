using Configs;
using UnityEngine;

namespace Core
{
    public class ResourceProviderService
    {
        private const string LevelsConfigPath = "Configs/LevelsConfig";
        private const string CommonConfigPath = "Configs/CommonConfigs";
        private const string UIConfigsPath = "Configs/UIConfigs";
        
        private LevelsConfig _levelsConfig;
        private CommonConfigs _commonConfig;
        private UIConfigs _uiConfigs;
        
        public LevelsConfig GetLevelsConfig()
        {
            if (_levelsConfig != null)
            {
                return _levelsConfig;
            }

            var result = Resources.Load<LevelsConfig>(LevelsConfigPath);
            if (result == null)
            {
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
                return null;
            }
                
            _commonConfig = result;

            return _commonConfig;
        }

        public UIConfigs GetUIConfigs()
        {
            if (_uiConfigs != null)
            {
                return _uiConfigs;
            }
            
            var result = Resources.Load<UIConfigs>(UIConfigsPath);
            if (result == null)
            {
                Debug.LogError("UIConfigs load FAILED");
                return null;
            }
                
            _uiConfigs = result;

            return _uiConfigs;
        }
    }
}