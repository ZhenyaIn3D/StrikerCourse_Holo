using CustomSoundSystem;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/StepConfig")]

public class StepConfig : ScriptableObject
{
    public StepName stepName;
    public int numOfButtons;
    public MenuActions[] actions;
    public string[] buttonTexts;
    public string title;
    public string text;
    public ScanTargetType[] scanPriority;
    public float waitTimeBetweenPriorities;
    public bool hasSound;
    public SoundTypeKeys SoundTypeKeys;
    public string[] optionaltexts;
    
}
