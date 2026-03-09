using UnityEngine;

namespace CMP.Scripts
{
    public class AssetDatabase
    {
        public static AssetDatabase Instance = new();

        public Pacman PacmanPrefab = Resources.Load<Pacman>("Pacman");
        public InputManager InputManagerPrefab = Resources.Load<InputManager>("InputManager");
        public GridData GridData = Resources.Load<GridData>("GridData");
        public Ghost Ghost = Resources.Load<Ghost>("Ghost");
        public MapVisualSettings MapVisualSettings = Resources.Load<MapVisualSettings>("MapVisualSettings");
        
    }
}