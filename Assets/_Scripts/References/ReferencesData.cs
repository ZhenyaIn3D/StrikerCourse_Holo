using UnityEngine;

[CreateAssetMenu(menuName = "Data/References",fileName = "References")]
public class ReferencesData : ScriptableObject
{
    public ReferencesKeys[] keys;
    public UnityEngine.Object[] assets;
}
