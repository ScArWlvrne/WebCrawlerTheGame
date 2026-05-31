using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor;
    }
}
