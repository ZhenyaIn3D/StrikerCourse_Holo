namespace VCHStateMachine
{
    public class MoreMARangeState: MARangeControlState
    {
        public MoreMARangeState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            maRangeControlKey = MARangeControlKey.MORE;
        }
    }
}