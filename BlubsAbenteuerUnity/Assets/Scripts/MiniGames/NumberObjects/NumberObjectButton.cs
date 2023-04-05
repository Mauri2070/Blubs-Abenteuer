using UnityEngine;
using UnityEngine.EventSystems;

// button version of number objects
public class NumberObjectButton : NumberObject, IPointerDownHandler
{
    private MiniGame miniGame;
    public MiniGame MiniGame
    {
        get { return miniGame; }
        set { miniGame = value; }
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (IsInteractable && miniGame != null)
        {
            miniGame.ReceiveNumberObjectButtonData(this);
        }
    }

    public void OnWrongInteraction()
    {
        StartCoroutine(WrongInteraction());
        Debug.Log("Dieser Button ist nicht an der Reihe");
    }

    public void OnRightInteraction()
    {
        IsInteractable = false;
        RightInteraction();
        Debug.Log("Richtigen Button gedrückt");
    }
}
