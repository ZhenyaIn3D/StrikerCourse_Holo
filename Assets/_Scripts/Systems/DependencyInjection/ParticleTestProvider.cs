

public class ParticleTestProvider : ProviderBase
{
    [Provide]
    public ParticleManager ProvideSoundSystem()
    {
        var service = FindAnyObjectByType<ParticleManager>();
        if (service == null)
        {
            service = HandleInitialization<ParticleManager>();
        }
        return service;  
    }
    
    [Provide]
    public TimelineController ProvideTimelineManager()
    {
        var service = FindAnyObjectByType<TimelineController>();
        if (service == null)
        {
            service = HandleInitialization<TimelineController>();
        }
        return service;  
    }
}
