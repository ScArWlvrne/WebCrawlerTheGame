using UnityEngine;

public class CodeBlockInteractable : MonoBehaviour, IInteractable
{
    [Header("Code Block")]
    [SerializeField] private string codeBlockId = GameCodeBlocks.ArnavTestCodeBlock;

    [Header("Visuals")]
    [SerializeField] private GameObject webbedOverlay;
    [SerializeField] private GameObject uncommentedVisual;

    [Header("Interaction")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private bool disableInteractionWhenUncommented = true;

    private Collider interactionCollider;

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

        if (IsUncommented())
        {
            Debug.Log("Code block already uncommented: " + codeBlockId);
            return;
        }

        GameStateManager.Instance.UncommentCodeBlock(codeBlockId);
        GameStateManager.Instance.SaveGame();

        Debug.Log("Uncommented code block: " + codeBlockId);

        SyncVisualState();
        ApplyInteractionState();
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
}
