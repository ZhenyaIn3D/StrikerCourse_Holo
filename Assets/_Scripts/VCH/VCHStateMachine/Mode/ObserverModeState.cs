using Unity.VisualScripting;

namespace VCHStateMachine
{
    public class ObserverModeState : ModeControlState
    {
        public ObserverModeState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            modeKey = ModeControlKey.OBSERVER;
            

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