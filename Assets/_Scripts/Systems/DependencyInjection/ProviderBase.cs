using UnityEngine;

public abstract class ProviderBase : MonoBehaviour,  IDependencyInjector
{
    
    protected T HandleInitialization<T>() where T : Component
    {
        var go = new GameObject();
        var service = go.AddComponent<T>();
        return service;
    }
    
     
}