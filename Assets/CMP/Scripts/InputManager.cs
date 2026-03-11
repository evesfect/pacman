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
            if (LeftButton != null) LeftButton.onClick.AddListener(() => _currentDirection = Direction.Left);
            if (RightButton != null) RightButton.onClick.AddListener(() => _currentDirection = Direction.Right);
            if (UpButton != null) UpButton.onClick.AddListener(() => _currentDirection = Direction.Up);
            if (DownButton != null) DownButton.onClick.AddListener(() => _currentDirection = Direction.Down);
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
    }
}