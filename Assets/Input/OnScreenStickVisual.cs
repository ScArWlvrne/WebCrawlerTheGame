using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class OnScreenStickVisual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image stickVisual;
    [SerializeField] private Image stickBackground;
    private float maxRadius = 50f;

    private RectTransform touchArea;
    private Vector2 origin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        touchArea = stickVisual.rectTransform.parent as RectTransform;
        stickVisual.gameObject.SetActive(false);
        stickBackground.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchArea, 
            eventData.position, 
            eventData.pressEventCamera, 
            out origin
            );

        stickVisual.rectTransform.anchoredPosition = origin;
        stickBackground.rectTransform.anchoredPosition = origin;
        stickVisual.gameObject.SetActive(true);
        stickBackground.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stickVisual.gameObject.SetActive(false);
        stickBackground.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchArea, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 currentPosition
            );

        Vector2 displacement = currentPosition - origin;
        Vector2 clampedDisplacement = Vector2.ClampMagnitude(displacement, maxRadius);
        stickVisual.rectTransform.anchoredPosition = origin + clampedDisplacement;
    }
}
