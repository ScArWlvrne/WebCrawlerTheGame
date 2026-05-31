using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneName = "DesktopHub";
    [SerializeField] private string requiredFlag;
    [SerializeField] private string lockedMessage = "Locked.";
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private CircleHoleTransition circleTransition;
    [SerializeField] private float transitionDelay = 0.2f;

    private bool isLoading;

    public void Configure(string targetSceneName)
    {
        sceneName = targetSceneName;
    }

    public void Configure(string targetSceneName, string newRequiredFlag)
    {
        sceneName = targetSceneName;
        requiredFlag = newRequiredFlag;
    }

    public void Interact()
    {
        if (isLoading)
            return;

        if (!CanLoad())
        {
            Debug.Log(lockedMessage);
            return;
        }

        StartCoroutine(LoadSceneSequence());
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    public void Configure(string targetScene, CircleHoleTransition transition, Transform anchor)
    {
        sceneName = targetScene;
        circleTransition = transition;
        promptAnchor = anchor;
    }

    private bool CanLoad()
    {
        if (string.IsNullOrEmpty(requiredFlag))
            return true;

        if (GameStateManager.Instance == null)
            return true;

        return GameStateManager.Instance.GetFlag(requiredFlag);
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
