using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// drag and drop with line trace implementation for 2 number object combinations (connect mini game)
public class NumberObjectMultiLineDnD : NumberObjectLineDnD, IPointerDownHandler
{
    [Header("Multi DnD vars.")]
    [SerializeField] private int leftValue;
    public int LeftValue
    {
        get { return leftValue; }
        set
        {
            leftValue = value;
        }
    }

    [SerializeField] private int rightValue;
    public int RightValue
    {
        get { return rightValue; }
        set
        {
            rightValue = value;
        }
    }

    [SerializeField] private GameObject visualRepresentation2;
    public Sprite visualSprite2;
    [SerializeField] private GameObject textRepresentation2;

    [SerializeField] private TextMeshProUGUI operatorText;

    private static readonly Vector3 singleModeScaleBack = new Vector3(0.82f, 1f, 1f);
    //private static readonly Vector3 singleModeScaleCommon = new Vector3(0.8f, 1f, 1f);

    private bool subtract;
    public bool Subtract
    {
        set
        {
            subtract = value;
            operatorText.text = subtract ? "-" : "+";
        }
    }

    public new void Start()
    {
        base.Start();

        switch (Mode)
        {
            case DisplayMode.SET:
                visualRepresentation2.SetActive(true);
                textRepresentation2.SetActive(false);
                visualRepresentation2.GetComponent<Image>().sprite = visualSprite2;
                feedbackBackground.gameObject.GetComponent<RectTransform>().localScale = singleModeScaleBack;
                //commonBackground.gameObject.GetComponent<RectTransform>().localScale = singleModeScaleCommon;
                break;
            case DisplayMode.MIXED:
                visualRepresentation2.SetActive(true);
                textRepresentation2.SetActive(true);
                visualRepresentation2.GetComponent<Image>().sprite = visualSprite2;
                textRepresentation.GetComponentInChildren<TextMeshProUGUI>().SetText("" + leftValue);
                textRepresentation2.GetComponentInChildren<TextMeshProUGUI>().SetText("" + rightValue);
                break;
            case DisplayMode.TEXT:
                visualRepresentation2.SetActive(false);
                textRepresentation2.SetActive(true);
                textRepresentation.GetComponentInChildren<TextMeshProUGUI>().SetText("" + leftValue);
                textRepresentation2.GetComponentInChildren<TextMeshProUGUI>().SetText("" + rightValue);
                feedbackBackground.gameObject.GetComponent<RectTransform>().localScale = singleModeScaleBack;
                //commonBackground.gameObject.GetComponent<RectTransform>().localScale = singleModeScaleCommon;
                break;
        }
    }

    public new void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Number Object - OnPointerDown (" + disableAudio + ")");
        if (disableAudio)
        {
            return;
        }

        //Debug.Log("Pointer position: " + eventData.position + "; test: " + (eventData.position.x - 410));
        if (eventData.position.x - 410 < -50)
        {
            // left side
            if (NumberRepresentationProvider.NumberAudio.Length < leftValue)
            {
                Debug.LogWarning("Missing audio output for number " + leftValue);
            }
            else if (PlayerPrefsController.ShouldPlayAudio())
            {
                SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.NumberAudio[leftValue - 1]);
            }
        }
        else if (eventData.position.x - 410 > 50)
        {
            // right side
            if (NumberRepresentationProvider.NumberAudio.Length < rightValue)
            {
                Debug.LogWarning("Missing audio output for number " + rightValue);
            }
            else if (PlayerPrefsController.ShouldPlayAudio())
            {
                SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.NumberAudio[rightValue - 1]);
            }
        }
        else if (PlayerPrefsController.ShouldPlayAudio())
        {
            if (subtract)
            {
                SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.MinusAudio);
            }
            else
            {
                SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.PlusAudio);
            }
        }
    }
}
