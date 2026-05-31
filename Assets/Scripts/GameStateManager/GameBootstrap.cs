using UnityEngine;

/// <summary>
/// Ensures persistent game services exist when a gameplay scene loads.
/// Place one instance in scenes like TestScene that do not already have GameStateManager.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private bool ensureDialogueUI = true;
    [SerializeField] private GameObject dialogueUIPrefab;

    private void Awake()
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
    }
}
