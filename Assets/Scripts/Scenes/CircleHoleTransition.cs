using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleHoleTransition : MonoBehaviour
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private float closeDuration = 0.75f;

    private Material material;

    private void Awake()
    {
        material = Instantiate(transitionImage.material);
        transitionImage.material = material;

        // SetRadius(2f);

        transitionImage.gameObject.SetActive(true);
    }

    public IEnumerator Close()
    {
        transitionImage.gameObject.SetActive(true);
        SetRadius(2f);

        float timer = 0f;

        while (timer < closeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / closeDuration;

            SetRadius(Mathf.Lerp(2f, 0f, t));

            yield return null;
        }

        SetRadius(0f);
    }

    public IEnumerator Open()
    {
        float timer = 0f;

        while (timer < closeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / closeDuration;

            SetRadius(Mathf.Lerp(0f, 2f, t));

            yield return null;
        }

        SetRadius(2f);
        transitionImage.gameObject.SetActive(false);
    }

    private void SetRadius(float radius)
    {
        material.SetFloat("_Radius", radius);
    }

    public void SetOpen()
    {
        SetRadius(2f);
    }

    public void SetClosed()
    {
        SetRadius(0f);
    }
}