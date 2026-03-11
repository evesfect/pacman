using UnityEngine;

namespace CMP.Scripts
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "PacMan/Game Settings")]
    public class GameSettingsData : ScriptableObject
    {
        public float AiMovementDuration = 0.25f;
        public float PacmanMovementDuration = 0.25f;
        public int AiCharacterCount = 3;
        public float[] AiJoinDelays = { 1f, 3f, 4f };
        public float CatchDistance = 0.3f;
        public Direction[] DirectionsToCheck = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
        public float CameraPadding = 1f;
        public int MaxLineOfSightRange = 4;
    }
}