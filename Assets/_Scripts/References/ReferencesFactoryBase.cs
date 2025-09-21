using UnityEngine;

public abstract class ReferencesFactoryBase : ScriptableObject
{
    public abstract IRefernces CreateGlobalReference(ReferencesData data);
    
}
