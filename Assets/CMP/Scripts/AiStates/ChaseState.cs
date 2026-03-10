using System.Collections.Generic;
using CMP.Scripts.Helper;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ChaseState : GhostState
    {
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.Empty, 
            CellType.Pacman, 
            CellType.JoinGameCell 
        };

        public ChaseState(GhostBlackboard blackboard) : base(blackboard) {}

        public override void OnEnter() {}

        public override void Update()
        {
            if (GhostBlackboard.MovementController.IsMoving) return;

            Direction nextStep = GhostBlackboard.CurrentDirection;

            if (IsAtCornerOrIntersection())
            {
                Vector2Int pacmanCell = GhostBlackboard.TargetPacman.GetComponent<GridMovementController>().CurrentCell;

                Direction reverseDir = Direction.None;
                if (GhostBlackboard.CurrentDirection != Direction.None)
                {
                    reverseDir = GhostBlackboard.CurrentDirection.Reverse();
                }

                nextStep = Pathfinder.GetShortestPathStep(
                    GhostBlackboard.GridData, 
                    GhostBlackboard.MovementController.CurrentCell, 
                    pacmanCell, 
                    _walkableCells, 
                    reverseDir
                );
            }

            if (nextStep != Direction.None)
            {
                GhostBlackboard.CurrentDirection = nextStep;
                GhostBlackboard.MovementController.TryMoveInDirection(
                    nextStep, 
                    GameSettings.AiMovementDuration, 
                    _walkableCells, 
                    false);
            }
        }

        private bool IsAtCornerOrIntersection()
        {
            Vector2Int currentCell = GhostBlackboard.MovementController.CurrentCell;
            Vector2Int forwardCell = currentCell + GhostBlackboard.CurrentDirection.ToVector2Int();
            
            bool canGoForward = false;
            if (GhostBlackboard.CurrentDirection != Direction.None && GhostBlackboard.GridData.IsCellMovable(forwardCell, _walkableCells))
            {
                canGoForward = true;
            }

            int validOptions = 0;
            foreach (Direction dir in GameSettings.DirectionsToCheck)
            {
                if (dir == GhostBlackboard.CurrentDirection.Reverse()) 
                {
                    continue; 
                }
                
                if (GhostBlackboard.GridData.IsCellMovable(currentCell + dir.ToVector2Int(), _walkableCells))
                {
                    validOptions++;
                }
            }

            return validOptions > 1 || !canGoForward;
        }

    }
}