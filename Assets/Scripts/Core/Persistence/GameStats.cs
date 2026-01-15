using System;

namespace Core.Persistence
{
    public class GameStats
    {
        private int _level;
        
        public int Level => _level;
        
        public void SetLevel(int level)
        {
            if (level < 0) return;
            if (_level == level) return;

            _level = level;
            OnLevelChanged?.Invoke(_level);
        }

        public void NextLevel()
        {
            SetLevel(_level + 1);
        }
        
        public event Action<int> OnLevelChanged;
    }
}