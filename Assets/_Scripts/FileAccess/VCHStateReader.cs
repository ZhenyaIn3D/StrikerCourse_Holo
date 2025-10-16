using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
namespace FileAccess
{
    public class VCHStateReader: MonoBehaviour
    {
        public float updateCycle = 1f;
        public TextMeshProUGUI textState;
        float timeTillUpdate;
        private Action<string> onSuccess;
        private VCHState _vchState;
        public Action<VCHState> ReportVCHState;
        private Dictionary<StepName, Func<bool>> endConditions;
        public GameObject SuccessSignal;
        
        private bool _isInitialized = false;
        private bool _isInitializing = false;
        async void Start()
        {
            onSuccess += InterpretVCHState;
            timeTillUpdate = updateCycle;
            SuccessSignal.SetActive(false);
            await InitializeAsync();
        }
        
        void Update()
        {
            // timeTillUpdate -= Time.deltaTime;
            //
            // if ( timeTillUpdate < 0 )
            // {
            //     
            //     timeTillUpdate = updateCycle;
            //     VCHLogAccess.GetVCHLogContent(onSuccess);
            // }
        }
        
        private async Task InitializeAsync()
        {
            if (_isInitializing || _isInitialized) return;
        
            _isInitializing = true;
        
            try
            {
                await VCHLogAccess.InitializeAsync("vchState.txt");
                _isInitialized = true;
                Debug.Log("VCH Log initialized successfully");
            
                // Start reading only after successful initialization
                InvokeRepeating(nameof(ReadVCHLogContent), 0f, updateCycle);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize VCH Log: {ex.Message}");
            }
            finally
            {
                _isInitializing = false;
            }
        }
        
        void ReadVCHLogContent()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("VCH Log not initialized yet");
                return;
            }

            VCHLogAccess.GetVCHLogContent(
                onSuccess,
                onError: ex => textState.text  = $"Error: {ex.Message}"
            ); 
            
        }
        
        public void InterpretVCHState(string vchState)
        {
            textState.text = "Success";
            _vchState = JsonUtility.FromJson<VCHState>(vchState);
            ReportVCHState.Invoke(_vchState);;
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
        
        void OnDestroy()
        {
            CancelInvoke(nameof(ReadVCHLogContent));
        }
        
    }
}

