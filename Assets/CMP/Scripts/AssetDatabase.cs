using UnityEngine;

namespace CMP.Scripts
{
    [CreateAssetMenu(fileName = "AssetDatabase", menuName = "PacMan/Asset Database")]
    public class AssetDatabase : ScriptableObject
    {
        public Pacman PacmanPrefab;
        public InputManager InputManagerPrefab;
        public GridData GridData;
        public Ghost Ghost;
        public MapVisualSettings MapVisualSettings;

        private void OnEnable()
        {
            if (PacmanPrefab == null) PacmanPrefab = Resources.Load<Pacman>("Pacman");
            if (InputManagerPrefab == null) InputManagerPrefab = Resources.Load<InputManager>("InputManager");
            if (GridData == null) GridData = Resources.Load<GridData>("GridData");
            if (Ghost == null) Ghost = Resources.Load<Ghost>("Ghost");
            if (MapVisualSettings == null) MapVisualSettings = Resources.Load<MapVisualSettings>("MapVisualSettings");
        }
        
    }
}