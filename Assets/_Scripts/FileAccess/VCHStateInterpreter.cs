using System;
using System.Collections.Generic;
using UnityEngine;

namespace FileAccess
{
    public class VCHStateInterpreter: MonoBehaviour
    {
        public float updateCycle = 1f;
        float timeTillUpdate;
        private Action<string> onSuccess;
        private VCHState _vchState;
        public Action<VCHState> ReportVCHState;
        private Dictionary<StepName, Func<bool>> endConditions;
        void Start()
        {
            onSuccess += InterpretVCHState;
            timeTillUpdate = updateCycle;     
        }
        
        void Update()
        {
            timeTillUpdate -= Time.deltaTime;
            if ( timeTillUpdate < 0 )
            {
                timeTillUpdate = updateCycle;
                VCHLogAccess.GetVCHLogContent(onSuccess);
            }
        }

        public void InterpretVCHState(string vchState)
        {
            _vchState = JsonUtility.FromJson<VCHState>(vchState);
            //Debug.Log("interpreted got " + _vchState.Mode);
            ReportVCHState(_vchState);
        }
        
    }
}

