using System.Collections.Generic;
using UnityEngine;


public class MaterialReferences : IRefernces
{
    private Dictionary<ReferencesKeys, Material> materialReferences;
    public MaterialReferences( ReferencesData data)
    {
        materialReferences = new Dictionary<ReferencesKeys, Material>();
        for (int i = 0; i < data.keys.Length; i++)
        {
            SettReference(data.keys[i], data.assets[i]);
        }
    }
    
    public Object GetReference(ReferencesKeys key)
    {
        if (materialReferences.ContainsKey(key))
        {
            return materialReferences[key];
        }

        return null;
    }

    public void SettReference(ReferencesKeys key, Object asset)
    {
        if (!materialReferences.ContainsKey(key))
        {
            materialReferences.Add(key,(Material)asset);
        }
    }
}
