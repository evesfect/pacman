namespace CMP.Scripts.AiStates
{
    public abstract class GhostState
    {
        public GhostState(GhostBlackboard blackboard)
        {
            GhostBlackboard = blackboard;
        }
        
        protected GhostBlackboard GhostBlackboard;
        public abstract void OnEnter();
        public abstract void Update();
    }
}