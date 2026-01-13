using System;
using Enums;
using UnityEngine;

namespace Core.Input
{
    public class SwipeInput : MonoBehaviour
    {
        [Range(0.01f, 0.3f)]
        [SerializeField] private float _minSwipeScreenPercent = 0.1f;
        
        private Vector2 _startPos;
        private bool _isSwiping;
        
        // TODO 
        private float MinSwipeDistance
        {
            get
            {
                float minSide = Mathf.Min(Screen.width, Screen.height);
                return minSide * _minSwipeScreenPercent;
            }
        }
        
        public event Action<Vector2, SwipeDirection> OnSwipe;

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        private void HandleMouse()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _startPos = UnityEngine.Input.mousePosition;
                _isSwiping = true;
            }

            if (UnityEngine.Input.GetMouseButtonUp(0) && _isSwiping)
            {
                DetectSwipe(UnityEngine.Input.mousePosition);
                _isSwiping = false;
            }
        }

        private void HandleTouch()
        {
            if (UnityEngine.Input.touchCount == 0)
            {
                return;
            }

            var touch = UnityEngine.Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
                _isSwiping = true;
            }

            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _isSwiping)
            {
                DetectSwipe(touch.position);
                _isSwiping = false;
            }
        }

        private void DetectSwipe(Vector2 endPos)
        {
            var delta = endPos - _startPos;

            if (delta.magnitude < MinSwipeDistance)
            {
                return;
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                OnSwipe?.Invoke(_startPos, delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left);
            }
            else
            {
                OnSwipe?.Invoke(_startPos, delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
            }
        }
    }
}