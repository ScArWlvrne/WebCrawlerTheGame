using UnityEngine;

public class FlagTester : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetFlag("joe_facebook_viewed", true);

        GameStateManager.Instance.SetTrust("lily", 50);

        GameStateManager.Instance.AddTrust("lily", 10);

        GameStateManager.Instance.SaveGame();

        GameStateManager.Instance.State = new GameState();

        Debug.Log("Trust before load: " + GameStateManager.Instance.GetTrust("lily"));

        GameStateManager.Instance.LoadGame();

        Debug.Log("Trust after load: " + GameStateManager.Instance.GetTrust("lily"));
    }
}