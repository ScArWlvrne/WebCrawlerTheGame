using UnityEngine;

public class FlagTester : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetFlag(GameFlags.JoeFacebookViewed, true);

        GameStateManager.Instance.SetTrust(GameCharacters.Lily, 50);

        GameStateManager.Instance.AddTrust(GameCharacters.Lily, 10);

        GameStateManager.Instance.SaveGame();

        GameStateManager.Instance.State = new GameState();

        Debug.Log("Trust before load: " + GameStateManager.Instance.GetTrust(GameCharacters.Lily));

        GameStateManager.Instance.LoadGame();

        Debug.Log("Trust after load: " + GameStateManager.Instance.GetTrust(GameCharacters.Lily));
    }
}