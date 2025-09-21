using UnityEngine;


/// <summary>
/// This script is a way to access the particle logic through the animator
/// </summary>
public class ParticleManagerWrapper : MonoBehaviour
{
    [Inject] private ParticleManager particleManager;

    public void Play(ParticleEffectType particleEffectType)
    {
        particleManager.Play(particleEffectType);
    }
    
    public void Stop(ParticleEffectType particleEffectType)
    {
        particleManager.Stop(particleEffectType);
    }
}
