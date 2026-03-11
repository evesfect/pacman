using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    public class GridMovementController : MonoBehaviour
    {
        public bool IsMoving { get; private set; }
        public Vector2Int CurrentCell { get; private set; }
        public Vector2Int TargetCell { get; private set; }
        
        private Direction _currentMovementDirection = Direction.None;
        private GridData _gridData;

        // State Machine Variables
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _elapsedTime;
        private float _actualDuration;

        public void Initialize(GridData gridData, Vector2Int startCell)
        {
            _gridData = gridData;
            CurrentCell = startCell;
            TargetCell = startCell;
            _currentMovementDirection = Direction.None;
            transform.position = new Vector3(startCell.x, startCell.y, transform.position.z);
            IsMoving = false;
            _elapsedTime = 0f;
        }

        private void Update()
        {
            if (!IsMoving) return;

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _actualDuration)
            {
                transform.position = _targetPosition;
                CurrentCell = TargetCell;
                IsMoving = false;

                _elapsedTime -= _actualDuration; 
            }
            else
            {
                float t = _elapsedTime / _actualDuration;
                transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
            }
        }

        public bool TryMoveInDirection(Direction direction, float duration, List<CellType> movableCellTypes, bool faceMovementDirection)
        {
            if (direction == Direction.None) return false;
            
            bool isReversing = IsMoving && direction == _currentMovementDirection.Reverse();

            if (IsMoving && !isReversing) return false;

            Vector2Int nextCell;
            if (isReversing) 
            {
                nextCell = CurrentCell;
                CurrentCell = TargetCell;
            }
            else // at the center of cell
            {
                nextCell = CurrentCell + direction.ToVector2Int();
            }

            if (_gridData.IsCellMovable(nextCell, movableCellTypes))
            {
                TargetCell = nextCell;
                _currentMovementDirection = direction;
                
                _startPosition = transform.position;
                _targetPosition = new Vector3(nextCell.x, nextCell.y, transform.position.z);
                
                float distance = Vector3.Distance(_startPosition, _targetPosition);
                _actualDuration = duration * distance;

                IsMoving = true;

                // If there is overflow time from previous cell, apply it
                if (!isReversing && _elapsedTime > 0f)
                {
                    // Cap overflow to prevent jumping if framerate drops
                    _elapsedTime = Mathf.Min(_elapsedTime, _actualDuration * 0.5f);
                    float t = _elapsedTime / _actualDuration;
                    transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
                }
                else
                {
                    // reversal bleed off timer
                    _elapsedTime = 0f;
                }

                if (faceMovementDirection)
                {
                    transform.rotation = direction.ToQuaternion();
                }

                return true;
            }

            // If cannot move, bleed the overflow time
            if (!IsMoving)
            {
                _elapsedTime = 0f;
            }

            return false;
        }

        public void StopMovement()
        {
            IsMoving = false;
            _elapsedTime = 0f;
        }
    }
}