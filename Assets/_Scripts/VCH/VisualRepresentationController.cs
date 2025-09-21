using UnityEngine;

namespace VCHStateMachine
{
    public class VisualRepresentationController : MonoBehaviour
    {
        [SerializeField] GameObject[] joystick_lightIndicators;
        [SerializeField] GameObject[] joystick_shooterControl_textIndicators;
        [SerializeField] GameObject[] screen_shooterControl_textIndicators;

        private void Awake()
        {
            ModeControlState.ModeEntered += ShowMode;
        }

        public void ShowMode(ModeControlKey mode)
        {
            Debug.Log($"Updates Visual representation of {mode}");
            TurnOnLightIndicator(mode);
            TurnOnTextIndicator(mode);
            TurnOnScreenModeIndicator(mode);
        }

        public void ShowAzymuth(AzymuthControlKey azymuth)
        {
            
        }
        public void ShowAElevation(ElevationControlKey elevation)
        {
            
        }
        
        #region Mode Light
        void TurnOnLightIndicator(ModeControlKey mode)
        {
            switch (mode)
            {
                case ModeControlKey.SHOOTER:
                    SetLightModeIndicator(0, true);
                    SetLightModeIndicator(1, false);
                    SetLightModeIndicator(2, false);
                    break;
                case ModeControlKey.OBSERVER:
                    SetLightModeIndicator(0, false);
                    SetLightModeIndicator(1, true);
                    SetLightModeIndicator(2, false);
                    break;
                case ModeControlKey.ENSLAVE:
                    Debug.Log("ENSLAVE");
                    SetLightModeIndicator(0, false);
                    SetLightModeIndicator(1, false);
                    SetLightModeIndicator(2, true);
                    break;
            }
        }

        void SetLightModeIndicator(int index, bool isOn)
        {
            Debug.Log(index + " is on " + isOn);
            joystick_lightIndicators[index].SetActive(isOn);
        }
        #endregion
        
        #region Mode Text
        void TurnOnTextIndicator(ModeControlKey mode)
        {
            switch (mode)
            {
                case ModeControlKey.SHOOTER:
                    SetTextModeIndicator(0, true);
                    SetTextModeIndicator(1, false);
                    SetTextModeIndicator(2, false);
                    break;
                case ModeControlKey.OBSERVER:
                    SetTextModeIndicator(0, false);
                    SetTextModeIndicator(1, true);
                    SetTextModeIndicator(2, false);
                    break;
                case ModeControlKey.ENSLAVE:
                    SetTextModeIndicator(0, false);
                    SetTextModeIndicator(1, false);
                    SetTextModeIndicator(2, true);
                    break;
            }
        }

        void SetTextModeIndicator(int index, bool isOn)
        {
            joystick_shooterControl_textIndicators[index].SetActive(isOn);
        }
        #endregion
        
        #region Mode Screen Text
        void TurnOnScreenModeIndicator(ModeControlKey mode)
        {
            switch (mode)
            {
                case ModeControlKey.SHOOTER:
                    SetScreenModeIndicator(0, true);
                    SetScreenModeIndicator(1, false);
                    SetScreenModeIndicator(2, false);
                    break;
                case ModeControlKey.OBSERVER:
                    SetScreenModeIndicator(0, false);
                    SetScreenModeIndicator(1, true);
                    SetScreenModeIndicator(2, false);
                    break;
                case ModeControlKey.ENSLAVE:
                    SetScreenModeIndicator(0, false);
                    SetScreenModeIndicator(1, false);
                    SetScreenModeIndicator(2, true);
                    break;
            }
        }
        
        void SetScreenModeIndicator(int index, bool isOn)
        {
            screen_shooterControl_textIndicators[index].SetActive(isOn);
        }
        #endregion

        
    }
}