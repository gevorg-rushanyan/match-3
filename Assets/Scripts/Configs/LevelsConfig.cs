using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Levels", menuName = "Configs/Levels")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> _levels = new List<LevelConfig>();
        
        public IReadOnlyList<LevelConfig> Levels => _levels;
    }
}