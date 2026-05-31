using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BrowserTabInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private Material activeTabMaterial;
    [SerializeField] private Material inactiveTabMaterial;
    [SerializeField] private GameObject contentToActivate;
    [SerializeField] private GameObject tabFrame;
    [SerializeField] private List<GameObject> contentToDeactivate;
    [SerializeField] private List<GameObject> tabsToDeactivate;

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (contentToActivate != null)
        {
            contentToActivate.SetActive(true);
            tabFrame.GetComponent<Renderer>().material = activeTabMaterial;
        }
        foreach (GameObject obj in contentToDeactivate)
        {
            if (obj != null)
            {
                Debug.Log("Deactivating " + obj.name);
                obj.SetActive(false);
            }
        }
        foreach (GameObject tab in tabsToDeactivate)
        {
            if (tab != null)
            {
                Debug.Log("Setting " + tab.name + " to inactive material");
                Renderer tabRenderer = tab.GetComponent<Renderer>();
                if (tabRenderer != null)
                {
                    tabRenderer.material = inactiveTabMaterial;
                }
                else
                {
                    Debug.LogWarning("No Renderer found on " + tab.name);
                }
            }
        }
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor;
    }

    
}
