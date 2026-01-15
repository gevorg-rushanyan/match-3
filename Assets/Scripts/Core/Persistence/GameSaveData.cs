using System;

namespace Core.Persistence
{
    [Serializable]
    public class GameSaveData
    {
        public int currentLevelIndex;
        public BoardSaveData board;

        public GameSaveData(int currentLevelIndex, BoardSaveData board)
        {
            this.board = board;
            this.currentLevelIndex = currentLevelIndex;
        }
    }
}