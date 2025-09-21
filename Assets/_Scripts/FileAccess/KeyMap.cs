using System;
using UnityEngine;

public enum ShooterControl
{
    SHOOTER,
    OBSERVER,
    ENSLAVE
}

public enum TumblerCode
{
    FIRE_EN,
    DRIVE_EN,
    OVVERRIDE,
}

public enum VCHKeyCode
{
    TRACK,
    CCD_IR,
    VIDEO_SOURCE,
    POLARITY,
    ENTER,
    FIRE,
    BRS,
    ZOOM_PUSH,
    ZOOM_UP,
    ZOOM_DOWN,
    ZOOM_LEFT,
    ZOOM_RIGHT,
    FIRE_CORR_UP,
    FIRE_CORR_DOWN,
    FIRE_CORR_LEFT,
    FIRE_CORR_RIGHT,
    MA_RANGE_DOWN,
    MA_RANGE_UP,
    MA_GAIN_PUSH,
    LRF,
}

public enum JoystickControl
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN
}

namespace VCHCommunication
{
    public class KeyMap : MonoBehaviour
    {
        public static KeyMap Instance;
        
        private void Awake()
        {
            Instance = this;
        }

        public Action<VCHKeyCode> OnPressButton;
        public Action<TumblerCode, bool> OnTumbleSwitch;
        public Action<ShooterControl> OnShooterControlChange;
        public Action<JoystickControl> OnJoystickControlChange;
    }
}