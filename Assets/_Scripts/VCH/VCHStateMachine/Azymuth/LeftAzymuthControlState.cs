namespace VCHStateMachine
{
    public class LeftAzymuthControlState : AzymuthControlState
    {
        
        public LeftAzymuthControlState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            azymuthKey = AzymuthControlKey.LEFT;
        }
    }
}