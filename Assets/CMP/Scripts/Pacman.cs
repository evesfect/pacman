using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    [RequireComponent(typeof(GridMovementController))]
    public class Pacman : MonoBehaviour
    {
        public Animator Animator;
        private const string FailAnimationName = "FailAnimation";

        private GridMovementController _movementController;
        private InputManager _inputManager;

        private Direction _currentDirection = Direction.None;
        private Direction _nextDirection = Direction.None;

        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.Empty, 
            CellType.Pacman, 
            CellType.JoinGameCell 
        };

        public void Initialize(GridMovementController movementController, InputManager inputManager, GridData gridData)
        {
            _movementController = movementController;
            _inputManager = inputManager;

            // Find starting coordinate from grid
            Vector2Int startCell = gridData.GetCoordsOfCellType(CellType.Pacman)[0];
            _movementController.Initialize(gridData, startCell);
        }

        private void Update()
        {
            if (_inputManager == null || _movementController == null) return;

            // Consume the input
            Direction input = _inputManager.ConsumeInput();
            if (input != Direction.None)
            {
                _nextDirection = input;
            }

            // Only make decisions when at the center of the cell
            if (_movementController.IsMoving) return;

            // Try to move to last buffered direction
            bool movedInNewDirection = _movementController.TryMoveInDirection(_nextDirection, GameSettings.PacmanMovementDuration, _walkableCells);
            
            if (movedInNewDirection)
            {
                _currentDirection = _nextDirection;
            }
            else
            {
                // Hit a wall in the new direction, try to keep going in the current direction
                _movementController.TryMoveInDirection(_currentDirection, GameSettings.PacmanMovementDuration, _walkableCells);
            }
        }

        public void PlayFailAnimation()
        {
            if (Animator != null)
            {
                Animator.Play(FailAnimationName);
            }
        }
    }
}