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
        [SerializeField] private float _winViewDuration = 2f;
        [SerializeField] private BoardController _boardController;
        
        // Dependencies
        private BoardVisual _boardVisual;
        private InputController _inputController;
        private SaveSystem _saveSystem;
        private UIManager _uiManager;
        private GameStats _gameStats;
        
        // Level data
        private LevelLoader _levelLoader;
        private BoardModel _boardModel;
        private BoardSystem _boardSystem;
        
        // State
        private GameSaveData _gameSaveData;
        private GameState _currentState = GameState.None;
        private Coroutine _nextLevelCoroutine;
        private bool _boardChanged;
        
        public event Action<Vector2Int> BoardSizeChanged;
        
        public GameState CurrentState => _currentState;

        public void Init(
            LevelsConfig levelsConfig,
            BoardVisual boardVisual,
            InputController inputController,
            SaveSystem saveSystem,
            UIManager uiManager,
            GameStats gameStats,
            int matchCount)
        {
            _boardVisual = boardVisual;
            _inputController = inputController;
            _saveSystem = saveSystem;
            _uiManager = uiManager;
            _gameStats = gameStats;
            
            _levelLoader = new LevelLoader(levelsConfig, matchCount);
            
            SubscribeToEvents();
        }

        public void StartGame()
        {
            _gameSaveData = _saveSystem.Load();
            _gameStats.SetLevel(_gameSaveData?.currentLevelIndex ?? 0);
            SetState(GameState.Menu);
        }
        
        private void SetState(GameState newState)
        {
            if (_currentState == newState)
            {
                return;
            }
            
            _currentState = newState;
            
            switch (newState)
            {
                case GameState.Menu:
                    _uiManager.Show(UIViewType.Main);
                    break;
                    
                case GameState.Playing:
                    _uiManager.Show(UIViewType.Gameplay);
                    break;
                    
                case GameState.LevelComplete:
                    StartLevelCompleteSequence();
                    break;
            }
        }

        private void OnPlaySelected()
        {
            if (_levelLoader.HasValidSave(_gameSaveData))
            {
                var levelData = _levelLoader.LoadFromSave(_gameSaveData);
                if (levelData.HasValue)
                {
                    _boardModel = levelData.Value.Model;
                    _boardSystem = levelData.Value.System;
                }
                _gameSaveData = null;
            }
            else
            {
                LoadLevel(_gameStats.Level);
            }
            
            InitializeBoard();
            SetState(GameState.Playing);
        }

        private void OnNextLevelSelected()
        {
            StopCoroutines();
            _gameStats.NextLevel();
            LoadLevel(_gameStats.Level);
            InitializeBoard();
        }

        private void OnRestartSelected()
        {
            StopCoroutines();
            LoadLevel(_gameStats.Level);
            InitializeBoard();
        }
        
        private void LoadLevel(int levelIndex)
        {
            var levelData = _levelLoader.LoadLevel(levelIndex);
            _boardModel = levelData.Model;
            _boardSystem = levelData.System;
        }
        
        private void InitializeBoard()
        {
            _boardVisual.CreateBoard(_boardModel, _boardSystem);
            _boardController.SetBoardSystem(_boardSystem);
            _boardChanged = true;
            BoardSizeChanged?.Invoke(new Vector2Int(_boardModel.Width, _boardModel.Height));
        }

        private void OnSwipe(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (_currentState != GameState.Playing)
            {
                return;
            }

            _boardController.TryMove(from, to, direction);
        }
        
        private void OnBoardChanged()
        {
            _boardChanged = true;
        }
        
        private void OnLevelCompleted()
        {
            Debug.Log("LEVEL COMPLETED!");
            _gameStats.NextLevel();
            _boardChanged = true;
            SetState(GameState.LevelComplete);
        }

        private void StartLevelCompleteSequence()
        {
            if (_nextLevelCoroutine != null)
            {
                StopCoroutine(_nextLevelCoroutine);
            }
            _nextLevelCoroutine = StartCoroutine(LevelCompleteSequence());
        }

        private IEnumerator LevelCompleteSequence()
        {
            _uiManager.Show(UIViewType.Win, false);
            yield return new WaitForSeconds(_winViewDuration);
            _uiManager.CloseView(UIViewType.Win);
            
            LoadLevel(_gameStats.Level);
            InitializeBoard();
            SetState(GameState.Playing);
            
            _nextLevelCoroutine = null;
        }

        private bool TrySaveProgress()
        {
            if (!_boardChanged || _boardModel == null)
            {
                return false;
            }
            
            var boardSaveData = _boardModel.ToSaveData();
            var gameSaveData = new GameSaveData(_gameStats.Level, boardSaveData);
            _saveSystem.Save(gameSaveData);
            _boardChanged = false;

            return true;
        }

        private void StopCoroutines()
        {
            _boardController.StopNormalize();
            
            if (_nextLevelCoroutine != null)
            {
                StopCoroutine(_nextLevelCoroutine);
                _nextLevelCoroutine = null;
            }
        }
        
        private void SubscribeToEvents()
        {
            if (_inputController != null)
            {
                _inputController.OnSwipe += OnSwipe;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected += OnPlaySelected;
                _uiManager.NextLevelSelected += OnNextLevelSelected;
                _uiManager.RestartSelected += OnRestartSelected;
            }
            
            if (_boardController != null)
            {
                _boardController.OnBoardChanged += OnBoardChanged;
                _boardController.OnLevelCompleted += OnLevelCompleted;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
            }
            
            if (_uiManager != null)
            {
                _uiManager.PlaySelected -= OnPlaySelected;
                _uiManager.NextLevelSelected -= OnNextLevelSelected;
                _uiManager.RestartSelected -= OnRestartSelected;
            }
            
            if (_boardController != null)
            {
                _boardController.OnBoardChanged -= OnBoardChanged;
                _boardController.OnLevelCompleted -= OnLevelCompleted;
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
            UnsubscribeFromEvents();
        }
    }
}