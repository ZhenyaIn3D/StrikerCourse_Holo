namespace VCHStateMachine
{
    public class NoneZoomState: ZoomControlState
    {
        public NoneZoomState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            zoomControlKey = ZoomControlKey.NONE;
        }
    }
}