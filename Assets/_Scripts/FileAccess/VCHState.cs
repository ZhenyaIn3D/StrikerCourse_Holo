using VCHStateMachine;

namespace FileAccess
{
    [System.Serializable]
    public class VCHState
    {
        public string Mode; // SHOOTER, OBSERVER, ENSLAVE
        public string Sensor; // CCD, IR
        public string Polarity; // WHITEHOT, BLACKHOT 
        
        public string Azymuth_Axes; // LEFT, NONE, RIGHT
        public float Azymuth_value;
        public float Azymuth_step = 0.01f;
        public string Elevation_Axes; //UP, NONE, DOWN
        public float Elevation_value;        
        
        // Values
        public float Range_value;
        
        public float Fire_correction_value;
        
        public float Zoom_value;
        public float Focus_value;
                
        // Enables
        public bool Fire_enabled; // ON/ OFF
        public bool Drive_enabled; // ON/OFF
        public bool Override_enabled; // ON/OFF

        /// <summary>
        /// Default Constructor with default state
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="ccd_ir"></param>
        /// <param name="polarity"></param>
        public VCHState(string mode = VCHStateCodes.SHOOTER,
            string ccd_ir = VCHStateCodes.CCD,
            string polarity = VCHStateCodes.WHITEHOT,
            float azimuth_value = 0.0f,
            float elevation_value = 0.0f,
            float range_value = 0.0f,
            float fire_correction_value = 0.0f,
            float zoom_value = 1.0f,
            float focus_value = 0.0f,
            bool fire_en = false,
            bool drive_en = false,
            bool override_en = false)
        {
            Mode = mode;
            Sensor = ccd_ir;
            Polarity = polarity;
            Azymuth_value = azimuth_value;
            Elevation_value = elevation_value;
            Range_value = range_value;
            Fire_correction_value = fire_correction_value;
            Zoom_value = zoom_value;
            Focus_value = focus_value;
            Fire_enabled = fire_en;
            Drive_enabled = drive_en;
            Override_enabled = override_en;
        }

        /// <summary>
        /// Attempt Mode change 
        /// </summary>
        /// <param name="newMode"></param>
        /// <returns> true iff Mode was changed,
        /// else if newMode is the same as current mode</returns>
        public void SetMode(string newMode)
        {
            if (Mode == newMode) return;
            Mode = newMode;
        }

        /// <summary>
        /// Attempt Sensor Change
        /// </summary>
        /// <param name="newSensor"></param>
        /// <returns> true iff Sensor was changed,
        /// else if newMode is the same as current sensor</returns>
        public bool SetSensor(string newSensor)
        {
            if (Sensor == newSensor) return false;
            Sensor = newSensor;
            return true;
        }

        /// <summary>
        /// Attempt Polarity Change
        /// </summary>
        /// <param name="newPolarity"></param>
        /// <returns> true iff Sensor was changed,
        /// else if newMode is the same as current sensor</returns>
        public bool SetPolarity(string newPolarity)
        {
            if (Polarity == newPolarity) return false;
            Polarity = newPolarity;
            return true;
        }
        
        /// <summary>
        /// Attempt Azymuth Change
        /// </summary>
        /// <param name="newAzymuth"></param>
        /// <returns> true iff Sensor was changed,
        /// else if newMode is the same as current sensor</returns>
        public void SetAzymuth(string newAzymuth)
        {
            Azymuth_Axes = newAzymuth;
            switch (newAzymuth)
            {
                case(VCHStateCodes.RIGHT):
                    Azymuth_value += Azymuth_step;
                    break;
                case(VCHStateCodes.LEFT):
                    Azymuth_value -= Azymuth_step;    
                    break;
                case(VCHStateCodes.NONE):
                    break;
            }
            
        }
        
        
    }
}
    