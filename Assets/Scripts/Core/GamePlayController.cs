using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using Core.Board;
using Core.Input;
using Core.Persistence;
using Core.UI;
using Enums;
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
        private Coroutine _nextLevelCoroutine;
        private SaveSystem _saveSystem;
        private UIManager _uiManager;
        private bool _isBoardChanged;
        private int _level;
        
        public void Init(LevelsConfig levelsConfig, BoardVisual boardVisual, InputController inputController, SaveSystem saveSystem, UIManager uiManager)
        {
            _levelsConfig = levelsConfig;
            _boardVisual = boardVisual;
            _inputController = inputController;
            _saveSystem = saveSystem;
            _uiManager = uiManager;
            
            if (inputController != null)
            {
                _inputController.OnSwipe += OnSwipe;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected += OnPlaySelected;
            }
            
            _uiManager.Show(UIViewType.Main);
        }

        private void StartGame()
        {
            var save = _saveSystem.Load();
            
            
            
            
            if (save != null)
            {
                _level = save.currentLevelIndex;
                LoadFromSave(save);
            }
            else
            {
                _level = 0;
                StartNewGame(_level);
            }
        }

        private void LoadLevel()
        {
        }

        private void OnPlaySelected()
        {
            StartGame();
        }

        private void StartNewGame(int levelIndex)
        {
            if (levelIndex > _levelsConfig.Levels.Count - 1)
            {
                levelIndex = levelIndex / _levelsConfig.Levels.Count;
            }
            var config = _levelsConfig.Levels[levelIndex];
            _boardModel = BoardModelFactory.CreateFromConfig(config);
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(_boardModel);
            _isBoardChanged = true;
        }
        
        private void LoadFromSave(GameSaveData save)
        {
            var boardData = save.board;
            if (boardData == null)
            {
                StartNewGame(_level);
                return;
            }
            
            _boardModel = BoardModelFactory.CreateFromSave(boardData.width, boardData.height, boardData.blocks);
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(_boardModel);
        }

        private void OnSwipe(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (!_boardSystem.TryMoveBlock(from, to, direction))
            {
                return;
            }
            
            _isBoardChanged = true;
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
                    _isBoardChanged = true;
                    yield return new WaitForSeconds(_gravityDelay);
                }
                
                var matchBlocks = _boardSystem.FindMatches();
                if (matchBlocks.Count == 0)
                {
                    break;
                }
                
                _boardSystem.DestroyBlocks(matchBlocks);
                _isBoardChanged = true;
                yield return new WaitForSeconds(_gravityDelay);
            }
            
            if (_boardSystem.IsLevelCompleted())
            {
                OnLevelCompleted();
            }
        }

        private void OnLevelCompleted()
        {
            Debug.Log("LEVEL COMPLETED!");
            _level++;
            _isBoardChanged = true;
            TrySaveProgress();
            _nextLevelCoroutine = StartCoroutine(StartNextLevel());
        }

        private IEnumerator StartNextLevel()
        {
            yield return new WaitForSeconds(1);
            StartNewGame(_level);
            _nextLevelCoroutine = null;
        }

        private bool TrySaveProgress()
        {
            if (!_isBoardChanged)
            {
                return false;
            }
            
            var boardSaveData = _boardModel.ToSaveData();
            var gameSaveData = new GameSaveData(_level, boardSaveData);
            _saveSystem.Save(gameSaveData);
            _isBoardChanged = false;

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

            if (_nextLevelCoroutine != null)
            {
                StopCoroutine(_nextLevelCoroutine);
                _nextLevelCoroutine = null;
            }

            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
                _inputController = null;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected -= OnPlaySelected;
                _uiManager = null;
            }
        }
    }
}