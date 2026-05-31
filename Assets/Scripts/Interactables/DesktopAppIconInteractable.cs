using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DesktopAppIconInteractable : MonoBehaviour, IInteractable
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    [Header("Scene")]
    [SerializeField] private string sceneName;

    [Header("References")]
    [SerializeField] private MonoBehaviour playerControllerScript;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private Transform promptAnchor;

    [Header("Player Positioning")]
    [SerializeField] private Transform playerLaunchPoint;
    [SerializeField] private float minDistanceFromPivot = 3f;

    [Header("Icon Trapdoor")]
    [SerializeField] private Transform iconPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openDuration = 0.4f;

    [Header("Player Leap")]
    [SerializeField] private Transform leapTarget;
    [SerializeField] private string leapAnimationName = "rig|leap";
    [SerializeField] private float leapDuration = 1.5f;
    [SerializeField] private float shrinkStartPercent = 0.75f;
    [SerializeField] private float moveStartPercent = 0.3f;

    [Header("Transition")]
    [SerializeField] private CircleHoleTransition circleTransition;
    [SerializeField] private float transitionDelay = 0.4f;

    private bool isRunning;

    public string GetPromptText()
    {
        return "Open";
    }

    public void Interact()
    {
        if (isRunning)
            return;

        if (PlayerIsTooCloseToPivot())
        {
            Debug.LogWarning("Too close to pivot!");
            // TODO: Error sound or something idfk
            return;
        }

        StartCoroutine(OpenAppSequence());
    }

    public Transform GetPromptAnchor()
    {
        return isRunning ? null : promptAnchor;
    }

    private IEnumerator OpenAppSequence()
    {
        Destroy(interactionPromptUI.gameObject);
        isRunning = true;

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;

        yield return new WaitForSeconds(transitionDelay);

        if (circleTransition != null)
            StartCoroutine(circleTransition.Close());

        yield return SwingIconOpen();

        if (playerAnimator != null)
            playerAnimator.Play(leapAnimationName);

        if (playerAnimator != null)
            playerAnimator.SetBool("IsMoving", false);

       yield return LeapAndShrinkPlayer();

        yield return _waitForSeconds0_5; // Allow the transition to play for a bit before switching scenes
    
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator SwingIconOpen()
    {
        Quaternion startRot = iconPivot.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, openAngle);

        float timer = 0f;

        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float t = timer / openDuration;

            iconPivot.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        iconPivot.localRotation = endRot;
    }

    private IEnumerator LeapAndShrinkPlayer()
    {
        Vector3 startPos = playerTransform.position;
        Vector3 endPos = leapTarget.position;
        endPos.y = startPos.y;

        Vector3 startScale = playerTransform.localScale;
        Vector3 endScale = Vector3.zero;

        float timer = 0f;

        while (timer < leapDuration)
        {
            timer += Time.deltaTime;

            float t = timer / leapDuration;

            // MOVEMENT
            if (t >= moveStartPercent)
            {
                float moveT = Mathf.InverseLerp(moveStartPercent, 1f, t);

                playerTransform.position =
                    Vector3.Lerp(startPos, endPos, moveT);
            }

            // SHRINKING
            if (t >= shrinkStartPercent)
            {
                float shrinkT =
                    Mathf.InverseLerp(shrinkStartPercent, 1f, t);

                playerTransform.localScale =
                    Vector3.Lerp(startScale, endScale, shrinkT);
            }

            yield return null;
        }

        playerTransform.position = endPos;
        playerTransform.localScale = endScale;
    }

    private bool PlayerIsTooCloseToPivot()
    {
        Vector3 playerPos = playerTransform.position;
        Vector3 pivotPos = iconPivot.position;

        playerPos.y = 0f;
        pivotPos.y = 0f;

        float distance = Vector3.Distance(playerPos, pivotPos);

        return distance < minDistanceFromPivot;
    }
}