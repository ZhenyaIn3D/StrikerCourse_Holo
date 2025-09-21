namespace VCHStateMachine
{
    public class VCHMachineState
    {
        public ModeControlState currentMode; // SHOOTER/OBSERVER/ENSLAVE
        public SensorState currentSensor; // CCD/IR
        public PolarityState currentPolarity; // WHITEHOT/ BLACKHOT
        public AzymuthControlState currentAzymuth; // direction?
        public ElevationControlState currentElevation; //direction?
        public ZoomControlState currentZoomDirection;
        public FocusControlState currentFocusDirection;
        
    }
}