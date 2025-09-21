namespace VCHStateMachine
{
    public class NoneFocusState: FocusControlState
    {
        public NoneFocusState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            focusControlKey = FocusControlKey.NONE;
        }
    }
}