using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildImageTransparancy : MonoBehaviour
{
    private Button parentButton;
    private bool isParentInteractable;
    private Image image;
    [SerializeField] private Vector4 enabledColor;
    [SerializeField] private Vector4 disabledColor;

    // Start is called before the first frame update
    void Start()
    {
        parentButton = transform.GetComponentInParent<Button>();
        isParentInteractable = parentButton.interactable;
        image = GetComponent<Image>();

        if (isParentInteractable)
        {
            image.color = enabledColor;
        } else
        {
            image.color = disabledColor;
        }
    }

    public void SwapInteractable()
    {
        isParentInteractable = !isParentInteractable;

        if (isParentInteractable)
        {
            image.color = enabledColor;
        }
        else
        {
            image.color = disabledColor;
        }
    }
}
