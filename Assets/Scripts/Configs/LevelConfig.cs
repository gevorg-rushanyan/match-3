using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Level", menuName = "Configs/Level")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private List<BlockConfig> _blocks = new List<BlockConfig>();
        
        public int Width => _width;
        public int Height => _height;
        public IReadOnlyList<BlockConfig> Blocks => _blocks;
    }
}