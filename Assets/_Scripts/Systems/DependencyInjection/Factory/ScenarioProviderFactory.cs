using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Factory/provider/Scenario Provider Factory", fileName = "Scenario Provider Factory")]
public class ScenarioProviderFactory : ProviderFactoryBase
{
    public override ProviderBase CreateProvider(GameObject requester)
    {
        return requester.AddComponent<ScenarionSceneProvider>();
    }
}
