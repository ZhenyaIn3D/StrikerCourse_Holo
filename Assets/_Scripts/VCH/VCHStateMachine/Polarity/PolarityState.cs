using System;
using UnityEngine;

namespace VCHStateMachine
{
    public enum PolarityControlKey
    {
        WHITEHOT,
        BLACKHOT
    }
    public class PolarityState: VCHControlState
    {
        protected PolarityControlKey polarityKey;

        public PolarityControlKey PolarityKey => polarityKey;
        
        public static Action<PolarityControlKey> PolarityChanged;
        
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Polarity Changed Activates:" + polarityKey.ToString());
            
        }

        public override void OnExit()
        {
            base.OnExit();

        }
    }
}