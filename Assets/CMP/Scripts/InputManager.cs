using UnityEngine;
using UnityEngine.UI;

namespace CMP.Scripts
{
    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    
    public class InputManager : MonoBehaviour
    {
        public Button LeftButton;
        public Button RightButton;
        public Button UpButton;
        public Button DownButton;

        Direction _currentDirection;
        
        private void Awake()
        {
            LeftButton.onClick.AddListener(() =>
            {
                _currentDirection = Direction.Left;
            });
            RightButton.onClick.AddListener(() =>
            {
                _currentDirection = Direction.Right;
            });
            UpButton.onClick.AddListener(() =>
            {
                _currentDirection = Direction.Up;
            });
            DownButton.onClick.AddListener(() =>
            {
                _currentDirection = Direction.Down;
            });
        }

        public Direction ConsumeInput()
        {
            var dir = _currentDirection;
            _currentDirection = Direction.None;
            return dir;
        }
    }
}