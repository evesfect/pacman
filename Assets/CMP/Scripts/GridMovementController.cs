using System;
using System.Collections;
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
        private Coroutine _movementCoroutine;

        // Initializes the controller and snaps the object to the starting cell
        public void Initialize(GridData gridData, Vector2Int startCell)
        {
            _gridData = gridData;
            CurrentCell = startCell;
            TargetCell = startCell;
            _currentMovementDirection = Direction.None;
            transform.position = new Vector3(startCell.x, startCell.y, transform.position.z);
            IsMoving = false;
        }

        // Attempts to move the object to the adjacent cell in the given direction
        public bool TryMoveInDirection(Direction direction, float duration, List<CellType> movableCellTypes, bool faceMovementDirection)
        {
            if (direction == Direction.None)
                return false;
            
            bool isReversing = IsMoving && direction == _currentMovementDirection.Reverse();

            if (IsMoving && !isReversing)
                return false;


            Vector2Int nextCell;
            if (isReversing)
            {
                nextCell = CurrentCell;
                CurrentCell = TargetCell;
                StopMovement();
            }
            else
            {
                nextCell = CurrentCell + direction.ToVector2Int();
            }

            if (_gridData.IsCellMovable(nextCell, movableCellTypes))
            {
                TargetCell = nextCell;
                _currentMovementDirection = direction;
                _movementCoroutine = StartCoroutine(MoveRoutine(nextCell, duration));
                
                if (faceMovementDirection)
                    transform.rotation = direction.ToQuaternion();
                return true;
            }

            return false;
        }

        private IEnumerator MoveRoutine(Vector2Int targetCell, float duration)
        {
            IsMoving = true;
            
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = new Vector3(targetCell.x, targetCell.y, transform.position.z);
            
            // calculate duration for mid cell reversals
            float distance = Vector3.Distance(startPosition, targetPosition);
            float actualDuration = duration * distance;

            if (actualDuration > 0f)
            {
                float elapsedTime = 0f;
                while (elapsedTime < actualDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / actualDuration);
                    transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    
                    yield return null;
                }
            }

            transform.position = targetPosition;
            CurrentCell = targetCell;
            IsMoving = false;
        }

        public void StopMovement()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                IsMoving = false;
            }
        }
    }
}