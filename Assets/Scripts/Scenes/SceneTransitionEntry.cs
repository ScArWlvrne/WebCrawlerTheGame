using System.Collections;
using UnityEngine;

public class SceneTransitionEntry : MonoBehaviour
{
    [SerializeField] private CircleHoleTransition transition;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool resetPlayerScale = true;
    [SerializeField] private MonoBehaviour playerControllerScript;

    private IEnumerator Start()
    {
        ApplyPlayerEntry();

        transition.SetClosed();

        yield return null;

        yield return transition.Open();
    }

    private void ApplyPlayerEntry()
    {
        if (playerTransform == null)
            return;

        if (resetPlayerScale)
            playerTransform.localScale = Vector3.one;

        if (spawnPoint != null)
            playerTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;
    }
}