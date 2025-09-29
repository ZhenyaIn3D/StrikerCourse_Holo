using System;
using System.Collections;
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
        public GameObject SuccessSignal;
        void Start()
        {
            onSuccess += InterpretVCHState;
            onSuccess += ShowConnectSignal;
            timeTillUpdate = updateCycle;
            SuccessSignal.SetActive(false);
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
            ReportVCHState(_vchState);
        }

        public void ShowConnectSignal(string vchState)
        {
            StartCoroutine(ShowConnectSignalCo());
        }

        private IEnumerator ShowConnectSignalCo()
        {
            SuccessSignal.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            SuccessSignal.SetActive(false);
            
        }
        
    }
}

