using UnityEngine;

public class FlagTester : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetFlag(GameFlags.JoeFacebookViewed, true);

        GameStateManager.Instance.SetTrust(GameCharacters.Lily, 50);
        GameStateManager.Instance.AddTrust(GameCharacters.Lily, 10);

        GameStateManager.Instance.AddJournalFile(
            "journal/araknyd/admin_url.txt",
            "https://admin.araknyd.local"
        );

        GameStateManager.Instance.ExhaustInteractable("joe_birthday_post");
        GameStateManager.Instance.UncommentCodeBlock("admin_download_database_button");

        GameStateManager.Instance.SaveGame();

        GameStateManager.Instance.State = new GameState();

        Debug.Log("Before load trust: " + GameStateManager.Instance.GetTrust(GameCharacters.Lily));
        Debug.Log("Before load admin URL file exists: " + GameStateManager.Instance.HasJournalFile("journal/araknyd/admin_url.txt"));

        GameStateManager.Instance.LoadGame();

        Debug.Log("After load trust: " + GameStateManager.Instance.GetTrust(GameCharacters.Lily));
        Debug.Log("After load admin URL file exists: " + GameStateManager.Instance.HasJournalFile("journal/araknyd/admin_url.txt"));
        Debug.Log("After load admin URL content: " + GameStateManager.Instance.GetJournalFileContent("journal/araknyd/admin_url.txt"));
        Debug.Log("After load interactable exhausted: " + GameStateManager.Instance.IsInteractableExhausted("joe_birthday_post"));
        Debug.Log("After load code block uncommented: " + GameStateManager.Instance.IsCodeBlockUncommented("admin_download_database_button"));
    }
}