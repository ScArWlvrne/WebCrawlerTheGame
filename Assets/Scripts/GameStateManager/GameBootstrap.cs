using UnityEngine;

/// <summary>
/// Ensures persistent game services exist when a gameplay scene loads.
/// Place one instance in scenes like ArnavTestScene that do not already have GameStateManager.
/// Do not also add GameStateManager manually to this object — bootstrap creates it if missing.
/// </summary>
[DefaultExecutionOrder(-100)]
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private bool ensureDialogueUI = true;
    [SerializeField] private GameObject dialogueUIPrefab;
    [SerializeField] private bool ensureJournalUI = true;

    private void Awake()
    {
        EnsureAllSystems();
    }

    private void Start()
    {
        EnsureAllSystems();
    }

    public void EnsureAllSystems()
    {
        if (GameStateManager.Instance == null)
        {
            GameObject stateObject = new GameObject("GameStateManager");
            stateObject.AddComponent<GameStateManager>();
            Debug.Log("GameBootstrap: created GameStateManager");
        }

        if (ensureDialogueUI && DialogueUI.Instance == null)
        {
            if (dialogueUIPrefab != null)
                Instantiate(dialogueUIPrefab);
            else
                DialogueUI.CreateDefaultUI();

            Debug.Log("GameBootstrap: created DialogueSystem");
        }

        if (ensureJournalUI && JournalUI.Instance == null)
        {
            JournalUI.EnsureExists();
            Debug.Log("GameBootstrap: created JournalSystem");
        }
    }
}
