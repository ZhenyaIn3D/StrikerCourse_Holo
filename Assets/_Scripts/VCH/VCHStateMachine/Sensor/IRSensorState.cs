namespace VCHStateMachine
{
    public class IRSensorState: SensorState
    {
        public IRSensorState()
        {
            Init();
        }
        
        public sealed override void Init()
        {
            base.Init();
            sensorKey = SensorControlKey.IR;
            
        }
    }
}