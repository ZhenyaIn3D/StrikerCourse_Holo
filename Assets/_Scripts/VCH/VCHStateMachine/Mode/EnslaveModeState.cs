namespace VCHStateMachine
{
    public class EnslaveModeState : ModeControlState
    {
        public EnslaveModeState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            modeKey = ModeControlKey.ENSLAVE;
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