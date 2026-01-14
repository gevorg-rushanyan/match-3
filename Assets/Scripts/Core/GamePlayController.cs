using System.Collections;
using Configs;
using Core.Board;
using Core.Input;
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
        private Coroutine _gravityCoroutine;
        
        public void Init(LevelsConfig levelsConfig, BoardVisual boardVisual, InputController inputController)
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
            if (!_boardSystem.TryMoveBlock(from, to, direction))
            {
                return;
            }
            
            if (_gravityCoroutine != null)
            {
                StopCoroutine(_gravityCoroutine);
            }
            
            // _gravityCoroutine = StartCoroutine(GravityWithDelay());
            _gravityCoroutine = StartCoroutine(Normalize());
        }
        
        private IEnumerator Normalize()
        {
            yield return new WaitForSeconds(_gravityDelay);

            while (true)
            {
                // 1️⃣ гравитация
                while (_boardSystem.ApplyGravity())
                {
                    yield return new WaitForSeconds(0.2f);
                }

                // 2️⃣ поиск матчей
                var areas = _boardSystem.FindDestroyAreas();
                if (areas.Count == 0)
                {
                    break;
                }

                // 3️⃣ уничтожение
                _boardSystem.DestroyAreas(areas);

                // 4️⃣ пауза перед следующей гравитацией
                yield return new WaitForSeconds(_gravityDelay);
            }
        }
        
        private IEnumerator GravityWithDelay()
        {
            yield return new WaitForSeconds(_gravityDelay);
            
            while (_boardSystem.ApplyGravity())
            {
                yield return null;
            }

            _gravityCoroutine = null;
        }

        public void OnDestroy()
        {
            if (_gravityCoroutine != null)
            {
                StopCoroutine(_gravityCoroutine);
                _gravityCoroutine = null;
            }

            if (_inputController != null)
            {
                _inputController.OnSwipe -= OnSwipe;
                _inputController = null;
            }
        }
    }
}