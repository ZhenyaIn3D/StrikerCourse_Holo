using System.Collections;
using System.Collections.Generic;
using CustomSoundSystem.RunTime;
using UnityEngine;

public class DefaultProvider : ProviderBase
{
 
    [Provide]
    public SoundSystem ProvideSoundSystem()
    {
        var service = FindAnyObjectByType<SoundSystem>();
        if (service == null)
        {
            service = HandleInitialization<SoundSystem>();
        }
        return service;  
    }
    
    [Provide]
    public InputManager ProvideInputManager()
    {
        var service = FindAnyObjectByType<InputManager>();
        if (service == null)
        {
            service = HandleInitialization<InputManager>();
        }
        return service;  
    }
    
    [Provide]
    public JoystickMessageSystem ProvideJoystickMessageSystem()
    {
        var service = FindAnyObjectByType<JoystickMessageSystem>();
        if (service == null)
        {
            service = HandleInitialization<JoystickMessageSystem>();
        }
        return service;  
    }
    
    
    [Provide]
    public ScreenMessageSystem ProvideScreenMessageSystem()
    {
        var service = FindAnyObjectByType<ScreenMessageSystem>();
        if (service == null)
        {
            service = HandleInitialization<ScreenMessageSystem>();
        }
        return service;  
    }
    
    [Provide]
    public ScreenController ProvideScreenControllerSystem()
    {
        var service = FindAnyObjectByType<ScreenController>();
        if (service == null)
        {
            service = HandleInitialization<ScreenController>();
        }
        return service;  
    }
    
    [Provide]
    public JoystickController ProvideJoystickControllerSystem()
    {
        var service = FindAnyObjectByType<JoystickController>();
        if (service == null)
        {
            service = HandleInitialization<JoystickController>();
        }
        return service;  
    }
    
    [Provide]
    public ScansManager ProvideScanManagerSystem()
    {
        var service = FindAnyObjectByType<ScansManager>();
        if (service == null)
        {
            service = HandleInitialization<ScansManager>();
        }
        return service;  
    }
    
    
    
    
}
