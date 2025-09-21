namespace VCHStateMachine
{
    public class NoneMARangeState: MARangeControlState
    {
        public NoneMARangeState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            maRangeControlKey = MARangeControlKey.NONE;
        }
    }
}