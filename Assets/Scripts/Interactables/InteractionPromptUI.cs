using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private Image buttonIcon;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite gamepadSprite;
    [SerializeField] private Camera mainCamera;

    public void Show(bool usingGamepad, Transform anchor)
    {
        gameObject.SetActive(true);

        buttonIcon.sprite = usingGamepad ? gamepadSprite : keyboardSprite;

        transform.position = anchor.position;

        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}