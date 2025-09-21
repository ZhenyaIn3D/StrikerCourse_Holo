namespace VCHStateMachine
{
    public class BlackHotPolarityState : PolarityState
    {
        
        public BlackHotPolarityState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            polarityKey = PolarityControlKey.BLACKHOT;
        }
    }
}