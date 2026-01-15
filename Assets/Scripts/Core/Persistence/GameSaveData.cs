using System;

namespace Core.Persistence
{
    [Serializable]
    public class GameSaveData
    {
        public int currentLevelIndex;
        public BoardSaveData board;

        public GameSaveData(BoardSaveData board, int currentLevelIndex)
        {
            this.board = board;
            this.currentLevelIndex = currentLevelIndex;
        }
    }
}