using UnityEngine;
using UnityEngine.UI;

namespace CMP.Scripts
{ 
    public class InputManager : MonoBehaviour
    {
        public Button LeftButton;
        public Button RightButton;
        public Button UpButton;
        public Button DownButton;

        [Header("Swipe Settings")]
        [Tooltip("Minimum distance in pixels to register a swipe.")]
        public float swipeResistance = 50f; 
        private Direction _currentDirection;
        private Vector2 _startTouchPosition;
        private Vector2 _currentTouchPosition;
        
        private void Awake()
        {
            if (LeftButton != null) LeftButton.onClick.AddListener(() => _currentDirection = Direction.Left);
            if (RightButton != null) RightButton.onClick.AddListener(() => _currentDirection = Direction.Right);
            if (UpButton != null) UpButton.onClick.AddListener(() => _currentDirection = Direction.Up);
            if (DownButton != null) DownButton.onClick.AddListener(() => _currentDirection = Direction.Down);
        }

        private void Update()
        {
            DetectSwipe();
        }

        public void SetInput(Direction dir)
        {
            _currentDirection = dir;
        }

        public Direction ConsumeInput()
        {
            var dir = _currentDirection;
            _currentDirection = Direction.None;
            return dir;
        }

        private void DetectSwipe()
        {
            if (Input.touches.Length > 0)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                {
                    _startTouchPosition = t.position;
                }
                else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Ended)
                {
                    _currentTouchPosition = t.position;
                    EvaluateSwipe();
                }
            }
            // mouse input fallback for editor testing
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _startTouchPosition = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                {
                    _currentTouchPosition = Input.mousePosition;
                    EvaluateSwipe();
                }
            }
        }

        private void EvaluateSwipe()
        {
            Vector2 swipeDelta = _currentTouchPosition - _startTouchPosition;

            if (swipeDelta.magnitude > swipeResistance)
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    SetInput(swipeDelta.x > 0 ? Direction.Right : Direction.Left);
                }
                else
                {
                    SetInput(swipeDelta.y > 0 ? Direction.Up : Direction.Down);
                }

                _startTouchPosition = _currentTouchPosition; // reset so can swipe continously
            }
        }
    }
}