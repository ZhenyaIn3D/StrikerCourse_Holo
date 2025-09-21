namespace VCHStateMachine
{
    public class NoneAzymuthControlState : AzymuthControlState
    {
        
        public NoneAzymuthControlState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            azymuthKey = AzymuthControlKey.NONE;
        }
    }
}