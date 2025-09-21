namespace VCHStateMachine
{
    public class ShooterModeState : ModeControlState
    {
        public ShooterModeState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            modeKey = ModeControlKey.SHOOTER;
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}