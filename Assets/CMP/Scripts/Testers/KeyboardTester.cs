using UnityEngine;

namespace CMP.Scripts
{
    [RequireComponent(typeof(InputManager))]
    public class KeyboardDebugTester : MonoBehaviour
    {
        private InputManager _inputManager;

        private void Awake()
        {
            _inputManager = GetComponent<InputManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                _inputManager.SetInput(Direction.Up);
                
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                _inputManager.SetInput(Direction.Down);
                
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                _inputManager.SetInput(Direction.Left);
                
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                _inputManager.SetInput(Direction.Right);
        }
    }
}