using System;
using UnityEngine;

namespace VCHStateMachine
{
    public enum ModeControlKey
    {
        SHOOTER,
        OBSERVER,
        ENSLAVE
    }
    public abstract class ModeControlState : VCHControlState
    {
        public bool LightIndicatorOn;
        protected ModeControlKey modeKey;

        public ModeControlKey ModeKey => modeKey;
        
        public static Action<ModeControlKey> ModeEntered;
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State:" + modeKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            ModeEntered.Invoke(modeKey);
        }

        public override void OnExit()
        {
            base.OnExit();
            LightIndicatorOn = false;
        }
    }
}