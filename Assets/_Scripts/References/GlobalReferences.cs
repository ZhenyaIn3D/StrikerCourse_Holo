using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    [SerializeField] private GloabalReferenceFactory factory;

    public IRefernces references { get; private set; }
    private void Start()
    {
        references = factory.CreateReferences();
    }
}
