using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    public static class Extensions
    {
        public static bool GetIsMovable(this CellType cellType)
        {
            return cellType is CellType.Pacman or CellType.Empty or CellType.JoinGameCell;
        }

        public static List<Vector2Int> GetNeighbours(this Vector2Int cellCoords)
        {
            return new List<Vector2Int>
            {
                cellCoords + Vector2Int.left, 
                cellCoords + Vector2Int.right, 
                cellCoords + Vector2Int.up,
                cellCoords + Vector2Int.down
            };
        }

        public static Vector2Int ToVector2Int(this Direction cellType)
        {
            switch (cellType)
            {
                case Direction.None:
                    return Vector2Int.zero;
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Down:
                    return Vector2Int.down;
            }

            Debug.Assert(false);
            return Vector2Int.zero;
        }

        public static Direction Reverse(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                _ => Direction.None
            };
        }
        
        public static Direction ToDirection(this Vector2Int vector)
        {
            if (vector == Vector2Int.left)
            {
                return Direction.Left;
            }
            if (vector == Vector2Int.right)
            {
                return Direction.Right;
            }
            if (vector == Vector2Int.up)
            {
                return Direction.Up;
            }
            if (vector == Vector2Int.down)
            {
                return Direction.Down;
            }

            return Direction.None;
        }

        public static Quaternion ToQuaternion(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return Quaternion.Euler(0, 0, 90);
                case Direction.Right:
                    return Quaternion.Euler(0, 0, -90);
                case Direction.Up:
                    return Quaternion.Euler(0, 0, 0);
                case Direction.Down:
                    return Quaternion.Euler(0, 0, 180);
            }
            
            return Quaternion.identity;
        }
    }
}