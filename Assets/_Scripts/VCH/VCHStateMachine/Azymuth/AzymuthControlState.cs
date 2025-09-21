using System;
using UnityEngine;

namespace VCHStateMachine
{
    public enum AzymuthControlKey
    {
        LEFT,
        RIGHT,
        NONE
    }
    
    public abstract class AzymuthControlState: VCHControlState
    {
        protected AzymuthControlKey azymuthKey;
        public AzymuthControlKey AzymuthControlKey => azymuthKey;
        
        public static Action<AzymuthControlKey> AzymuthControlEntered;

        protected float azymuthValue;
        public float AzymuthValue => azymuthValue;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Enters State:" + azymuthKey.ToString());
            
            // Tell Visual representation of Display to change text of Mode to this state
            AzymuthControlEntered.Invoke(azymuthKey);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}