using UnityEngine;

public abstract class ParticleBase :MonoBehaviour
{
    [SerializeField] protected ParticleEffectType particleEffectType;
    protected ParticleSystem particleSystem;
    public abstract void Play();
    public abstract void Stop();

}
