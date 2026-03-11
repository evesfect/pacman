namespace CMP.Scripts.AiStates
{
    public class GhostBlackboard
    {
        public Ghost GhostComponent;
        public GridMovementController MovementController;
        public GridData GridData;
        public Pacman TargetPacman;
        public int GhostIndex;
        public Direction CurrentDirection = Direction.None;
        public CMP.Scripts.GhostState CurrentStateEnum;

        public GhostBlackboard(Ghost ghost, GridMovementController movement, GridData grid, Pacman pacman, int index){
            GhostComponent = ghost;
            MovementController = movement;
            GridData = grid;
            TargetPacman = pacman;
            GhostIndex = index;
        }
    }
}