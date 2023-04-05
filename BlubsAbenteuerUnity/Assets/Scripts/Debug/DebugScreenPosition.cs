using UnityEngine;
using UnityEngine.EventSystems;

public class DebugScreenPosition : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Debug: "+eventData.position);
    }
}
