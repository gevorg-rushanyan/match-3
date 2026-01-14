using System;
using Configs;
using Core.Board;
using Core.Input;
using UnityEngine;

namespace Core
{
    public class GamePlayController : IDisposable
    {
        private readonly LevelsConfig _levelsConfig;
        private readonly BoardVisual _boardVisual;
        private readonly InputController _inputController;
        private BoardModel _boardModel;
        private BoardSystem _boardSystem;
        
        public GamePlayController(LevelsConfig levelsConfig, BoardVisual boardVisual, InputController inputController)
        {
            _levelsConfig = levelsConfig;
            _boardVisual = boardVisual;
            _inputController = inputController;
            if (inputController != null)
            {
                _inputController.OnSwipe += OnSwipe;
            }
        }

        public void StartGame()
        {
            var level = _levelsConfig.Levels[0];
            _boardModel = new BoardModel(level.Width, level.Height);
            
            foreach (var block in level.Blocks)
            {
                var data = new BlockData(block.Type);
                _boardModel.Set(block.Position.x, block.Position.y, data);
            }
            
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(level);
        }

        private void OnSwipe(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (_boardSystem.TryMoveBlock(from, to, direction))
            {
                while (_boardSystem.ApplyGravity())
                {
                    // позже здесь будет поиск матчей
                }
            }
        }

        public void Dispose()
        {
            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
            }
        }
    }
}