using UnityEngine;


[CreateAssetMenu(menuName = "Factory/provider/particle Test Provider Factory", fileName = "Particle Test Provider Factory")]
public class ParticleProviderFactory : ProviderFactoryBase
{
    public override ProviderBase CreateProvider(GameObject requester)
    {
        return requester.AddComponent<ParticleTestProvider>();
    }
}
