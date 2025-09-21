using UnityEngine;

[CreateAssetMenu(menuName = "Factory/References", fileName = "Reference Factory")]
public class GloabalReferenceFactory : ScriptableObject
{
    public ReferencesFactoryBase referenceFactory;
    public ReferencesData data;

    public IRefernces CreateReferences() => referenceFactory != null
        ? referenceFactory.CreateGlobalReference(data) : IRefernces.InitializeDefault(data);

}
