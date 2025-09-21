using System;
using Unity.VisualScripting;
using UnityEngine;
namespace VCHStateMachine
{
    public enum FocusControlKey
    {
        LESS,
        MORE,
        NONE
    }
    public abstract class FocusControlState : VCHControlState
    {
        protected FocusControlKey focusControlKey;

        public FocusControlKey FocusControlKey => focusControlKey;
        
        public static Action<FocusControlKey> FocusControlEntered;
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State:" + focusControlKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            FocusControlEntered.Invoke(focusControlKey);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}