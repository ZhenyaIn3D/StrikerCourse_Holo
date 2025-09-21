using System;
using UnityEngine;
namespace VCHStateMachine
{
    public enum ElevationControlKey
    {
        UP,
        DOWN,
        NONE
    }
    
    public abstract class ElevationControlState: VCHControlState
    {
        protected ElevationControlKey elevationKey;
        public ElevationControlKey ElevationControlKey => elevationKey;
        
        public static Action<ElevationControlKey> ElevationControlEntered;

        protected float elevationValue;
        public float ElevationValue => elevationValue;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State: " + elevationKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            ElevationControlEntered.Invoke(elevationKey);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}