using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferecesTest : MonoBehaviour
{
    [Inject] private GlobalReferences gr;
    private MeshRenderer mr;

    IEnumerator Start()
    {
        mr = GetComponent<MeshRenderer>();

        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.RegularBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.HighlightBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.RegularBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.HighlightBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.RegularBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.HighlightBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.RegularBackground);
        
        yield return new WaitForSeconds(1);
        mr.sharedMaterial = (Material)gr.references.GetReference(ReferencesKeys.HighlightBackground);
    }
}
