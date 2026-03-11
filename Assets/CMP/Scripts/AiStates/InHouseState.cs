using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class InHouseState : GhostState
    {
        private float _timer;
        private readonly float _joinDelay;
        
        private readonly List<CellType> _walkableCells = new List<CellType> 
        { 
            CellType.AiSpawnZone 
        };

        public InHouseState(GhostBlackboard blackboard) : base(blackboard)
        {
            _joinDelay = GameSettings.AiJoinDelays[blackboard.GhostIndex];
        }

        public override void OnEnter()
        {
            _timer = 0f;
            
            GhostBlackboard.CurrentDirection = Random.value > 0.5f ? Direction.Up : Direction.Down;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _joinDelay)
            {
                GhostBlackboard.GhostComponent.ChangeState(
                    new JoiningGameState(GhostBlackboard), 
                    CMP.Scripts.GhostState.JoiningGame);
                return;
            }

            // handle idle bouncing
            if (GhostBlackboard.MovementController.IsMoving) return;

            bool moved = GhostBlackboard.MovementController.TryMoveInDirection(
                GhostBlackboard.CurrentDirection, 
                GameSettings.AiMovementDuration, 
                _walkableCells,
                false);

            if (!moved) // if hit boundary reverse dir
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