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

    public enum Direction 
    { 
        None, 
        Left, 
        Right, 
        Up, 
        Down 
    }

    public enum GameMode
    {
        Scatter,
        Chase,
        GameOver,
    }

    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }
}