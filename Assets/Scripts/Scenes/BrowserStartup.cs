using UnityEngine;
using System.Collections.Generic;

public class BrowserStartup : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToDeactivate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
       foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                Debug.Log("Deactivating " + obj.name);
                obj.SetActive(false);
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
