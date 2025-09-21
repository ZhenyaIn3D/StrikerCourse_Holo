namespace VCHStateMachine
{
    public class MoreFocusState: FocusControlState
    {
        public MoreFocusState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            focusControlKey = FocusControlKey.MORE;
        }
    }
}