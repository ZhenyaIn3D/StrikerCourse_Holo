using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [SerializeReference] private StepName stepToActivate;
    [SerializeField] private GameObject tank;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject sparksEffect;
    private PlayableDirector playableDirector;
    [Inject] private ParticleManager particleManager;
    [Inject] private ScenarioManager scenarioManager;
    [Inject] private InputManager inputManager;
    
    
    private bool played;
    
    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        //tank.SetActive(false);
        //plane.SetActive(false);
        Debug.Log("sc" + (scenarioManager == null));
        //StartSequence();
    }

    public void Update()
    {
        //StartSequence();
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     StartSequence();
        // }
    }

    private void OnEnable()
    {
        
        scenarioManager.OnStepCompleted += StartSequence;
        //inputManager.SubscribeToSpeech("Fire",StartSequence);
        
    }
    
    private void OnDisable()
    {
         scenarioManager.OnStepCompleted -= StartSequence;
         inputManager.UnSubscribeToSpeech("Fire",StartSequence);
    }

    private async void StartSequence(StepName stepName)
    {
        if(stepToActivate!=stepName) return;
        await Task.Delay(10000);
        if(played) return;
        StartSequence();
    }

    public void StartSequence()
    {
        ResetSequence();
        playableDirector.enabled = true;
        playableDirector.Play();
        played = true;
    }
    
    public void StopSequence()
    {
        playableDirector.Stop();
    }
    
    public void PauseSequence()
    {
        playableDirector.Pause();
    }

    public void ResetSequence()
    {
        StopSequence();
        //tank.SetActive(false);
        //plane.SetActive(false);
        //sparksEffect.SetActive(false);
        particleManager.Stop(ParticleEffectType.Tank_SmallFlames);
        particleManager.Stop(ParticleEffectType.Tank_BigFlames);
        particleManager.Stop(ParticleEffectType.Tank_Steam);
        particleManager.Stop(ParticleEffectType.Plane_Smoke);
        particleManager.Stop(ParticleEffectType.Plane_BigFlames);
    }

}
