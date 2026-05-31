using UnityEngine;

public class JournalOpenInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;

    public void Interact()
    {
        JournalUI.EnsureExists();
        JournalUI.Instance?.Show();
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }
}
