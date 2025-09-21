using System;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using FileAccess;

namespace VCHStateMachine
{
    public class VCHStateMachineController : MonoBehaviour
    {
        public static VCHStateMachineController Instance;
        private VCHMachineState currentState;
        [SerializeField] private VCHStateInterpreter VchStateInterpreter;

        public Dictionary<StepName, Func<bool>> endConditions;

        private bool stateChanged = false;
        
        // MODE
        private ModeControlKey receivedModeKey;
        public ModeControlState CurrentMode => currentState.currentMode;
        public ShooterModeState shooterMode = new ShooterModeState();
        public ObserverModeState observerMode = new ObserverModeState();
        public EnslaveModeState enslaveMode = new EnslaveModeState();

        //SENSOR
        private SensorControlKey receivedSensorKey;
        public SensorState CurrentSensor => currentState.currentSensor;
        public SensorState cddSensor = new CCDSensorState();
        public SensorState irSensor = new IRSensorState();

        //POLARITY
        private PolarityControlKey receivedPolarityKey;
        public PolarityState CurrentPolarity => currentState.currentPolarity;
        public PolarityState blackPolarity = new BlackHotPolarityState();
        public PolarityState whitePolarity = new WhiteHotPolarityState();

        //AZYMUTH
        private AzymuthControlKey receivedAzymuthKey;
        public AzymuthControlState CurrentAzymuth => currentState.currentAzymuth;
        public AzymuthControlState azymuthRight  = new RightAzymuthControlState();
        public AzymuthControlState azymuthLeft = new LeftAzymuthControlState();
        public AzymuthControlState azymuthNone  = new NoneAzymuthControlState();
        
        //ELEVATION
        // private ElevationControlKey receivedElevationKey;
        // public ElevationControlState CurrentElevation => currentState.currentElevation;
        // public ElevationControlState elevationUp  = new UpElevationControlState();
        // public ElevationControlState elevationDown = new DownElevationControlState();
        // public ElevationControlState elevationNone  = new NoneElevationControlState();
        
        
        //DATA
        private Dictionary<string, ModeControlKey> modeControlKeyDictionary;
        private Dictionary<string, SensorControlKey> sensorKeyDictionary;
        private Dictionary<string, PolarityControlKey> polarityKeyDictionary;
        private Dictionary<string, AzymuthControlKey> azymuthControlKeyDictionary;
        private Dictionary<string, ElevationControlKey> elevationControlKeyDictionary;
        
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null) Instance = this;

            currentState = new VCHMachineState();
            
            endConditions = new Dictionary<StepName, Func<bool>>
            {
                { StepName.SightModeSelectionToObserver, IsObserverMode },
                { StepName.SightModeSelectionToEnslave, IsEnslaveMode },
                { StepName.SightModeSelectionToShooter, IsShooterMode },
                { StepName.SensorToggleToIR, IsSensorIR },
                { StepName.SensorToggleToCDD, IsSensorCDD },
                { StepName.PolarityToggleToBlackHot, IsPolarityBlack },
                { StepName.PolarityToggleToWhiteHot, IsPolarityWhite }
            };
            
            VchStateInterpreter.ReportVCHState += OnVCHStateReport;

            modeControlKeyDictionary = new Dictionary<string, ModeControlKey>()
            {
                { "SHOOTER", ModeControlKey.SHOOTER },
                { "OBSERVER", ModeControlKey.OBSERVER },
                { "ENSLAVE", ModeControlKey.ENSLAVE }
            };

            sensorKeyDictionary = new Dictionary<string, SensorControlKey>()
            {
                { "CCD", SensorControlKey.CCD },
                { "IR", SensorControlKey.IR }
            };
            
            polarityKeyDictionary = new Dictionary<string, PolarityControlKey>()
            {
                { "WHITEHOT", PolarityControlKey.WHITEHOT },
                { "BLACKHOT", PolarityControlKey.BLACKHOT }

            };
            
            azymuthControlKeyDictionary = new Dictionary<string, AzymuthControlKey>()
            {
                { "LEFT", AzymuthControlKey.LEFT },
                { "RIGHT", AzymuthControlKey.RIGHT },
                { "NONE", AzymuthControlKey.NONE }
            };
            
            elevationControlKeyDictionary = new Dictionary<string, ElevationControlKey>()
            {
                { "UP", ElevationControlKey.UP },
                { "DOWN", ElevationControlKey.DOWN },
                { "NONE", ElevationControlKey.NONE }
            };

        }

        public bool IsStateChanged()
        {
            return stateChanged;
        }

        #region Shooter Control
        public bool IsObserverMode()
        {
            return currentState.currentMode == observerMode;
        }

        public bool IsShooterMode()
        {
            return currentState.currentMode == shooterMode;
        }

        public bool IsEnslaveMode()
        {
            return currentState.currentMode == enslaveMode;
        }
        #endregion
        
        #region Sensor
        public bool IsSensorIR()
        {
            return currentState.currentSensor == irSensor;
        }

        public bool IsSensorCDD()
        {
            return currentState.currentSensor == cddSensor;
        }
        #endregion
        
        #region Polarity
        public bool IsPolarityBlack()
        {
            return currentState.currentPolarity == blackPolarity;
        }

        public bool IsPolarityWhite()
        {
            return currentState.currentPolarity == whitePolarity;
        }
        #endregion
        
        #region Azymuth
        public bool IsAzymuthRight()
        {
            return currentState.currentAzymuth == azymuthRight;
        }

        public bool IsAzymuthLeft()
        {
            return currentState.currentAzymuth == azymuthLeft;
        }

        public bool IsAzymuthNone()
        {
            return currentState.currentAzymuth == azymuthNone;
        }
        #endregion
        
        private void OnVCHStateReport(VCHState newState)
        {
            stateChanged = false;
            // MODE, if new
            receivedModeKey = modeControlKeyDictionary[newState.Mode];
            if (currentState.currentMode == null || receivedModeKey != currentState.currentMode.ModeKey) 
            {
                ChangeModeState(receivedModeKey); // only for the first
                stateChanged = true;
            }
            
            // SENSOR, if new
            receivedSensorKey = sensorKeyDictionary[newState.Sensor];
            if (currentState.currentSensor == null || receivedSensorKey != currentState.currentSensor.SensorKey)
            {
                ChangeSensorState(receivedSensorKey);
                stateChanged = true;
            }

            // POLARITY, if new
            receivedPolarityKey = polarityKeyDictionary[newState.Polarity];
            if (currentState.currentPolarity == null || receivedPolarityKey != currentState.currentPolarity.PolarityKey)
            {
                ChangePolarityState(receivedPolarityKey);
                stateChanged = true;
            }
            

            // AZYMUTH, if new
            // receivedAzymuthKey = azymuthControlKeyDictionary[newState.Azymuth_Axes];
            // if (currentAzymuth == null) ChangeAzymuthControlState(receivedAzymuthKey);
            // if (receivedAzymuthKey != currentAzymuth.AzymuthControlKey) ChangeAzymuthControlState(receivedAzymuthKey);
            
        }

        public void ChangeModeState(ModeControlKey mode)
        {
            if (currentState.currentMode != null)
            {
                currentState.currentMode.OnExit();
            }

            switch (mode)
            {
                case ModeControlKey.SHOOTER:
                    currentState.currentMode = shooterMode;
                    break;
                case ModeControlKey.OBSERVER:
                    currentState.currentMode = observerMode;
                    break;
                case ModeControlKey.ENSLAVE:
                    currentState.currentMode = enslaveMode;
                    break;
                default:
                    throw new Exception("Unknown Mode Control State");
            }

            currentState.currentMode.OnEnter();
        }

        public void ChangeSensorState(SensorControlKey sensor)
        {
            if (currentState.currentSensor != null)
            {
                currentState.currentSensor.OnExit();
            }

            switch (sensor)
            {
                case SensorControlKey.CCD:
                    currentState.currentSensor = cddSensor;
                    break;
                case SensorControlKey.IR:
                    currentState.currentSensor = irSensor;
                    break;

                default:
                    throw new Exception("Unknown Sensor Control State");
            }

            currentState.currentSensor.OnEnter();
        }

        public void ChangePolarityState(PolarityControlKey pol)
        {
            if (currentState.currentPolarity != null)
            {
                currentState.currentPolarity.OnExit();
            }

            switch (pol)
            {
                case PolarityControlKey.WHITEHOT:
                    currentState.currentPolarity = whitePolarity;
                    break;
                case PolarityControlKey.BLACKHOT:
                    currentState.currentPolarity = blackPolarity;
                    break;
                default:
                    throw new Exception("Unknown Polarity Control State");
            }

            currentState.currentPolarity.OnEnter();
        }

        public void ChangeAzymuthControlState(AzymuthControlKey az)
        {
            if (currentState.currentAzymuth != null)
            {
                currentState.currentAzymuth.OnExit();
            }

            switch (az)
            {
                case AzymuthControlKey.LEFT:
                    currentState.currentAzymuth = azymuthLeft;
                    break;
                case AzymuthControlKey.RIGHT:
                    currentState.currentAzymuth = azymuthRight;
                    break;
                case AzymuthControlKey.NONE:
                    currentState.currentAzymuth = azymuthNone;
                    break;
                default:
                    throw new Exception("Unknown Azymuth Control State");
            }

            currentState.currentAzymuth.OnEnter();
        }
        
    }
}
