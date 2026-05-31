using UnityEngine;

public class JournalEntryInteractable : MonoBehaviour, IInteractable
{
    [Header("Journal File")]
    [SerializeField] private JournalPaths.JournalOwner journalOwner = JournalPaths.JournalOwner.Test;
    [SerializeField] private string fileName = "test_note.txt";

    [TextArea(3, 8)]
    [SerializeField] private string content = "This is a test journal entry collected from ArnavTestScene.";

    [Header("Interaction")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private string interactableId = "arnav_test_journal_entry";
    [SerializeField] private bool exhaustAfterUse = true;
    [SerializeField] private bool showJournalOnPickup = true;

    [Header("UI")]
    [SerializeField] private JournalUI journalUI;

    private Collider interactionCollider;

    private void Awake()
    {
        interactionCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        ApplyExhaustedStateIfNeeded();
    }

    public void Interact()
    {
        Debug.Log("JournalEntryInteractable.Interact() on " + gameObject.name);

        JournalUI.EnsureExists();

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("JournalEntryInteractable: GameStateManager.Instance is null.");
            return;
        }

        if (IsExhausted())
        {
            Debug.Log("Journal entry already collected: " + interactableId);
            return;
        }

        JournalUI ui = ResolveJournalUI();
        if (ui == null)
        {
            Debug.LogError("JournalEntryInteractable: JournalUI could not be created.");
            return;
        }

        string targetFolder = JournalPaths.GetOwnerFolder(journalOwner);
        string path = JournalPaths.Build(targetFolder, fileName);
        GameStateManager.Instance.AddJournalFile(path, content);

        if (exhaustAfterUse)
        {
            GameStateManager.Instance.ExhaustInteractable(interactableId);
            DisableInteraction();
        }

        GameStateManager.Instance.SaveGame();

        Debug.Log("Added journal file: " + path);

        ui.Refresh();
        if (showJournalOnPickup)
            ui.Show();
    }

    public Transform GetPromptAnchor()
    {
        if (IsExhausted())
            return null;

        return promptAnchor != null ? promptAnchor : transform;
    }

    private bool IsExhausted()
    {
        return exhaustAfterUse &&
               GameStateManager.Instance != null &&
               GameStateManager.Instance.IsInteractableExhausted(interactableId);
    }

    private void ApplyExhaustedStateIfNeeded()
    {
        if (IsExhausted())
            DisableInteraction();
    }

    private void DisableInteraction()
    {
        if (interactionCollider != null)
            interactionCollider.enabled = false;
    }

    private JournalUI ResolveJournalUI()
    {
        if (journalUI != null)
            return journalUI;

        return JournalUI.Instance;
    }
}
