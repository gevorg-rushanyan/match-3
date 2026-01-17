using Configs;
using Core.Board;
using Core.Persistence;
using UnityEngine;

namespace Core
{
    public readonly struct LevelData
    {
        public readonly BoardModel Model;
        public readonly BoardSystem System;
        
        public LevelData(BoardModel model, BoardSystem system)
        {
            Model = model;
            System = system;
        }
    }
    
    public class LevelLoader
    {
        private readonly LevelsConfig _levelsConfig;
        private readonly int _matchCount;
        
        public LevelLoader(LevelsConfig levelsConfig, int matchCount)
        {
            _levelsConfig = levelsConfig;
            _matchCount = matchCount;
        }
        
        public LevelData LoadLevel(int levelIndex)
        {
            int index = levelIndex % _levelsConfig.Levels.Count;
            var config = _levelsConfig.Levels[index];
            
            var model = BoardModelFactory.CreateFromConfig(config);
            var system = new BoardSystem(model, _matchCount);
            
            return new LevelData(model, system);
        }
        
        public LevelData? LoadFromSave(GameSaveData saveData)
        {
            if (saveData?.board == null)
            {
                Debug.LogError("Invalid Save Data");
                return null;
            }
            
            var boardData = saveData.board;
            var model = BoardModelFactory.CreateFromSave(boardData.width, boardData.height, boardData.blocks);
            var system = new BoardSystem(model, _matchCount);
            
            return new LevelData(model, system);
        }
        
        public bool HasValidSave(GameSaveData saveData)
        {
            return saveData is { board: { blocks: { Count: > 0 } } };
        }
    }
}
