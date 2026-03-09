using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    public enum CellType
    {
        Empty,
        Wall,
        AiSpawnZone,
        AiGate,
        Pacman,
        JoinGameCell,
        Invalid,
    }

    [CreateAssetMenu(fileName = "NewGridData", menuName = "PacMan/Grid Data")]
    public class GridData : ScriptableObject
    {
        public int Width;
        public int Height;
        public CellType[] Grid;

        public List<Vector2Int> GetCoordsOfCellType(CellType targetType)
        {
            var coords = new List<Vector2Int>();
            for (var i = 0; i < Grid.Length; i++)
            {
                if (Grid[i] == targetType)
                {
                    var y = i / Width;
                    var x = i - y * Width;
                    coords.Add(new Vector2Int(x,y));
                }
            }

            Debug.Assert(coords.Count != 0);
            return coords;
        }
        
        public bool GetInBounds(Vector2Int coords)
        {
            return coords.x >= 0 && coords.x < Width && coords.y >= 0 && coords.y < Height;
        }
        
        public CellType GetCellAtOrDefault(Vector2Int coords, CellType cellType)
        {
            return !GetInBounds(coords) ? cellType : GetCellAt(coords.x, coords.y);
        }
        
        public CellType GetCellAt(Vector2Int coords)
        {
            return GetCellAt(coords.x, coords.y);
        }
        
        public CellType GetCellAt(int x, int y)
        {
            return Grid[y * Width + x];
        }
        
        public bool IsCellMovable(Vector2Int cellCoords, List<CellType> availableCells)
        {
            return GetInBounds(cellCoords) && availableCells.Contains(GetCellAt(cellCoords));
        }
    }
}