using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// hold button implementation for main menu switch child->parent
public class ToParentModeSwitch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] [Range(0, 5)] private float holdTime;
    [SerializeField] private Slider progressSlider;

    private void Awake()
    {
        progressSlider.value = 0;
        progressSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (startTime > 0)
        {
            progressSlider.value = (Time.time - startTime) / holdTime;
            if (Time.time - startTime >= holdTime)
            {
                startTime = -1;
                FindObjectOfType<MainMenuController>().ActivateParentMode();
                progressSlider.value = 0;
                progressSlider.gameObject.SetActive(false);
            }
        }
    }

    private float startTime = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        startTime = Time.time;
        progressSlider.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        startTime = -1;
        progressSlider.value = 0;
        progressSlider.gameObject.SetActive(false);
    }
}
