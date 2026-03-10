using System.Collections.Generic;
using UnityEngine;
using CMP.Scripts.Helper;

namespace CMP.Scripts.AiStates
{
    public class JoiningGameState : GhostState
    {
        private Vector2Int _targetCell;
        private readonly Queue<Vector2Int> _bfsQueue = new Queue<Vector2Int>();
        private readonly Dictionary<Vector2Int, Vector2Int> _bfsCameFrom = new Dictionary<Vector2Int, Vector2Int>();
        
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.AiSpawnZone, 
            CellType.AiGate, 
            CellType.Empty, 
            CellType.JoinGameCell,
            CellType.Pacman
        };

        public JoiningGameState(GhostBlackboard blackboard) : base(blackboard) {}

        public override void OnEnter()
        {
            _targetCell = GhostBlackboard.GridData.GetCoordsOfCellType(CellType.JoinGameCell)[0];
        }

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return; // only check at cell centers

            if (GhostBlackboard.MovementController.CurrentCell == _targetCell) // check if arrived at join cell
            {
                GhostBlackboard.GhostComponent.ChangeState(
                    new ScatterState(GhostBlackboard),
                    CMP.Scripts.GhostState.Scatter
                );
                return; 
            }

            Direction nextStep = Pathfinder.GetShortestPathStep(
                GhostBlackboard.GridData, 
                GhostBlackboard.MovementController.CurrentCell, 
                _targetCell, 
                _walkableCells
            );
            
            if (nextStep != Direction.None)
            {
                GhostBlackboard.CurrentDirection = nextStep;
                GhostBlackboard.MovementController.TryMoveInDirection(
                    nextStep, 
                    GameSettings.AiMovementDuration, 
                    _walkableCells, 
                    false
                );
            }
        }

    }
}