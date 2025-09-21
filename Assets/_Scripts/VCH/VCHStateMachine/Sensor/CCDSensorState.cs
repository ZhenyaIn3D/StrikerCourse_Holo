namespace VCHStateMachine
{
    public class CCDSensorState: SensorState
    {
        public CCDSensorState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            sensorKey = SensorControlKey.CCD;
            
        }
    }
}