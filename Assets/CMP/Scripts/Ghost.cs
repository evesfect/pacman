using UnityEngine;
using CMP.Scripts.AiStates;
using System;

namespace CMP.Scripts
{
    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }

    [Serializable]
    public class EyePupils
    {
        public GameObject Up;
        public GameObject Down;
        public GameObject Left;
        public GameObject Right;
        public void UpdateDirection(Direction dir)
        {
            if (dir == Direction.None) 
            {
                return;
            }

            if (Up != null) Up.SetActive(dir == Direction.Up);
            if (Down != null) Down.SetActive(dir == Direction.Down);
            if (Left != null) Left.SetActive(dir == Direction.Left);
            if (Right != null) Right.SetActive(dir == Direction.Right);
        }
    }

    [RequireComponent(typeof(GridMovementController))]
    public class Ghost : MonoBehaviour
    {
        public EyePupils LeftPupils;
        public EyePupils RightPupils;

        public GhostBlackboard Blackboard {get; private set;}

        private CMP.Scripts.AiStates.GhostState _currentAiState;
        public static event Action OnPacmanSpotted;

        public void Initialize(GridData gridData, Pacman pacman, int ghostIndex, Vector2Int startCell)
        {
            var movementController = GetComponent<GridMovementController>();
            movementController.Initialize(gridData, startCell);
            Blackboard = new GhostBlackboard(this, movementController, gridData, pacman, ghostIndex);
        }

        public void ChangeState(CMP.Scripts.AiStates.GhostState newState, CMP.Scripts.GhostState enumState)
        {
            _currentAiState = newState;
            Blackboard.CurrentStateEnum = enumState;
            _currentAiState?.OnEnter();
        }

        private void Update()
        {
            _currentAiState?.Update();

            if (Blackboard != null)
            {
                // eye movement
                LeftPupils.UpdateDirection(Blackboard.CurrentDirection);
                RightPupils.UpdateDirection(Blackboard.CurrentDirection);

                // pacman spotting
                if (Blackboard.CurrentStateEnum == CMP.Scripts.GhostState.Scatter)
                {
                    if (CheckLineOfSight())
                    {
                        OnPacmanSpotted?.Invoke();
                    }
                }
            }
        }

        private bool CheckLineOfSight()
        {
            Direction facingDir = Blackboard.CurrentDirection;
            if (facingDir == Direction.None) return false;

            Vector2Int stepDir = facingDir.ToVector2Int();
            Vector2Int currentCell = Blackboard.MovementController.CurrentCell;
            Vector2Int pacmanCell = Blackboard.TargetPacman.MovementController.CurrentCell; 

            for (int i = 1; i <= GameSettings.MaxLineOfSightRange; i++)
            {
                Vector2Int checkCell = currentCell + stepDir * i;

                if (!Blackboard.GridData.GetInBounds(checkCell) || Blackboard.GridData.GetCellAt(checkCell) == CellType.Wall)
                    return false; 

                if (checkCell == pacmanCell)
                    return true; 
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if (Blackboard == null || Blackboard.GridData == null) return;

            Direction facingDir = Blackboard.CurrentDirection;
            if (facingDir == Direction.None) return;

            Vector2Int stepDir = facingDir.ToVector2Int();
            Vector2Int currentCell = Blackboard.MovementController.CurrentCell;
            
            Gizmos.color = Blackboard.CurrentStateEnum == CMP.Scripts.GhostState.Chase ? Color.red : Color.yellow;
            
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos;

            for (int i = 1; i <= GameSettings.MaxLineOfSightRange; i++)
            {
                Vector2Int checkCell = currentCell + stepDir * i;
                
                if (!Blackboard.GridData.GetInBounds(checkCell) || Blackboard.GridData.GetCellAt(checkCell) == CellType.Wall) break;
                endPos = new Vector3(checkCell.x, checkCell.y, startPos.z);
            }

            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawWireSphere(endPos, 0.2f);
        }
    }
}