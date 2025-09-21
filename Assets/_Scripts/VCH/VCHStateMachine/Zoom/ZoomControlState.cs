using System;
using Unity.VisualScripting;
using UnityEngine;

namespace VCHStateMachine
{
    public enum ZoomControlKey
    {
        LESS,
        MORE,
        NONE
    }
    public abstract class ZoomControlState : VCHControlState
    {
        protected ZoomControlKey zoomControlKey;

        public ZoomControlKey ZoomControlKey => zoomControlKey;
        
        public static Action<ZoomControlKey> ZoomControlEntered;
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State:" + zoomControlKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            ZoomControlEntered.Invoke(zoomControlKey);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}