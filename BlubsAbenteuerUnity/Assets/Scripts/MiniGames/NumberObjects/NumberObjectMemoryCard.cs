using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// memory card implementation of number objects
public class NumberObjectMemoryCard : NumberObject, IPointerDownHandler
{
    [SerializeField] private Sprite cardBackground;
    [SerializeField] private GameObject textCover;

    public bool turningCard;
    public bool Turning
    {
        get { return turningCard; }
    }
    public bool firstHalf = true;
    public bool backUp = true;
    [SerializeField] [Range(1, 1000)] private float turningSpeed;

    public MemoryMiniGame miniGame;

    public bool busy;

    public new void Start()
    {
        base.Start();
        visualRepresentation.GetComponent<Image>().sprite = cardBackground;
        if (true || Mode != DisplayMode.SET)
        {
            textCover.SetActive(true);
        }
        turningCard = false;
        firstHalf = true;
        backUp = true;
        busy = false;
    }

    private void Update()
    {
        if (turningCard)
        {
            // rotate to 90°, then rotate back to 0° after changing the sprite
            float angle = turningSpeed * Time.deltaTime;
            if (firstHalf)
            {
                transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
            }
            else
            {
                transform.rotation *= Quaternion.AngleAxis(angle, -1 * Vector3.up);
            }
            // Debug.Log("Rotated to " + transform.eulerAngles.y);

            // change rotation half
            /*  Rotation Bug Fix:
             *  Checking for firstHalf to avoid swapping twice in one pass
             *  double swapp caused by large deltaTime followed by short deltaTime causing the angle to be
             *  larger than 90 for more than one frame
             */
            if (transform.eulerAngles.y >= 90.0f && transform.eulerAngles.y <= 300.0f && firstHalf)
            {
                // Debug.Log("Changing half. firstHalf: "+firstHalf+", backUp: "+backUp);
                firstHalf = false;
                // backUp = true -> flip from hidden to open
                if (backUp)
                {
                    // Debug.Log("Swapping to front");
                    visualRepresentation.GetComponent<Image>().sprite = VisualSprite;
                    if (true || Mode != DisplayMode.SET)
                    {
                        textCover.SetActive(false);
                    }
                    backUp = false;
                }
                else
                {
                    // Debug.Log("Swapping to back");
                    visualRepresentation.GetComponent<Image>().sprite = cardBackground;
                    if (true || Mode != DisplayMode.SET)
                    {
                        textCover.SetActive(true);
                    }
                    backUp = true;
                }
            }

            // stop turning
            if (!firstHalf && (transform.eulerAngles.y == 0 || transform.eulerAngles.y > 300))
            {
                // Debug.Log("Stopping to turn.");
                turningCard = false;
                firstHalf = true;
                transform.rotation = Quaternion.identity;
                busy = false;
            }
        }
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        if (IsInteractable && !turningCard)
        {
            miniGame.AcceptCard(this);
        }
    }

    public void TurnCard()
    {
        turningCard = true;
    }

    public new IEnumerator WrongInteraction()
    {
        IsInteractable = false;
        yield return base.WrongInteraction();
        turningCard = true;
        StartCoroutine(WaitTurning());
    }

    private IEnumerator WaitTurning()
    {
        while (turningCard)
        {
            yield return new WaitForSeconds(0.1f);
        }
        IsInteractable = true;
        busy = false;
    }
}
