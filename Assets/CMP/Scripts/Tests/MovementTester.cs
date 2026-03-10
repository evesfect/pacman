using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    [RequireComponent(typeof(GridMovementController))]
    public class MovementTester : MonoBehaviour
    {
        private GridMovementController _movementController;
        private InputManager _inputManager;
        
        // These are the cells this dummy object is allowed to walk on
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.Empty, 
            CellType.Pacman, 
            CellType.JoinGameCell 
        };

        private void Start()
        {
            _movementController = GetComponent<GridMovementController>();
            
            // Fetch dependencies from AssetDatabase and instantiate InputManager
            var gridData = AssetDatabase.Instance.GridData;
            _inputManager = Instantiate(AssetDatabase.Instance.InputManagerPrefab);

            // Find a valid starting spot (e.g., Pacman's start location)
            Vector2Int startCell = gridData.GetCoordsOfCellType(CellType.Pacman)[0];
            
            _movementController.Initialize(gridData, startCell);
        }

        private void Update()
        {
            // If we are currently moving, wait until we reach the center of the next cell
            if (_movementController.IsMoving) return;

            Direction inputDirection = _inputManager.ConsumeInput();
            
            if (inputDirection != Direction.None)
            {
                _movementController.TryMoveInDirection(inputDirection, GameSettings.PacmanMovementDuration, _walkableCells, true);
            }
        }
    }
}