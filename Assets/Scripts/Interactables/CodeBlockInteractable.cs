using UnityEngine;

public class CodeBlockInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string codeBlockId = "admin_download_database_button";
    [SerializeField] private string requiredFlag = GameFlags.AdminDashboardUnlocked;
    [SerializeField] private Transform promptAnchor;

    public void Interact()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("CodeBlockInteractable: no GameStateManager is available.");
            return;
        }

        if (!string.IsNullOrEmpty(requiredFlag) &&
            !GameStateManager.Instance.GetFlag(requiredFlag))
        {
            Debug.Log("CodeBlockInteractable: required flag not set: " + requiredFlag);
            return;
        }

        GameStateManager.Instance.UncommentCodeBlock(codeBlockId);
        GameStateManager.Instance.SaveGame();
        Debug.Log("CodeBlockInteractable: uncommented " + codeBlockId);
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }
}
