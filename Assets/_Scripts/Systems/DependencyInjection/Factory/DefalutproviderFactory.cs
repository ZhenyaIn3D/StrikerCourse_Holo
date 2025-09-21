
using UnityEngine;


[CreateAssetMenu(menuName = "Factory/provider/Default Provider", fileName = "DefaultProvider")]
public class DefalutproviderFactory : ProviderFactoryBase
{
    public override ProviderBase CreateProvider(GameObject requester)
    {
        return requester.AddComponent<DefaultProvider>();
    }
}
