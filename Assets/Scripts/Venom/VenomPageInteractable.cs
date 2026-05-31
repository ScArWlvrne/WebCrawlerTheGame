using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switches active Venom UI pages (login, forgot-password, etc.) like BrowserTabInteractable.
/// </summary>
public class VenomPageInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private GameObject contentToActivate;
    [SerializeField] private List<GameObject> contentToDeactivate = new List<GameObject>();

    public void Interact()
    {
        Debug.Log("VenomPageInteractable.Interact() on " + gameObject.name);

        if (contentToActivate != null)
            contentToActivate.SetActive(true);

        foreach (GameObject obj in contentToDeactivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    public void Configure(Transform anchor, GameObject activate, GameObject[] deactivate)
    {
        promptAnchor = anchor;
        contentToActivate = activate;
        contentToDeactivate = deactivate != null
            ? new List<GameObject>(deactivate)
            : new List<GameObject>();
    }
}
