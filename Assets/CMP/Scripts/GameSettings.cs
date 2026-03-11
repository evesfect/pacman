namespace CMP.Scripts
{
    public static class GameSettings
    {
        private static GameSettingsData _data;

        public static void Initialize(GameSettingsData data)
        {
            _data = data;
        }

        public static float AiMovementDuration => _data.AiMovementDuration;
        public static float PacmanMovementDuration => _data.PacmanMovementDuration;
        public static int AiCharacterCount => _data.AiCharacterCount;
        public static float[] AiJoinDelays => _data.AiJoinDelays;
        public static float CatchDistance => _data.CatchDistance;
        public static Direction[] DirectionsToCheck => _data.DirectionsToCheck;
        public static float CameraPadding => _data.CameraPadding;
        public static int MaxLineOfSightRange => _data.MaxLineOfSightRange;
    }
}