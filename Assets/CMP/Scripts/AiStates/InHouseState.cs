using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class InHouseState : GhostState
    {
        private float _timer;
        private readonly float _joinDelay;
        
        // In the house, ghosts can only walk on the spawn zone
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.AiSpawnZone 
        };

        public InHouseState(GhostBlackboard blackboard) : base(blackboard)
        {
            // Fetch this specific ghost's delay from the settings
            _joinDelay = GameSettings.AiJoinDelays[blackboard.GhostIndex];
        }

        public override void OnEnter()
        {
            _timer = 0f;
            
            // Start by trying to move Up or Down
            GhostBlackboard.CurrentDirection = Random.value > 0.5f ? Direction.Up : Direction.Down;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;

            // 1. Check if it's time to join the game
            if (_timer >= _joinDelay)
            {
                GhostBlackboard.GhostComponent.ChangeState(
                    new JoiningGameState(GhostBlackboard), 
                    CMP.Scripts.GhostState.JoiningGame);
                return;
            }

            // 2. Handle Up/Down bouncing movement
            if (GhostBlackboard.MovementController.IsMoving) return;

            bool moved = GhostBlackboard.MovementController.TryMoveInDirection(
                GhostBlackboard.CurrentDirection, 
                GameSettings.AiMovementDuration, 
                _walkableCells,
                false);

            // If we hit the boundary of the spawn zone, reverse direction
            if (!moved)
            {
                GhostBlackboard.CurrentDirection = GhostBlackboard.CurrentDirection.Reverse();
                GhostBlackboard.MovementController.TryMoveInDirection(
                    GhostBlackboard.CurrentDirection, 
                    GameSettings.AiMovementDuration, 
                    _walkableCells,
                    false);
            }
        }
    }
}