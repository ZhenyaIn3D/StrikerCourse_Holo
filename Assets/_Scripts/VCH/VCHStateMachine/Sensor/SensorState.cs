using System;
using UnityEngine;

namespace VCHStateMachine
{
    public enum SensorControlKey
    {
        CCD,
        IR
    }
    public class SensorState :VCHControlState
    {
        protected SensorControlKey sensorKey;

        public SensorControlKey SensorKey => sensorKey;
        
        public static Action<SensorControlKey> SensorChanged;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Sensor Activates:" + sensorKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            //SensorChanged.Invoke(sensorKey);
        }

        public override void OnExit()
        {
            base.OnExit();

        }
    }
}