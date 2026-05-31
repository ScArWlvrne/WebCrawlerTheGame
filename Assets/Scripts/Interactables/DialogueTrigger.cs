using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    public enum ConversationPreset
    {
        LilyTest
    }

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private ConversationPreset conversationPreset = ConversationPreset.LilyTest;

    [Header("Interaction")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private string interactableId = "test_scene_lily_dialogue";
    [SerializeField] private bool exhaustAfterDialogue = true;

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

        return DialogueUI.Instance;
    }

    private DialogueConversation BuildConversation()
    {
        switch (conversationPreset)
        {
            case ConversationPreset.LilyTest:
            default:
                return DialogueConversationFactory.GetLilyTestConversation();
        }
    }
}
