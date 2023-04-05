using UnityEngine;
using UnityEngine.EventSystems;

// drag-and-drop with line trace implementation of number objects
public class NumberObjectLineDnD : NumberObject, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("DnD vars.")]
    [SerializeField] private Canvas canvas;
    public Canvas Canvas
    {
        get { return canvas; }
        set { canvas = value; }
    }

    private RectTransform rectTransform;
    private Vector2 dragPosition;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        // set starting-Point for lineRenderer
        dragPosition = rectTransform.position;
        //Debug.Log(dragPosition);
        SetLinePosition(dragPosition, 0);
        SetLinePosition(dragPosition, 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        //Debug.Log("B1" + lineRenderer.GetPosition(0));
        if (IsInteractable)
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.NeutralInteraction());
            dragPosition = eventData.position;
            SetLinePosition(dragPosition, 0, true);
        }
        //Debug.Log("B2" + lineRenderer.GetPosition(1));
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if (IsInteractable)
        {
            dragPosition += eventData.delta;
            SetLinePosition(dragPosition, 1, true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if (IsInteractable)
        {
            ResetLineDnD();
        }
    }

    public void SetLinePosition(Vector3 targetPosition, int idx, bool worldTransform = false)
    {
        if (worldTransform)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(targetPosition);
            worldPoint.z = 0;
            lineRenderer.SetPosition(idx, worldPoint);
        }
        else
        {
            lineRenderer.SetPosition(idx, targetPosition);
        }
    }

    public void ResetLineDnD()
    {
        SetLinePosition(lineRenderer.GetPosition(0), 1);
    }
}
