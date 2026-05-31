using UnityEngine;

public class CodeBlockInteractable : MonoBehaviour, IInteractable
{
    [Header("Code Block")]
    [SerializeField] private string codeBlockId = GameCodeBlocks.ArnavTestCodeBlock;
    [SerializeField] private string requiredFlag;

    [Header("Visuals")]
    [SerializeField] private GameObject webbedOverlay;
    [SerializeField] private GameObject uncommentedVisual;

    [Header("Interaction")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private bool disableInteractionWhenUncommented = true;

    [Header("Optional Dialogue")]
    [SerializeField] private bool startDialogueOnUncomment;
    [SerializeField] private string requiredFlagForDialogue;
    [SerializeField] private CodeBlockDialoguePreset postUncommentDialogue = CodeBlockDialoguePreset.None;

    public enum CodeBlockDialoguePreset
    {
        None,
        WebInspectorAdminDownloadReaction
    }

    private Collider interactionCollider;

    public void Configure(string newCodeBlockId, string newRequiredFlag = null, bool disableWhenUncommented = true)
    {
        codeBlockId = newCodeBlockId;
        requiredFlag = newRequiredFlag;
        disableInteractionWhenUncommented = disableWhenUncommented;
        SyncVisualState();
        ApplyInteractionState();
    }

    private void Awake()
    {
        interactionCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        SyncVisualState();
        ApplyInteractionState();
    }

    public void Interact()
    {
        Debug.Log("CodeBlockInteractable.Interact() on " + gameObject.name);

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("CodeBlockInteractable: GameStateManager.Instance is null.");
            return;
        }

        if (!string.IsNullOrEmpty(requiredFlag) &&
            !GameStateManager.Instance.GetFlag(requiredFlag))
        {
            Debug.Log("CodeBlockInteractable: required flag not set: " + requiredFlag);
            return;
        }

        if (IsUncommented())
        {
            Debug.Log("Code block already uncommented: " + codeBlockId);
            return;
        }

        GameStateManager.Instance.UncommentCodeBlock(codeBlockId);
        if (codeBlockId == GameCodeBlocks.AdminDownloadDatabaseButton)
            GameStateManager.Instance.SetFlag(GameFlags.AraknydFinaleUnlocked, true);

        if (codeBlockId == GameCodeBlocks.AdminFileExplorerSyncButton)
            SyncDonaldBankPasswordToJournal();

        GameStateManager.Instance.SaveGame();

        Debug.Log("Uncommented code block: " + codeBlockId);

        SyncVisualState();
        ApplyInteractionState();
        FindFirstObjectByType<BrowserAdminDashboardGate>()?.Refresh();
        FindFirstObjectByType<BrowserXBankGate>()?.Refresh();
        TryStartPostUncommentDialogue();
    }

    public Transform GetPromptAnchor()
    {
        if (disableInteractionWhenUncommented && IsUncommented())
            return null;

        return promptAnchor != null ? promptAnchor : transform;
    }

    private bool IsUncommented()
    {
        return GameStateManager.Instance != null &&
               GameStateManager.Instance.IsCodeBlockUncommented(codeBlockId);
    }

    private void SyncVisualState()
    {
        bool uncommented = IsUncommented();

        if (webbedOverlay != null)
            webbedOverlay.SetActive(!uncommented);

        if (uncommentedVisual != null)
            uncommentedVisual.SetActive(uncommented);
    }

    private void ApplyInteractionState()
    {
        if (!disableInteractionWhenUncommented || !IsUncommented())
            return;

        if (interactionCollider != null)
            interactionCollider.enabled = false;
    }

    private void TryStartPostUncommentDialogue()
    {
        if (!startDialogueOnUncomment || postUncommentDialogue == CodeBlockDialoguePreset.None)
            return;

        if (GameStateManager.Instance == null)
            return;

        if (!string.IsNullOrEmpty(requiredFlagForDialogue) &&
            !GameStateManager.Instance.GetFlag(requiredFlagForDialogue))
        {
            return;
        }

        DialogueConversation conversation = BuildPostUncommentConversation();
        if (conversation == null)
            return;

        DialogueUI ui = DialogueUI.Instance;
        if (ui == null)
        {
            Debug.LogWarning("CodeBlockInteractable: DialogueUI.Instance is null.");
            return;
        }

        ui.StartDialogue(conversation);
    }

    private DialogueConversation BuildPostUncommentConversation()
    {
        switch (postUncommentDialogue)
        {
            case CodeBlockDialoguePreset.WebInspectorAdminDownloadReaction:
                return DialogueConversationFactory.GetWebInspectorAdminDownloadReaction();
            default:
                return null;
        }
    }

    private static void SyncDonaldBankPasswordToJournal()
    {
        DonaldBankPasswordIntel.SyncToPasswordsFile();
        JournalUI.EnsureExists();
        JournalUI.Instance?.Show();
    }
}
