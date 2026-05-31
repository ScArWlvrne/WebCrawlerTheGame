using TMPro;
using UnityEngine;

public class EmailMessageInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private string subject = "Email";

    [TextArea(4, 12)]
    [SerializeField] private string body;

    [SerializeField] private string journalFilePath;

    [TextArea(3, 8)]
    [SerializeField] private string journalFileContent;

    [SerializeField] private string flagToSet = GameFlags.DonaldEmailRead;
    [SerializeField] private string requiredJournalFile;
    [SerializeField] private string unavailableText = "This email is still buried in the inbox. Read the earlier X Bank clues first.";

    public void Configure(
        TMP_Text targetBodyText,
        string emailSubject,
        string emailBody,
        string targetJournalPath,
        string targetJournalContent,
        string requiredFile = null)
    {
        bodyText = targetBodyText;
        subject = emailSubject;
        body = emailBody;
        journalFilePath = targetJournalPath;
        journalFileContent = targetJournalContent;
        requiredJournalFile = requiredFile;
    }

    public void Interact()
    {
        if (GameStateManager.Instance == null)
            return;

        if (!string.IsNullOrEmpty(requiredJournalFile) &&
            !GameStateManager.Instance.HasJournalFile(requiredJournalFile))
        {
            SetBody(unavailableText);
            return;
        }

        SetBody(subject + "\n\n" + body);

        if (!string.IsNullOrEmpty(journalFilePath))
            GameStateManager.Instance.AddJournalFile(journalFilePath, journalFileContent);

        if (!string.IsNullOrEmpty(flagToSet))
            GameStateManager.Instance.SetFlag(flagToSet, true);

        GameStateManager.Instance.SaveGame();
        JournalUI.Instance?.Refresh();
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    private void SetBody(string text)
    {
        if (bodyText != null)
            bodyText.text = text;
    }
}
