using System;
using Unity.VisualScripting;
using UnityEngine;
namespace VCHStateMachine
{
    public enum MARangeControlKey
    {
        LESS,
        MORE,
        NONE
    }
    public abstract class MARangeControlState : VCHControlState
    {
        protected MARangeControlKey maRangeControlKey;

        public MARangeControlKey MARangeControlKey => maRangeControlKey;
        
        public static Action<MARangeControlKey> MARangeControlEntered;
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State:" + maRangeControlKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            MARangeControlEntered.Invoke(maRangeControlKey);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}