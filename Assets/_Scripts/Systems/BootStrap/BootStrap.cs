using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class BootStrap : MonoBehaviour
{
   [SerializeField] private ProviderFactoryBase providerFactory;

   private ProviderBase provider;

   private void Awake()
   {
      provider=providerFactory.CreateProvider(gameObject);
      Injector.HandleInjection();
      
   }
   
}
