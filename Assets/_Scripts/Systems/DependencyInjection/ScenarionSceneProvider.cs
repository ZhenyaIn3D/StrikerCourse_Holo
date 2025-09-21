using CustomSoundSystem.RunTime;


public class ScenarionSceneProvider : ProviderBase
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
        public ScansManager ProvideScanManagerSystem()
        {
            var service = FindAnyObjectByType<ScansManager>();
            if (service == null)
            {
                service = HandleInitialization<ScansManager>();
            }
            return service;  
        }
        
        [Provide]
        public ParticleManager ProvideParticleSystem()
        {
            var service = FindAnyObjectByType<ParticleManager>();
            if (service == null)
            {
                service = HandleInitialization<ParticleManager>();
            }
            return service;  
        }
        
        
            
        [Provide]
        public ScenarioManager ProvideScenarioManager()
        {
            var service = FindAnyObjectByType<ScenarioManager>();
            if (service == null)
            {
                service = HandleInitialization<ScenarioManager>();
            }
            return service;  
        }
        
        [Provide]
        public GlobalReferences ProvideGlobalReferences()
        {
            var service = FindAnyObjectByType<GlobalReferences>();
            if (service == null)
            {
                service = HandleInitialization<GlobalReferences>();
            }
            return service;  
        }
        
        [Provide]
        public TimelineController ProvideTimelineController()
        {
            var service = FindAnyObjectByType<TimelineController>();
            if (service == null)
            {
                service = HandleInitialization<TimelineController>();
            }
            return service;  
        }


    
        
        
        
        
}
