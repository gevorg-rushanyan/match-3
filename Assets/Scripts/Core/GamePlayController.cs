using System;
using System.Collections;
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
        [SerializeField] private float _winViewDuration;
        // Dependencies
        private LevelsConfig _levelsConfig;
        private BoardVisual _boardVisual;
        private InputController _inputController;
        private SaveSystem _saveSystem;
        private UIManager _uiManager;
        private GameStats _gameStats;
        private CameraController _cameraController;
        
        private BoardModel _boardModel;
        private BoardSystem _boardSystem;
        private Coroutine _normalizeCoroutine;
        private Coroutine _nextLevelCoroutine;
        private bool _isBoardChanged;
        private GameSaveData _gameSaveData;
        
        public event Action<Vector2Int> BoardSizeChanged; 
        
        public void Init(
            LevelsConfig levelsConfig,
            BoardVisual boardVisual,
            InputController inputController,
            SaveSystem saveSystem,
            UIManager uiManager,
            GameStats gameStats)
        {
            _levelsConfig = levelsConfig;
            _boardVisual = boardVisual;
            _inputController = inputController;
            _saveSystem = saveSystem;
            _uiManager = uiManager;
            _gameStats = gameStats;
            
            if (inputController != null)
            {
                _inputController.OnSwipe += OnSwipe;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected += OnPlaySelected;
                _uiManager.NextLevelSelected += OnNextLevelSelected;
                _uiManager.RestartSelected += OnRestartSelected;
            }
        }

        public void StartGame()
        {
            _gameSaveData = _saveSystem.Load();
            _gameStats.SetLevel(_gameSaveData != null ? _gameSaveData.currentLevelIndex : 0);
            _uiManager.Show(UIViewType.Main);
        }

        private void OnPlaySelected()
        {
            _uiManager.Show(UIViewType.Gameplay);
            if (_gameSaveData != null)
            {
                LoadFromSave(_gameSaveData);
                _gameSaveData = null;
            }
            else
            {
                StartNewGame(_gameStats.Level);
            }
        }

        private void OnNextLevelSelected()
        {
            StopCoroutines();
            _gameStats.NextLevel();
            StartNewGame(_gameStats.Level);
        }

        private void OnRestartSelected()
        {
            StopCoroutines();
            StartNewGame(_gameStats.Level);
        }

        private void StartNewGame(int levelIndex)
        {
            if (levelIndex >= _levelsConfig.Levels.Count)
            {
                levelIndex %= _levelsConfig.Levels.Count;
            }
            var config = _levelsConfig.Levels[levelIndex];
            _boardModel = BoardModelFactory.CreateFromConfig(config);
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(_boardModel);
            _isBoardChanged = true;
            TriggerBordSizeChangedEvent();
        }
        
        private void LoadFromSave(GameSaveData save)
        {
            var boardData = save.board;
            if (boardData == null)
            {
                StartNewGame(_gameStats.Level);
                return;
            }
            
            _boardModel = BoardModelFactory.CreateFromSave(boardData.width, boardData.height, boardData.blocks);
            _boardSystem = new BoardSystem(_boardModel, _boardVisual);
            _boardVisual.CreateBoard(_boardModel);
            TriggerBordSizeChangedEvent();
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
            _gameStats.NextLevel();
            _isBoardChanged = true;
            _nextLevelCoroutine = StartCoroutine(StartNextLevel());
        }

        private IEnumerator StartNextLevel()
        {
            _uiManager.Show(UIViewType.Win, false);
            yield return new WaitForSeconds(_winViewDuration);
            _uiManager.CloseView(UIViewType.Win);
            StartNewGame(_gameStats.Level);
            _nextLevelCoroutine = null;
        }

        private bool TrySaveProgress()
        {
            if (!_isBoardChanged)
            {
                return false;
            }
            
            var boardSaveData = _boardModel.ToSaveData();
            var gameSaveData = new GameSaveData(_gameStats.Level, boardSaveData);
            _saveSystem.Save(gameSaveData);
            _isBoardChanged = false;

            return true;
        }

        private void TriggerBordSizeChangedEvent()
        {
            BoardSizeChanged?.Invoke(new Vector2Int(_boardModel.Width, _boardModel.Height));
        }

        private void StopCoroutines()
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

        private void OnDestroy()
        {
            StopCoroutines();

            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
                _inputController = null;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected -= OnPlaySelected;
                _uiManager.NextLevelSelected -= OnNextLevelSelected;
                _uiManager.RestartSelected -= OnRestartSelected;
                _uiManager = null;
            }
        }
    }
}