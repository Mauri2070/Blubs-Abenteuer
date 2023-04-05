using UnityEngine;
using UnityEngine.EventSystems;

// drag-and-drop implementation of number objects
public class NumberObjectDnD : NumberObject, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] float maxScale;
    [SerializeField] private Canvas canvas;
    public Canvas Canvas
    {
        get { return canvas; }
        set { canvas = value; }
    }
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 initialPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        if (IsInteractable)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
            initialPosition = rectTransform.position;
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.NeutralInteraction());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if (IsInteractable)
        {
            rectTransform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (IsInteractable)
        {
            ResetDnD();
        }

    }

    public void ResetDnD()
    {
        rectTransform.position = initialPosition;
    }

    // animation for insert mini games
    private bool animate = false;
    private float animationTime;
    private float currentTime;

    public void Animate(float animationTime)
    {
        this.animationTime = animationTime;
        currentTime = 0;
        animate = true;
    }

    private void Update()
    {
        if (animate)
        {
            if (currentTime <= animationTime / 2)
            {
                gameObject.transform.localScale += Vector3.one * animationTime / 2f * Time.deltaTime;
            }
            else
            {
                gameObject.transform.localScale -= Vector3.one * animationTime / 2f * Time.deltaTime;
            }
            currentTime += Time.deltaTime;
            if (currentTime >= animationTime)
            {
                animate = false;
                gameObject.transform.localScale = Vector3.one;
            }
        }
    }
}
