using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(-1000)]
public static class Injector
{
    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Dictionary<Type, object> registry= new Dictionary<Type, object>();

    
    public static void HandleInjection()
    {
        var providers = FindMonoBehaviours().OfType<IDependencyInjector>();
        foreach (var provider in providers)
        {
            RegisterProvider(provider);
        }

        var injectables = FindMonoBehaviours().Where(IsInjectable);

        foreach (var injectable in injectables)
        {
            Inject(injectable);
        }
        
    }
    


    static void Inject(object instance)
    {
        var type = instance.GetType();
        var injectablesFields =
            type.GetFields(bindingFlags).Where(member => Attribute.IsDefined(member,typeof(InjectAttribute)));

        foreach (var injectableField in injectablesFields)
        {
            var fieldType = injectableField.FieldType;
            var resolvedInstance = Resolve(fieldType);
            if (resolvedInstance == null)
            {
                throw new Exception($"Failed to resolve {fieldType.Name} for type {type.Name}");
            } 
            
            injectableField.SetValue(instance,resolvedInstance);
        }
        
        var injectablesProperties =
            type.GetProperties(bindingFlags).Where(member => Attribute.IsDefined(member,typeof(InjectAttribute)));
        
        
        foreach (var injectableProperties in injectablesProperties)
        {
            var propertyType = injectableProperties.PropertyType;
            var resolvedInstance = Resolve(propertyType);
            if (resolvedInstance == null)
            {
                throw new Exception($"Failed to resolve {propertyType.Name} for type {type.Name}");
            } 
            
            injectableProperties.SetValue(instance,resolvedInstance);
        }

        var injectableMethods = type.GetMethods(bindingFlags)
            .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

        foreach (var injectableMethod in injectableMethods)
        {
            var requiredParameter =
                injectableMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var resolvedInstance = requiredParameter.Select(Resolve).ToArray();
            if(resolvedInstance.Any(resolvedInstance=>resolvedInstance==null))
            {
                throw new Exception($"Failed to inject {type.Name}.{injectableMethod.Name}");
            }
            
            injectableMethod.Invoke(instance, resolvedInstance);
        }
    }
    
    static object Resolve(Type type)
    {
        registry.TryGetValue(type, out var resolvedInstance);
        return resolvedInstance;
    }

    static void RegisterProvider(IDependencyInjector provider)
    {
        var methods = provider.GetType().GetMethods(bindingFlags);

        foreach (var method in methods)
        {
            if(!Attribute.IsDefined(method,typeof(ProvideAttribute))) continue;

            var returnType = method.ReturnType;
            var providerInstance = method.Invoke(provider, null);
            if (providerInstance == null)
            {
                throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
            }
            
            registry.Add(returnType,providerInstance);
        }
    }

    static bool IsInjectable(MonoBehaviour obj)
    {
        var members = obj.GetType().GetMembers(bindingFlags);
        return members.Any(member => member.IsDefined(typeof(InjectAttribute)));
    }
    
  

    static MonoBehaviour[] FindMonoBehaviours()
    {
        return Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None);
    }
    
}
