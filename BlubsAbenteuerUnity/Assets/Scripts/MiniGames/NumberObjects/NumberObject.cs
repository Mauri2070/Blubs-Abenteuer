using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// base class for a number object
public class NumberObject : MonoBehaviour, IPointerDownHandler
{
    // number object color constants
    public static Color failColor = new Color(0.749f, 0, 0);
    public static Color completedColor = new Color(0, 0.749f, 0.0156f);
    public static Color completedColorParent = new Color(0.5882f, 0.047f, 0.5647f);
    public static Color normalColor = new Color(0.96f, 0.96f, 0.96f);
    public static Color specialColor = new Color(0.0666f, 0, 1f);

    [SerializeField] protected int value;
    public int Value
    {
        get { return value; }
        set
        {
            this.value = value;
            if (textRepresentation.activeSelf)
            {
                textRepresentation.GetComponentInChildren<TextMeshProUGUI>().text = "" + value;
            }
        }
    }
    [SerializeField] private DisplayMode mode;
    public DisplayMode Mode
    {
        get { return mode; }
        set { mode = value; }
    }

    [SerializeField] protected GameObject visualRepresentation;
    private Sprite visualSprite;
    public Sprite VisualSprite
    {
        get
        {
            return visualSprite;
        }
        set
        {
            visualSprite = value;
            if (visualRepresentation.activeSelf)
            {
                visualRepresentation.GetComponent<Image>().sprite = value;
            }
        }
    }
    [SerializeField] protected GameObject textRepresentation;

    [SerializeField] private bool isInteractable = true;
    public bool IsInteractable
    {
        get { return isInteractable; }
        set { isInteractable = value; }
    }

    protected bool disableAudio = false;
    public bool DisableAudio
    {
        set
        {
            disableAudio = value;
        }
    }

    [SerializeField] protected Image feedbackBackground;
    //[SerializeField] protected Image commonBackground;
    private static readonly Vector3 singleDisplayScaleBack = new Vector3(0.55f, 1f, 1f);
    //private static readonly Vector3 singleDisplayScaleCommon = new Vector3(0.5f, 1f, 1f);
    private static readonly Vector3 doubleDisplayScale = new Vector3(1f, 1f, 1f);

    public bool overrideFeedbackColor = false;
    public bool childButton = true;

    public void Start()
    {
        //Debug.Log("NumberObject.Start - starting: " + disableAudio);
        switch (mode)
        {
            case DisplayMode.SET:
                visualRepresentation.SetActive(true);
                textRepresentation.SetActive(false);
                visualRepresentation.GetComponent<Image>().sprite = visualSprite;
                feedbackBackground.gameObject.GetComponent<RectTransform>().localScale = singleDisplayScaleBack;
                //commonBackground.gameObject.GetComponent<RectTransform>().localScale = singleDisplayScaleCommon;
                break;
            case DisplayMode.MIXED:
                visualRepresentation.SetActive(true);
                textRepresentation.SetActive(true);
                visualRepresentation.GetComponent<Image>().sprite = visualSprite;
                textRepresentation.GetComponentInChildren<TextMeshProUGUI>().SetText("" + value);
                feedbackBackground.gameObject.GetComponent<RectTransform>().localScale = doubleDisplayScale;
                //commonBackground.gameObject.GetComponent<RectTransform>().localScale = doubleDisplayScale;
                break;
            case DisplayMode.TEXT:
                visualRepresentation.SetActive(false);
                textRepresentation.SetActive(true);
                textRepresentation.GetComponentInChildren<TextMeshProUGUI>().SetText("" + value);
                feedbackBackground.gameObject.GetComponent<RectTransform>().localScale = singleDisplayScaleBack;
                //commonBackground.gameObject.GetComponent<RectTransform>().localScale = singleDisplayScaleCommon;
                break;
        }

        if (!feedbackBackground.color.Equals(specialColor) && !overrideFeedbackColor)
        {
            feedbackBackground.color = normalColor;
        }
        //Debug.Log("NumberObject.Start - finished: " + disableAudio);
    }

    public IEnumerator WrongInteraction()
    {
        feedbackBackground.color = failColor;
        yield return new WaitForSeconds(1f);
        feedbackBackground.color = normalColor;
    }

    public void RightInteraction()
    {
        if (childButton)
        {
            feedbackBackground.color = completedColor;
        }
        else
        {
            feedbackBackground.color = completedColorParent;
        }
    }

    public void SetSpecialColor()
    {
        //Debug.Log("Setting special color to number object with value "+value);
        feedbackBackground.color = specialColor;
    }

    public void SetFeedbackColor(Color color)
    {
        feedbackBackground.color = color;
    }

    // for playing audio on press
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Number Object - OnPointerDown (" + disableAudio + ")");
        if (disableAudio)
        {
            return;
        }

        if (NumberRepresentationProvider.NumberAudio.Length < value)
        {
            Debug.LogWarning("Missing audio output for number " + value);
        }
        else if (PlayerPrefsController.ShouldPlayAudio())
        {
            SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.NumberAudio[value - 1]);
        }
    }
}
