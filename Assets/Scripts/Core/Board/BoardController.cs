using System;
using System.Collections;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BoardController : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] private float _gravityDelay;
        
        private BoardSystem _boardSystem;
        private Coroutine _normalizeCoroutine;
        private WaitForSeconds _gravityWait;
        
        public event Action OnBoardChanged;
        public event Action OnLevelCompleted;
        
        private void Awake()
        {
            _gravityWait = new WaitForSeconds(_gravityDelay);
        }
        
        public void SetBoardSystem(BoardSystem boardSystem)
        {
            _boardSystem = boardSystem;
        }
        
        public void TryMove(Vector2Int from, Vector2Int to, Vector2Int direction)
        {
            if (_boardSystem == null)
            {
                return;
            }
            
            var moveType = _boardSystem.TryMoveBlock(from, to, direction);
            if (moveType == MoveType.None)
            {
                return;
            }
            
            OnBoardChanged?.Invoke();
            StopNormalize();
            _normalizeCoroutine = StartCoroutine(Normalize(moveType));
        }
        
        public void StopNormalize()
        {
            if (_normalizeCoroutine != null)
            {
                StopCoroutine(_normalizeCoroutine);
                _normalizeCoroutine = null;
            }
        }
        
        private IEnumerator Normalize(MoveType moveType)
        {
            // If block moved to empty space, add delay before gravity
            if (moveType == MoveType.Move)
            {
                yield return _gravityWait;
            }

            while (true)
            {
                if (_boardSystem.ApplyGravity())
                {
                    OnBoardChanged?.Invoke();
                    yield return _gravityWait;
                }
                
                var matchBlocks = _boardSystem.FindMatches();
                if (matchBlocks.Count == 0)
                {
                    break;
                }
                
                bool destroyed = _boardSystem.DestroyBlocks(matchBlocks);
                OnBoardChanged?.Invoke();
                
                if (destroyed)
                {
                    yield return _gravityWait;
                }
            }
            
            _normalizeCoroutine = null;
            
            if (_boardSystem.IsLevelCompleted())
            {
                OnLevelCompleted?.Invoke();
            }
        }
        
        private void OnDestroy()
        {
            StopNormalize();
        }
    }
}
