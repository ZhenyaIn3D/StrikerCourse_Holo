using UnityEngine;

public interface IRefernces
{
    public static IRefernces InitializeDefault(ReferencesData data) => new MaterialReferences(data);

    public Object GetReference(ReferencesKeys key);
    public void SettReference(ReferencesKeys key,Object asset);

}
