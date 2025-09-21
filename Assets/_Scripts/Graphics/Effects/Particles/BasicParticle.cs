using System;
using UnityEngine;

public class BasicParticle : ParticleBase
{
    [Inject] private ParticleManager particleManager;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleManager.Subscribe(particleEffectType,this);
        particleSystem.Stop();
        gameObject.SetActive(false);
    }

    public override void Play()
    {
        gameObject.SetActive(true);
        particleSystem.Play();
    }

    public override void Stop()
    {
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
    }
}
