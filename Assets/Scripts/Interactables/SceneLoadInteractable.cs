using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadInteractable : Interactable
{
    [SerializeField] private string sceneName = "DesktopHub";
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private CircleHoleTransition circleTransition;
    [SerializeField] private float transitionDelay = 0.2f;

    private bool isLoading;

    public override void Interact()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneSequence());
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    private IEnumerator LoadSceneSequence()
    {
        isLoading = true;

        if (circleTransition != null)
        {
            yield return StartCoroutine(circleTransition.Close());
            yield return new WaitForSeconds(transitionDelay);
        }

        SceneManager.LoadScene(sceneName);
    }
}
