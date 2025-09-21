namespace VCHStateMachine
{
    public class LessMARangeState: MARangeControlState
    {
        public LessMARangeState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            maRangeControlKey = MARangeControlKey.LESS;
        }
    }
}