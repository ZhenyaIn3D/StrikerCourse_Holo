using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProviderFactoryBase : ScriptableObject
{
    public abstract ProviderBase CreateProvider(GameObject requester);

}
