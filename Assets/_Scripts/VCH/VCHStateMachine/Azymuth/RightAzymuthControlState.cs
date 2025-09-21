namespace VCHStateMachine
{
    public class RightAzymuthControlState : AzymuthControlState
    {
        
        public RightAzymuthControlState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            azymuthKey = AzymuthControlKey.RIGHT;
        }
    }
}