using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using CustomSoundSystem;
using CustomSoundSystem.RunTime;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ScansManager : MonoBehaviour
{
    [SerializeField] private VuforiaBehaviour vuforiaBehaviour;
    [SerializeField] private UnityEngine.UI.Image soundImage;
    
    [Header("Buttons")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material notActiveMaterial;
    [SerializeField] private RawImage[] buttonBackgroundMR;
    
    [Header("Scan targets")]
    [SerializeField] private ScanTransformAdjustment[] transfomAdjusments;
    [SerializeField] private ScanTargetControllerBase[] scanControllers;
    [SerializeField] private ScalableDataSetTrackableBehaviour[] vuforiaScanTargets;
    
    [Header("Menu")]
    [SerializeField] private Sprite[] soundIcons;
    [SerializeField] private Transform menuParent;
    
    [Inject] private SoundSystem soundSystem;
    [Inject] private InputManager inputManager;
    private CountdownTimer timer;
    private Follow follow;
    private bool[] scanStatus;
    private bool mute;
    
    public event Action<ScanTargetType> OnFinishScan;
    public event Action OnAllScanFound;
    public event Action OnStartScan;
    
    private void Awake()
    {
        follow = GetComponentInChildren<Follow>();
        timer = new CountdownTimer(0.2f);
        scanStatus = new bool[transfomAdjusments.Length];
        for (var index = 0; index < scanStatus.Length; index++)
        {
            scanStatus[index] = false;
        }
        
        SubscribeToScanStatus();
    }

    IEnumerator  Start()
    {
        yield return new WaitForSeconds(2);
        
        soundSystem.Play(SoundSourceKeys.Narration.ToString(),SoundTypeKeys.Introduction.ToString());
        
        follow.enabled=false;
        
        inputManager.SubscribeToSpeech("Sound", StartNarrationAndAdjustUI);
        inputManager.SubscribeToSpeech("Mute", StopNarrationAndAdjustUI);
        
    }

    private void OnDestroy()
    {
        inputManager.UnSubscribeToSpeech("Sound", StartNarrationAndAdjustUI);
        inputManager.UnSubscribeToSpeech("Mute", StopNarrationAndAdjustUI);
    }
    

    private void SubscribeToScanStatus()
    {

        for (int i = 0; i < transfomAdjusments.Length; i++)
        {
            var index = i;
            transfomAdjusments[i].OnFinishScan += (ScanTargetType st)=>RegisterScanStatus(st,false);
            transfomAdjusments[i].OnFinishScan += NotifyFinishedScan;
            transfomAdjusments[i].OnFinishScan += (ScanTargetType st)=>ChangeMaterial(buttonBackgroundMR[index],false);
        }
    }
    

    private void NotifyFinishedScan(ScanTargetType scanTargetType)
    {
        OnFinishScan?.Invoke(scanTargetType);
    }


    public void StartScan(int scanTarget)
    {
        RegisterScanStatus(scanTarget, true);
        
        OnStartScan?.Invoke();
    }
    
    public void StartScan(ScanTargetType targetType)
    {
        RegisterScanStatus(targetType, true);
        
        OnStartScan?.Invoke();
    }

    public void ToggleVuforiaBehaviour()
    {
        vuforiaBehaviour.enabled = IsScanActive();
    }

    private void ChangeMaterial(RawImage mr, bool active)
    {
        mr.material = active ? activeMaterial : notActiveMaterial;
    }

    private void RegisterScanStatus(ScanTargetType targetType, bool active)
    {
        var index = (int)targetType;
        if (active)
        {
            var soundKey = targetType == ScanTargetType.Joystick
                ? SoundTypeKeys.Narration_LookAtJoystic
                : SoundTypeKeys.Narration_LookAtDisplayScreen;
            soundSystem.Play(SoundSourceKeys.Narration.ToString(),soundKey.ToString());
            soundSystem.Play(SoundSourceKeys.ScanLoop.ToString(),SoundTypeKeys.Ding.ToString());
        }
        else
        {
            soundSystem.Stop(SoundSourceKeys.ScanLoop.ToString());
        }
        scanStatus[index] = active;
        HandelVuforiaScanGO(index, active);
        HandleScanTargetsGO(index,active);
        HandleScanResultsGO(index,active);
        ChangeMaterial(buttonBackgroundMR[index],active);
        ToggleVuforiaBehaviour();
    }
    
    private void RegisterScanStatus(int target, bool active)
    {
        if (active)
        {
            var soundKey = target == 0
                ? SoundTypeKeys.Narration_LookAtJoystic
                : SoundTypeKeys.Narration_LookAtDisplayScreen;
            soundSystem.Play(SoundSourceKeys.Narration.ToString(),soundKey.ToString());
            soundSystem.Play(SoundSourceKeys.ScanLoop.ToString(),SoundTypeKeys.Ding.ToString());
        }
        else
        {
            soundSystem.Stop(SoundSourceKeys.ScanLoop.ToString());
        }
        scanStatus[target] = active;
        HandelVuforiaScanGO(target, active);
        HandleScanTargetsGO(target,active);
        HandleScanResultsGO(target,active);
        ChangeMaterial(buttonBackgroundMR[target],active);
        ToggleVuforiaBehaviour();
    }


    private void HandelVuforiaScanGO(int index, bool active)
    {
        vuforiaScanTargets[index].gameObject.SetActive(active);
    }

    private void HandleScanResultsGO(int index, bool scanActive)
    {
        var scanActiveNum = Mathf.Clamp(index, 0, scanControllers.Length - 1);
        if (scanActive)
        {
            scanControllers[scanActiveNum].HideAll();
        }
        else
        {
            soundSystem.Play(SoundSourceKeys.Narration.ToString(),SoundTypeKeys.ScanSuccessful.ToString());
            soundSystem.Play(SoundSourceKeys.Scan.ToString(),SoundTypeKeys.ScanFinished.ToString());
            scanControllers[scanActiveNum].ShowAll();
        }
    }
    
    private void HandleScanTargetsGO(int index, bool scanActive)
    {
        var scanActiveNum = Mathf.Clamp(index, 0, scanControllers.Length - 1);
        if (scanActive)
        {
            scanControllers[scanActiveNum].ActivateScanTarget((ScanTargetType)index);
        }
        else
        {
            if (scanControllers[scanActiveNum].DeActivateScanTarget((ScanTargetType)index))
            {
                OnAllScanFound?.Invoke();
            }
        }
    }

    private bool IsScanActive()
    {
        return scanStatus.Any(s => s);
    }

    public void ToggleSound()
    {
        if (mute)
        {
            StartNarrationAndAdjustUI();
        }
        else
        {
            StopNarrationAndAdjustUI();
        }
        
    }

    private void StartNarrationAndAdjustUI()
    {
        soundImage.sprite = soundIcons[0];
        soundSystem.Play(SoundSourceKeys.Narration.ToString(), SoundTypeKeys.Introduction.ToString());
        mute = false;
    }

    private void StopNarrationAndAdjustUI()
    {
        soundImage.sprite = soundIcons[1];
        soundSystem.Stop(SoundSourceKeys.Narration.ToString());
        mute=true;
    }


    public void ToggleMenu()
    {
        if (menuParent.gameObject.activeInHierarchy)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }
    
    private async void ShowMenu()
    {
        menuParent.gameObject.SetActive(true);
        await AnimateMenu(menuParent, true);
    }

    private async void HideMenu()
    {
        await AnimateMenu(menuParent, false);
        menuParent.gameObject.SetActive(false);
    }

    private async Task AnimateMenu(Transform t, bool show)
    {
        timer.Start();
        var start = show ? Vector3.one*0.01f: Vector3.one;
        var target = show ? Vector3.one : Vector3.one*0.01f;

        while (!timer.IsFinished)
        {
            t.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }
    }
    
    
}

public enum ScanTargetType
{
    Joystick,
    Screen
}
