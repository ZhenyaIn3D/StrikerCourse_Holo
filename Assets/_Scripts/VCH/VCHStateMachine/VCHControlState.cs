namespace VCHStateMachine
{
    public abstract class VCHControlState
    {
        protected bool isInitialized = false;
        
        public virtual void Init()
        {
            isInitialized = true;
        }
        
        public virtual void OnEnter()
        {
            
        }
        
        public virtual void OnExit()
        {
            
        }
    }
}