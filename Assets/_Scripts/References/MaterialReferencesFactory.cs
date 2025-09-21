using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Factory/MaterialReferencesFactory", fileName = "Material References Factory")]
public class MaterialReferencesFactory : ReferencesFactoryBase
{
    public override IRefernces CreateGlobalReference(ReferencesData data)
    {
        return new MaterialReferences(data);
    }
}
