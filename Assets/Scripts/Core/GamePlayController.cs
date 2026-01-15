using System.Collections;
using System.Collections.Generic;
using Configs;
using Core.Board;
using Core.Input;
using Core.Persistence;
using UnityEngine;

namespace Core
{
    public class GamePlayController : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] private float _gravityDelay;
        private LevelsConfig _levelsConfig;
        private BoardVisual _boardVisual;
        private InputController _inputController;
        private BoardModel _boardModel;
        private BoardSystem _boardSystem;
        private Coroutine _normalizeCoroutine;
        private SaveSystem _saveSystem;
        private bool _isBordChanged;
        private int _level;
        
        public void Init(LevelsConfig levelsConfig, BoardVisual boardVisual, InputController inputController, SaveSystem saveSystem)
        {
            _levelsConfig = levelsConfig;
            _boardVisual = boardVisual;
            _inputController = inputController;
            _saveSystem = saveSystem;
            if (inputController != null)
            {
                _inputController.OnSwipe += OnSwipe;
            }
        }

        public void StartGame()
        {
            var save = _saveSystem.Load();

            if (save != null)
            {
                _level = save.currentLevelIndex;
                LoadFromSave(save);
            }
            else
            {
                var level = _levelsConfig.Levels[0];
                _boardModel = new BoardModel(level.Width, level.Height);
                foreach (var block in level.Blocks)
                {
                    var data = new BlockData(block.Type);
                    _boardModel.Set(block.Position.x, block.Position.y, data);
                }
                _boardSystem = new BoardSystem(_boardModel, _boardVisual);
                _boardVisual.CreateBoard(_boardModel);
                _isBordChanged = true;
            }
        }

        private void LoadFromSave(GameSaveData save)
        {
            var boardData = save.board;
            _boardModel = CreateBoardModelFromSave(boardData.width, boardData.height, boardData.blocks);
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(_boardModel);
        }

        private BoardModel CreateBoardModelFromSave(int width, int height, IReadOnlyList<BlockSaveData> blocks)
        {
            var boardModel = new BoardModel(width, height);
            foreach (var block in blocks)
            {
                var data = new BlockData(block.type);
                boardModel.Set(block.x, block.y, data);
            }
            
            return boardModel;
        }

        private void OnSwipe(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (!_boardSystem.TryMoveBlock(from, to, direction))
            {
                _isBordChanged = true;
                return;
            }
            
            _isBordChanged = true;
            if (_normalizeCoroutine != null)
            {
                StopCoroutine(_normalizeCoroutine);
            }
            
            _normalizeCoroutine = StartCoroutine(Normalize());
        }
        
        private IEnumerator Normalize()
        {
            yield return new WaitForSeconds(_gravityDelay);

            while (true)
            {
                while (_boardSystem.ApplyGravity())
                {
                    _isBordChanged = true;
                    yield return new WaitForSeconds(_gravityDelay);
                }
                
                var matchBlocks = _boardSystem.FindMatches();
                if (matchBlocks.Count == 0)
                {
                    break;
                }
                
                _boardSystem.DestroyBlocks(matchBlocks);
                _isBordChanged = true;
                yield return new WaitForSeconds(_gravityDelay);
            }
            
        }
        
        private bool TrySaveProgress()
        {
            if (!_isBordChanged)
            {
                return false;
            }

            var boardSaveData = _boardModel.ToSaveData();
            var gameSaveData = new GameSaveData(boardSaveData, _level);
            _saveSystem.Save(gameSaveData);
            _isBordChanged = false;

            return true;
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                TrySaveProgress();
            }
        }

        private void OnApplicationQuit()
        {
            TrySaveProgress();
        }

        public void OnDestroy()
        {
            if (_normalizeCoroutine != null)
            {
                StopCoroutine(_normalizeCoroutine);
                _normalizeCoroutine = null;
            }

            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
                _inputController = null;
            }
        }
    }
}