namespace VCHStateMachine
{
    public class WhiteHotPolarityState : PolarityState
    {
        public WhiteHotPolarityState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            polarityKey = PolarityControlKey.WHITEHOT;
            
        }
    }
}