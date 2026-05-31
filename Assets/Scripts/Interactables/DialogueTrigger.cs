using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    public enum ConversationPreset
    {
        LilyTest,
        AraknydCrawlerTest,
        LilyE2E,
        WebInspectorE2E,
        XBankCrawler,
        WebInspectorXBank,
        HaleyXBank
    }

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private ConversationPreset conversationPreset = ConversationPreset.LilyTest;
    [SerializeField] private TextAsset dialogueAsset;

    [Header("Interaction")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private string interactableId = "test_scene_lily_dialogue";
    [SerializeField] private bool exhaustAfterDialogue = true;

    public void Configure(ConversationPreset preset, string newInteractableId, bool exhaust = true)
    {
        conversationPreset = preset;
        interactableId = newInteractableId;
        exhaustAfterDialogue = exhaust;
    }

    public void Interact()
    {
        Debug.Log("DialogueTrigger.Interact() on " + gameObject.name);

        if (exhaustAfterDialogue &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.IsInteractableExhausted(interactableId))
        {
            Debug.Log("Dialogue already completed for: " + interactableId);
            return;
        }

        DialogueUI ui = ResolveDialogueUI();
        if (ui == null)
        {
            Debug.LogError("DialogueTrigger: No DialogueUI found in scene.");
            return;
        }

        DialogueConversation conversation = BuildConversation();
        string exhaustId = exhaustAfterDialogue ? interactableId : null;
        ui.StartDialogue(conversation, exhaustId);
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    private DialogueUI ResolveDialogueUI()
    {
        if (dialogueUI != null)
            return dialogueUI;

        DialogueUI.EnsureExists();
        return DialogueUI.Instance;
    }

    private DialogueConversation BuildConversation()
    {
        if (dialogueAsset != null)
            return DialogueDocumentLoader.Load(dialogueAsset);

        switch (conversationPreset)
        {
            case ConversationPreset.AraknydCrawlerTest:
                return DialogueConversationFactory.GetAraknydCrawlerTestConversation();
            case ConversationPreset.LilyE2E:
                return DialogueConversationFactory.GetLilyE2EConversation();
            case ConversationPreset.WebInspectorE2E:
                return DialogueConversationFactory.GetWebInspectorE2EConversation();
            case ConversationPreset.XBankCrawler:
                return DialogueConversationFactory.GetXBankCrawlerConversation();
            case ConversationPreset.WebInspectorXBank:
                return DialogueConversationFactory.GetWebInspectorXBankSourceConversation();
            case ConversationPreset.HaleyXBank:
                return DialogueConversationFactory.GetHaleyXBankConversation();
            case ConversationPreset.LilyTest:
            default:
                return DialogueConversationFactory.GetLilyTestConversation();
        }
    }
}
