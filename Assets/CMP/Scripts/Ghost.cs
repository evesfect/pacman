using UnityEngine;
using CMP.Scripts.AiStates;

namespace CMP.Scripts
{
    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }

    [RequireComponent(typeof(GridMovementController))]
    public class Ghost : MonoBehaviour
    {
        public GameObject LeftEye;
        public GameObject RightEye;
        public GhostBlackboard Blackboard {get; private set;}

        private CMP.Scripts.AiStates.GhostState _currentAiState;

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
        }
    }
}