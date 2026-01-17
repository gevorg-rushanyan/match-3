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
        
        public BlockVisualState State => _state;
        public event Action OnAnimationStarted;
        public event Action OnAnimationFinished;
        
        public Vector2Int GridPosition => _gridPosition;
        
        public void Init(BlockType type, Vector2Int gridPosition, Vector3 worldPosition)
        {
            _animator.SetBool(DestroyTrigger, false);
            _state = BlockVisualState.Idle;
            _type = type;
            _gridPosition = gridPosition;
            transform.localPosition = worldPosition;
            gameObject.SetActive(true);
        }

        public void SetGridPosition(Vector2Int pos)
        {
            _gridPosition = pos;
        }

        public void MoveTo(Vector3 worldPos)
        {
            // transform.localPosition = worldPos;
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                OnAnimationFinished?.Invoke();
            }
            _state = BlockVisualState.Moving;
            OnAnimationStarted?.Invoke();
            _moveCoroutine = StartCoroutine(MoveCoroutine(worldPos));
        }

        public void PlayDestroyAnimation(Action callback = null)
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                OnAnimationFinished?.Invoke();
            }

            _state = BlockVisualState.Destroying;
            OnAnimationStarted?.Invoke();
            _destroyCoroutine = StartCoroutine(DestroyAnimation(callback));
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
            OnAnimationFinished?.Invoke();
        }

        private IEnumerator DestroyAnimation(Action callback)
        {
            _animator.SetBool(DestroyTrigger, true);
            yield return new WaitForSeconds(_destroyDuration);
            callback?.Invoke();
            OnAnimationFinished?.Invoke();
        }

        private void OnDestroy()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                OnAnimationFinished?.Invoke();
            }

            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                OnAnimationFinished?.Invoke();
            }
        }
    }
}