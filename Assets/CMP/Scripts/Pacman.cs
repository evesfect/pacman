using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    [RequireComponent(typeof(GridMovementController))]
    public class Pacman : MonoBehaviour
    {
        public Animator Animator;
        private const string FailAnimationName = "FailAnimation";

        public GridMovementController MovementController { get; private set; }
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
            MovementController = movementController;
            _inputManager = inputManager;

            // Find starting coordinate from grid
            Vector2Int startCell = gridData.GetCoordsOfCellType(CellType.Pacman)[0];
            MovementController.Initialize(gridData, startCell);
        }

        private void Update()
        {
            if (_inputManager == null || MovementController == null) return;

            Direction input = _inputManager.ConsumeInput();
            if (input != Direction.None)
            {
                _nextDirection = input;
            }

            if (MovementController.IsMoving) 
            {
                if (_nextDirection == _currentDirection.Reverse() && _nextDirection != Direction.None)
                {
                    bool reversed = MovementController.TryMoveInDirection(_nextDirection, GameSettings.PacmanMovementDuration, _walkableCells, true);
                    if (reversed)
                    {
                        _currentDirection = _nextDirection;
                    }
                }
                return;
            }

            // Try to move to last buffered direction
            bool movedInNewDirection = MovementController.TryMoveInDirection(_nextDirection, GameSettings.PacmanMovementDuration, _walkableCells, true);
            
            if (movedInNewDirection)
            {
                _currentDirection = _nextDirection;
            }
            else
            {
                // Hit a wall in the new direction, try to keep going in the current direction
                MovementController.TryMoveInDirection(_currentDirection, GameSettings.PacmanMovementDuration, _walkableCells, true);
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