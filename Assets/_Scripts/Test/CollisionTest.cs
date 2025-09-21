using CustomSoundSystem;
using CustomSoundSystem.RunTime;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    [Inject] private SoundSystem soundSystem;
    public void PlayPolarity()
    {
        soundSystem.Play(SoundSourceKeys.Scan.ToString(),SoundTypeKeys.Polarity.ToString());
    }
    
    public void PlayTrack()
    {
        soundSystem.Play(SoundSourceKeys.Scan.ToString(),SoundTypeKeys.Track.ToString());
    }
    
    public void PlayVideoSource()
    {
        soundSystem.Play(SoundSourceKeys.Scan.ToString(),SoundTypeKeys.VideoSource.ToString());
    }
    
    public void PlayCCDIR()
    {
        soundSystem.Play(SoundSourceKeys.Scan.ToString(),SoundTypeKeys.CCDIR.ToString());
    }
}
