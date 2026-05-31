using UnityEngine;

public class JournalInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        JournalUI.Instance.Toggle();
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor;
    }
}
