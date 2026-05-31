using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetTrust(GameCharacters.Lily, 40);

        DialogueOption lowTrustOption = new DialogueOption
        {
            optionText = "Ask Lily for the admin URL",
            trustCharacter = GameCharacters.Lily,
            minTrust = 50
        };

        Debug.Log("Can show low trust option before trust gain: " +
            DialogueStateEvaluator.CanShowOption(lowTrustOption));

        GameStateManager.Instance.AddTrust(GameCharacters.Lily, 20);

        Debug.Log("Can show low trust option after trust gain: " +
            DialogueStateEvaluator.CanShowOption(lowTrustOption));

        DialogueOption trustGainOption = new DialogueOption
        {
            optionText = "Say something Joe-like",
            trustChangeCharacter = GameCharacters.Lily,
            trustChange = 10
        };

        DialogueStateEvaluator.ApplyOptionEffects(trustGainOption);

        Debug.Log("Lily trust after option effect: " +
            GameStateManager.Instance.GetTrust(GameCharacters.Lily));
    }
}