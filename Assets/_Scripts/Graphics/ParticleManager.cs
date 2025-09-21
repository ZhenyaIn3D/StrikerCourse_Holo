using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private Dictionary<ParticleEffectType, ParticleBase> particles= new Dictionary<ParticleEffectType, ParticleBase>();
    

    public void Subscribe(ParticleEffectType particleEffectType, ParticleBase particleBaseSystem)
    {
        particles.TryAdd(particleEffectType, particleBaseSystem);
    }

    public void Play(ParticleEffectType particleEffectType)
    {
        if(!particles.ContainsKey(particleEffectType)) return;
        particles[particleEffectType].Play();
    }

    public void Stop(ParticleEffectType particleEffectType)
    {
        if(!particles.ContainsKey(particleEffectType)) return;
        particles[particleEffectType].Stop();
    }
}

public enum ParticleEffectType
{
    TrailSparks,
    Trails,
    Tank_Smoke,
    Tank_SmallFlames,
    Tank_Explosion,
    Tank_BigFlames,
    Sparks,
    Tank_Steam,
    Plane_SmallFlames,
    Plane_Explosion,
    Plane_BigFlames,
    Plane_Smoke
}
