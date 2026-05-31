using TMPro;
using UnityEngine;

/// <summary>
/// Opens a Venom temp-password email and records that Joe's security-question journal path is unlocked.
/// </summary>
public class EmailTempPasswordMessageInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private GameObject emailBodyPanel;
    [SerializeField] private TMP_Text emailBodyText;
    [SerializeField] private string tempPasswordMarker = EmailConstants.VenomTempPassword;

    public void Interact()
    {
        if (emailBodyPanel != null)
            emailBodyPanel.SetActive(true);

        string body = emailBodyText != null ? emailBodyText.text : string.Empty;
        if (!body.Contains(tempPasswordMarker))
            return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("EmailTempPasswordMessageInteractable: GameStateManager missing.");
            return;
        }

        GameStateManager.Instance.SetFlag(GameFlags.JoeSecurityQuestionsInJournal, true);

        GameStateManager.Instance.AddJournalFile(
            EmailConstants.SecurityQuestionsJournalPath,
            "Venom temp password: " + tempPasswordMarker);

        GameStateManager.Instance.SaveGame();
        Debug.Log("Venom temp-password email read; security-question journal flag set.");
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    public void Configure(GameObject bodyPanel, TMP_Text bodyText, Transform anchor = null)
    {
        emailBodyPanel = bodyPanel;
        emailBodyText = bodyText;
        if (anchor != null)
            promptAnchor = anchor;
    }
}
