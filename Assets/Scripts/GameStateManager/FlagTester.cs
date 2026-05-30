using UnityEngine;

public class FlagTester : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetFlag(
            "joe_facebook_viewed",
            true
        );

        Debug.Log(
            GameStateManager.Instance.GetFlag(
                "joe_facebook_viewed"
            )
        );
    }
}