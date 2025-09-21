using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    IEnumerator Start()
    { 
        yield  return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(1);
    }
    
}
