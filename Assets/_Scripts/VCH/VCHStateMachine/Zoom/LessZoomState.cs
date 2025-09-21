namespace VCHStateMachine
{
    public class LessZoomState: ZoomControlState
    {
        public LessZoomState()
        {
            Init();
        }

        public sealed override void Init()
        {
            base.Init();
            zoomControlKey = ZoomControlKey.LESS;
        }
    }
}