namespace VCHStateMachine
{
    public class LessFocusState: FocusControlState
    {
        public LessFocusState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            focusControlKey = FocusControlKey.LESS;
        }
    }
}