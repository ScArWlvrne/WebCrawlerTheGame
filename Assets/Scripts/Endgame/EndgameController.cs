using UnityEngine;

public class EndgameController : MonoBehaviour
{
    public static EndgameController Instance { get; private set; }

    private bool finaleAnnounced;

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        new GameObject("EndgameController").AddComponent<EndgameController>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (finaleAnnounced || GameStateManager.Instance == null)
            return;

        if (!GameStateManager.Instance.GetFlag(GameFlags.GameWon))
            return;

        finaleAnnounced = true;
        DialogueUI.EnsureExists();
        DialogueUI.Instance?.StartDialogue(BuildFinaleConversation());
    }

    private static DialogueConversation BuildFinaleConversation()
    {
        return new DialogueConversation
        {
            conversationId = "xbank_finale",
            startNodeId = "finale",
            nodes = new System.Collections.Generic.List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "finale",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "The CEO's bank account is empty. The paper trail is not. X Bank's transaction history ties Donald, Meridian, DataVault, and Araknyd into one very expensive web.",
                    nextNodeId = "credits"
                },
                new DialogueNode
                {
                    nodeId = "credits",
                    speaker = "Web Crawler",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Endgame complete: transfer initiated and Meridian wires exposed.",
                    nextNodeId = null
                }
            }
        };
    }
}
