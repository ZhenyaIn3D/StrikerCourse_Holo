namespace VCHStateMachine
{
    public class MoreZoomState: ZoomControlState
    {
        public MoreZoomState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            zoomControlKey = ZoomControlKey.MORE;
        }
    }
}