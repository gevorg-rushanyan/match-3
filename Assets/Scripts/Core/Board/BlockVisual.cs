using System;
using System.Collections;
using Enums;
using UnityEngine;

namespace Core.Board
{
    public class BlockVisual : MonoBehaviour
    {
        private const string DestroyTrigger = "destroy";
        [SerializeField] private Animator _animator;
        [SerializeField] private float _moveDuration = 0.15f;
        [SerializeField] private float _destroyDuration = 0.15f;
        
        private BlockVisualState _state;
        private BlockType _type;
        private Vector2Int _gridPosition;
        private Coroutine _moveCoroutine;
        private Coroutine _destroyCoroutine;
        private WaitForSeconds _destroyWait;
        
        public BlockVisualState State => _state;
        public BlockType Type => _type;
        public Vector2Int GridPosition => _gridPosition;
        
        public event Action AnimationStarted;
        public event Action AnimationFinished;
        
        private void Awake()
        {
            _destroyWait = new WaitForSeconds(_destroyDuration);
        }
        
        public void Init(BlockType type, Vector2Int gridPosition, Vector3 worldPosition)
        {
            _type = type;
            _gridPosition = gridPosition;
            transform.localPosition = worldPosition;
            
            ResetState();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            StopAllAnimations();
            ClearEvents();
            gameObject.SetActive(false);
        }

        public void SetGridPosition(Vector2Int pos)
        {
            _gridPosition = pos;
        }

        public void MoveTo(Vector3 worldPos)
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                AnimationFinished?.Invoke();
            }
            _state = BlockVisualState.Moving;
            AnimationStarted?.Invoke();
            _moveCoroutine = StartCoroutine(MoveCoroutine(worldPos));
        }

        public void PlayDestroyAnimation(Action callback = null)
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                AnimationFinished?.Invoke();
            }

            _state = BlockVisualState.Destroying;
            AnimationStarted?.Invoke();
            _destroyCoroutine = StartCoroutine(DestroyAnimation(callback));
        }
        
        private void ResetState()
        {
            _state = BlockVisualState.Idle;
            
            if (_animator != null)
            {
                _animator.SetBool(DestroyTrigger, false);
            }
        }
        
        private void StopAllAnimations()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }
            
            _state = BlockVisualState.Idle;
        }
        
        private void ClearEvents()
        {
            AnimationStarted = null;
            AnimationFinished = null;
        }
        
        private IEnumerator MoveCoroutine(Vector3 targetWorldPos)
        {
            Vector3 startPos = transform.localPosition;
            float time = 0f;

            while (time < _moveDuration)
            {
                time += Time.deltaTime;
                float t = time / _moveDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.localPosition = Vector3.Lerp(startPos, targetWorldPos, t);
                yield return null;
            }

            transform.localPosition = targetWorldPos;
            _moveCoroutine = null;
            _state = BlockVisualState.Idle;
            AnimationFinished?.Invoke();
        }

        private IEnumerator DestroyAnimation(Action callback)
        {
            if (_animator != null)
            {
                _animator.SetBool(DestroyTrigger, true);
            }
            
            yield return _destroyWait;
            
            callback?.Invoke();
            AnimationFinished?.Invoke();
            _destroyCoroutine = null;
        }

        private void OnDestroy()
        {
            StopAllAnimations();
        }
    }
}
