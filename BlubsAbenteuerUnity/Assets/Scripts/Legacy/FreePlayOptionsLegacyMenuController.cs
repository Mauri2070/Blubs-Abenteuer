using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreePlayOptionsLegacyMenuController : MonoBehaviour
{
    #region SerDecl
    [Header("Options pages")]
    [SerializeField] private GameObject gameTypeSelection;
    [SerializeField] private GameObject specialMemoryOptions;
    [SerializeField] private GameObject commonOptionsSelection;
    [SerializeField] private GameObject numberRepresentationSelection;
    [SerializeField] private GameObject specialOptionsSelection;

    [Header("Memory Options")]
    [SerializeField] private Slider memorySizeSlider;
    [SerializeField] private Slider memoryMatchSlider;

    [Header("Common Options")]
    [SerializeField] private Slider minValueSlider;
    [SerializeField] private TextMeshProUGUI minText;
    [SerializeField] private Slider maxValueSlider;
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private GameObject numberOfValuesSelector;
    [SerializeField] private Slider numberOfValuesSlider;
    [SerializeField] private TextMeshProUGUI numberOfValuesText;
    [SerializeField] private GameObject displayModeSelector;
    [SerializeField] private Slider displayModeSlider;

    [Header("Representation Options")]
    [SerializeField] private Slider alternativeRepresentationSlider;

    [Header("Special Options")]
    [SerializeField] private GameObject increasingSelector;
    [SerializeField] private Slider increasingSlider;
    [SerializeField] private GameObject extraValueSelector;
    [SerializeField] private Slider extraValueSlider;
    [SerializeField] private TextMeshProUGUI extraValueText;
    [SerializeField] private TextMeshProUGUI extraValueSliderText;
    [SerializeField] private GameObject nonSolValuesSelector;
    [SerializeField] private Slider nonSolValuesSlider;
    [SerializeField] private TextMeshProUGUI nonSolValueText;
    [SerializeField] private GameObject secondDisplayModeSelector;
    [SerializeField] private GameObject connectModeSelector;
    [SerializeField] private Slider connectModeSlider;

    [Header("Navigation Buttons")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;

    [Header("Scene Navigation")]
    [SerializeField] private SceneController sceneController;
    #endregion SerDecl

    private MiniGameOptions options;
    private int currentMenu;

    private readonly string insertMissingValueText = "Anzahl der fehlenden Werte";
    private readonly string addTargetValueText = "Zielzahl";

    private void Awake()
    {
        options = FreePlayOptionsSingleton.Instance.GameOptions;
        // setup menu
        currentMenu = 0;
        nextButton.SetActive(false);
        prevButton.SetActive(false);
        gameTypeSelection.SetActive(true);
        specialMemoryOptions.SetActive(false);
        commonOptionsSelection.SetActive(false);
        numberRepresentationSelection.SetActive(false);
        specialOptionsSelection.SetActive(false);

        // buttons setup
        memorySizeSlider.onValueChanged.RemoveAllListeners();
        memorySizeSlider.onValueChanged.AddListener(delegate { ChangeMemorySizeSlider(); });
        memoryMatchSlider.onValueChanged.RemoveAllListeners();
        memoryMatchSlider.onValueChanged.AddListener(delegate { ChangeMemoryMatchSlider(); });
        minValueSlider.onValueChanged.RemoveAllListeners();
        minValueSlider.onValueChanged.AddListener(delegate { ChangeMinValue(); });
        maxValueSlider.onValueChanged.RemoveAllListeners();
        maxValueSlider.onValueChanged.AddListener(delegate { ChangeMaxValue(); });
        numberOfValuesSlider.onValueChanged.RemoveAllListeners();
        numberOfValuesSlider.onValueChanged.AddListener(delegate { ChangeNumberOfValues(); });
        displayModeSlider.onValueChanged.RemoveAllListeners();
        displayModeSlider.onValueChanged.AddListener(delegate { ChangeDisplayMode(); });
        alternativeRepresentationSlider.onValueChanged.RemoveAllListeners();
        alternativeRepresentationSlider.onValueChanged.AddListener(delegate { ChangeAlternativeRepresentationSlider(); });
        increasingSlider.onValueChanged.RemoveAllListeners();
        increasingSlider.onValueChanged.AddListener(delegate { ChangeIncreasingSlider(); });
        extraValueSlider.onValueChanged.RemoveAllListeners();
        extraValueSlider.onValueChanged.AddListener(delegate { ChangeExtraValueSlider(); });
        nonSolValuesSlider.onValueChanged.RemoveAllListeners();
        nonSolValuesSlider.onValueChanged.AddListener(delegate { ChangeNonSolValueSlider(); });
        connectModeSlider.onValueChanged.RemoveAllListeners();
        connectModeSlider.onValueChanged.AddListener(delegate { ChangeConnectModeSlider(); });
    }

    #region MenuNavigation
    // Menu navigation
    public void NextMenu()
    {
        //Debug.Log("Executing NextMenu() from menu " + currentMenu);
        switch (currentMenu)
        {
            case 0:     // game type selection -> special mem / common value
                gameTypeSelection.SetActive(false);
                nextButton.SetActive(true);
                prevButton.SetActive(true);
                if (options.gameType == MiniGameType.MEMORY || options.gameType == MiniGameType.MEMORY_VS)
                {
                    specialMemoryOptions.SetActive(true);
                    SetupMemorySlider();
                }
                else
                {
                    currentMenu++;
                    NextMenu();
                    return;
                }
                break;
            case 1:     // special memory options -> common value
                if (options.gameType == MiniGameType.MEMORY || options.gameType == MiniGameType.MEMORY_VS)
                {
                    specialMemoryOptions.SetActive(false);
                    numberOfValuesSelector.SetActive(false);
                    displayModeSelector.SetActive(false);
                }
                else
                {
                    numberOfValuesSelector.SetActive(true);
                    displayModeSelector.SetActive(true);
                }
                commonOptionsSelection.SetActive(true);
                SetupValueSlider();
                break;
            case 2:     // common value -> representation/special options
                commonOptionsSelection.SetActive(false);
                if (options.displayMode == DisplayMode.TEXT && options.gameType != MiniGameType.PAIRS && options.gameType != MiniGameType.CONNECT && options.gameType != MiniGameType.CONNECT_VS)
                {
                    currentMenu++;
                    NextMenu();
                    return;
                }
                else
                {
                    numberRepresentationSelection.SetActive(true);
                    SetupAlternativeRepresentationSlider();
                    nextButton.SetActive(false);
                }
                break;
            case 3:     // representation -> special options
                numberRepresentationSelection.SetActive(false);
                nextButton.SetActive(true);
                specialOptionsSelection.SetActive(true);
                switch (options.gameType)
                {
                    case MiniGameType.INSERT:
                        increasingSelector.SetActive(true);
                        SetupIncreasingSlider();
                        extraValueSelector.SetActive(true);
                        SetupExtraValueSlider();
                        nonSolValuesSelector.SetActive(true);
                        SetupNonSolValueSlider();
                        secondDisplayModeSelector.SetActive(false);
                        connectModeSelector.SetActive(false);
                        break;
                    case MiniGameType.ADD:
                        increasingSelector.SetActive(false);
                        extraValueSelector.SetActive(true);
                        SetupExtraValueSlider();
                        nonSolValuesSelector.SetActive(true);
                        SetupNonSolValueSlider();
                        secondDisplayModeSelector.SetActive(false);
                        connectModeSelector.SetActive(false);
                        break;
                    case MiniGameType.PAIRS:
                        increasingSelector.SetActive(false);
                        extraValueSelector.SetActive(false);
                        nonSolValuesSelector.SetActive(false);
                        secondDisplayModeSelector.SetActive(true);
                        SetupDisplayModeSlider();
                        connectModeSelector.SetActive(false);
                        break;
                    case MiniGameType.COUNT_VS:
                    case MiniGameType.COUNT:
                        increasingSelector.SetActive(true);
                        SetupIncreasingSlider();
                        extraValueSelector.SetActive(false);
                        nonSolValuesSelector.SetActive(false);
                        secondDisplayModeSelector.SetActive(false);
                        connectModeSelector.SetActive(false);
                        break;
                    case MiniGameType.MEMORY_VS:
                    case MiniGameType.MEMORY:
                        if (!options.matchSetText)
                        {
                            numberRepresentationSelection.SetActive(true);
                            specialOptionsSelection.SetActive(false);
                            alternativeRepresentationSlider.gameObject.transform.parent.gameObject.SetActive(false);
                            nextButton.SetActive(false);
                        }
                        else
                        {
                            currentMenu++;
                            NextMenu();
                            return;
                        }
                        break;
                    case MiniGameType.CONNECT_VS:
                    case MiniGameType.CONNECT:
                        increasingSelector.SetActive(false);
                        extraValueSelector.SetActive(false);
                        nonSolValuesSelector.SetActive(false);
                        secondDisplayModeSelector.SetActive(true);
                        SetupDisplayModeSlider();
                        connectModeSelector.SetActive(true);
                        SetupConnectModeSlider();
                        break;
                }
                break;
            case 4:     // additional options -> free play
                switch (options.gameType)
                {
                    case MiniGameType.INSERT:
                        options.VerifyInsert();
                        break;
                    case MiniGameType.COUNT_VS:
                    case MiniGameType.COUNT:
                        options.VerifyCount();
                        break;
                    case MiniGameType.PAIRS:
                        options.VerifyPairs();
                        break;
                    case MiniGameType.ADD:
                        options.VerifyAdd();
                        break;
                    case MiniGameType.MEMORY_VS:
                    case MiniGameType.MEMORY:
                        options.VerifyMemory();
                        break;
                    case MiniGameType.CONNECT_VS:
                    case MiniGameType.CONNECT:
                        options.VerifyConnect();
                        break;
                }
                FreePlayOptionsSingleton.Instance.QuickPlay = false;
                sceneController.LoadFreePlayScene();
                return;
            default:
                Debug.LogError(currentMenu + " is not a valid FreePlay menu state to advance from!");
                return;
        }
        currentMenu++;
    }

    public void PrevMenu()
    {
        switch (currentMenu)
        {
            case 1:     // mem specific -> game type selection
                specialMemoryOptions.SetActive(false);
                gameTypeSelection.SetActive(true);

                prevButton.SetActive(false);
                nextButton.SetActive(false);
                break;
            case 2:     // common options -> type / mem specific
                commonOptionsSelection.SetActive(false);
                if (options.gameType == MiniGameType.MEMORY || options.gameType == MiniGameType.MEMORY_VS)
                {
                    specialMemoryOptions.SetActive(true);
                    SetupMemorySlider();
                }
                else
                {
                    currentMenu--;
                    PrevMenu();
                    return;
                }
                break;
            case 3:     // number rep -> common options
                numberRepresentationSelection.SetActive(false);
                nextButton.SetActive(true);
                commonOptionsSelection.SetActive(true);
                if (options.gameType == MiniGameType.MEMORY || options.gameType == MiniGameType.MEMORY_VS)
                {
                    numberOfValuesSelector.SetActive(false);
                }
                else
                {
                    numberOfValuesSelector.SetActive(true);
                }
                SetupValueSlider();
                break;
            case 4:     // additional options -> number rep/ common options
                specialOptionsSelection.SetActive(false);
                if (options.displayMode == DisplayMode.TEXT && options.gameType != MiniGameType.PAIRS && options.gameType != MiniGameType.CONNECT && options.gameType != MiniGameType.CONNECT_VS)
                {
                    currentMenu--;
                    PrevMenu();
                    return;
                }
                else
                {
                    numberRepresentationSelection.SetActive(true);
                    SetupAlternativeRepresentationSlider();
                }
                break;
            default:
                Debug.LogError(currentMenu + " is not a valid FreePlay menu state to go back from!");
                return;
        }
        currentMenu--;
    }
    #endregion MenuNavigation

    #region SelectorFunction
    // Game Type selection
    public void GameTypeButton(string type)
    {
        if (type.Equals("insert"))
        {
            SetGameType(MiniGameType.INSERT);
        }
        else if (type.Equals("count"))
        {
            SetGameType(MiniGameType.COUNT);
        }
        else if (type.Equals("pairs"))
        {
            SetGameType(MiniGameType.PAIRS);
        }
        else if (type.Equals("add"))
        {
            SetGameType(MiniGameType.ADD);
        }
        else if (type.Equals("memory"))
        {
            SetGameType(MiniGameType.MEMORY);
        }
        else if (type.Equals("connect"))
        {
            SetGameType(MiniGameType.CONNECT);
        }
        else if (type.Equals("connectVS"))
        {
            SetGameType(MiniGameType.CONNECT_VS);
        }
        else if (type.Equals("memoryVS"))
        {
            SetGameType(MiniGameType.MEMORY_VS);
        }
        else if (type.Equals("countVS"))
        {
            SetGameType(MiniGameType.COUNT_VS);
        }
        else
        {
            Debug.LogError(type + " is no valid MiniGameType!");
        }
    }

    private void SetGameType(MiniGameType type)
    {
        //Debug.Log("Setting gameType to " + type);
        options.gameType = type;
        NextMenu();
    }

    private void SetupMemorySlider()
    {
        switch (options.memorySize)
        {
            case MemoryMiniGame.MemorySize.SMALL:
                memorySizeSlider.value = 1;
                break;
            case MemoryMiniGame.MemorySize.MEDIUM:
                memorySizeSlider.value = 2;
                break;
            case MemoryMiniGame.MemorySize.LARGE:
                memorySizeSlider.value = 3;
                break;
        }
        memoryMatchSlider.value = options.matchSetText ? 2 : 1;
    }

    public void ChangeMemorySizeSlider()
    {
        switch ((int)memorySizeSlider.value)
        {
            case 1:
                options.memorySize = MemoryMiniGame.MemorySize.SMALL;
                break;
            case 2:
                options.memorySize = MemoryMiniGame.MemorySize.MEDIUM;
                break;
            case 3:
                options.memorySize = MemoryMiniGame.MemorySize.LARGE;
                break;
            default:
                Debug.LogError(memorySizeSlider.value + " is not a valid memory game size.");
                return;
        }
    }

    public void ChangeMemoryMatchSlider()
    {
        options.matchSetText = ((int)memoryMatchSlider.value) == 2;
    }

    // min/max value selection
    private void SetupValueSlider()
    {
        options.VerifyMinMax();
        if (options.gameType == MiniGameType.MEMORY || options.gameType == MiniGameType.MEMORY_VS)
        {
            switch (options.memorySize)
            {
                case MemoryMiniGame.MemorySize.SMALL:
                    maxValueSlider.minValue = options.minValue + 4;
                    minValueSlider.maxValue = 16;
                    break;
                case MemoryMiniGame.MemorySize.MEDIUM:
                    maxValueSlider.minValue = options.minValue + 10;
                    minValueSlider.maxValue = 10;
                    break;
                case MemoryMiniGame.MemorySize.LARGE:
                    maxValueSlider.minValue = options.minValue + 14;
                    minValueSlider.maxValue = 6;
                    break;
            }
        }
        else if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
        {
            // td: - mode
            minValueSlider.maxValue = Mathf.Floor(maxValueSlider.value / 2);
            maxValueSlider.minValue = 2;
        }
        else
        {
            minValueSlider.maxValue = 19;
            maxValueSlider.minValue = 2;
        }
        minValueSlider.value = options.minValue;
        minText.text = "" + options.minValue;
        maxValueSlider.value = options.maxValue;
        maxText.text = "" + options.maxValue;

        numberOfValuesSlider.value = options.numberOfValues;
        numberOfValuesText.text = "" + options.numberOfValues;
        if (options.gameType == MiniGameType.PAIRS)
        {
            numberOfValuesSlider.minValue = 1;
            numberOfValuesSlider.maxValue = Mathf.Min(options.maxValue - options.minValue + 1, 7);
        }
        else if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
        {
            // td: - mode
            numberOfValuesSlider.minValue = 1;
            int n = options.maxValue - 2 * options.minValue + 2;
            numberOfValuesSlider.maxValue = Mathf.Min(n * n / 4, 7);
        }
        else
        {
            numberOfValuesSlider.minValue = 2;
            numberOfValuesSlider.maxValue = options.maxValue - options.minValue + 1;
        }
    }

    public void ChangeMinValue()
    {
        options.minValue = (int)minValueSlider.value;
        SetupValueSlider();
    }

    public void ChangeMaxValue()
    {
        options.maxValue = (int)maxValueSlider.value;
        SetupValueSlider();
    }

    public void ChangeNumberOfValues()
    {
        options.numberOfValues = (int)numberOfValuesSlider.value;
        SetupValueSlider();
    }

    private void SetupAlternativeRepresentationSlider()
    {
        alternativeRepresentationSlider.gameObject.transform.parent.gameObject.SetActive(true);
        if (options.alternativeRepresentation)
        {
            alternativeRepresentationSlider.value = 1;
        }
        else
        {
            alternativeRepresentationSlider.value = 2;
        }
    }

    public void ChangeAlternativeRepresentationSlider()
    {
        options.alternativeRepresentation = ((int)alternativeRepresentationSlider.value) == 2;
        //SetupAlternativeRepresentationSlider();
    }

    private void SetupDisplayModeSlider()
    {
        switch (options.displayMode)
        {
            case DisplayMode.SET:
                displayModeSlider.value = 1;
                break;
            case DisplayMode.MIXED:
                displayModeSlider.value = 2;
                break;
            case DisplayMode.TEXT:
                displayModeSlider.value = 3;
                break;
        }
    }

    public void ChangeDisplayMode()
    {
        switch ((int)displayModeSlider.value)
        {
            case 1:
                options.displayMode = DisplayMode.SET;
                break;
            case 2:
                options.displayMode = DisplayMode.MIXED;
                break;
            case 3:
                options.displayMode = DisplayMode.TEXT;
                break;
            default:
                Debug.LogError(((int)displayModeSlider.value) + " is not a valid value for display mode selection.");
                break;
        }
        SetupDisplayModeSlider();
    }

    // number representation selection
    public void NumberRepresentationButton(string rep)
    {
        if (rep.Equals("random"))
        {
            SetNumberRepresentation(NumberRepresentation.RANDOM);
        }
        else if (rep.Equals("cups"))
        {
            SetNumberRepresentation(NumberRepresentation.HANDS);
        }
        else
        {
            Debug.LogError(rep + " is no valid NumberRepresentation!");
        }
    }

    private void SetNumberRepresentation(NumberRepresentation rep)
    {
        if (currentMenu == 3)
        {
            options.numberRepresentation = rep;
        }
        else
        {
            options.secondRepresentation = rep;
        }
        NextMenu();
    }

    private void SetupIncreasingSlider()
    {
        if (options.increasing)
        {
            increasingSlider.value = 1;
        }
        else
        {
            increasingSlider.value = 2;
        }
    }

    public void ChangeIncreasingSlider()
    {
        options.increasing = ((int)increasingSlider.value) == 1;
        SetupIncreasingSlider();
    }

    private void SetupExtraValueSlider()
    {
        switch (options.gameType)
        {
            case MiniGameType.INSERT:
                extraValueText.text = insertMissingValueText;
                extraValueSlider.minValue = 1;
                extraValueSlider.maxValue = options.numberOfValues - 1;
                options.numberMissingValues = (int)Mathf.Max(Mathf.Min(options.numberMissingValues, extraValueSlider.maxValue), extraValueSlider.minValue);
                extraValueSlider.value = options.numberMissingValues;
                break;
            case MiniGameType.ADD:
                extraValueText.text = addTargetValueText;
                extraValueSlider.minValue = 2;
                extraValueSlider.maxValue = 20;
                options.targetValue = (int)Mathf.Max(Mathf.Min(options.targetValue, extraValueSlider.maxValue), extraValueSlider.minValue);
                extraValueSlider.value = options.targetValue;
                break;
            default:
                Debug.LogWarning(options.gameType + "doesn't support an extra value.");
                break;
        }
        extraValueSliderText.text = "" + extraValueSlider.value;
    }

    public void ChangeExtraValueSlider()
    {
        switch (options.gameType)
        {
            case MiniGameType.INSERT:
                options.numberMissingValues = (int)extraValueSlider.value;
                break;
            case MiniGameType.ADD:
                options.targetValue = (int)extraValueSlider.value;
                break;
            default:
                Debug.LogWarning(options.gameType + "doesn't support an extra value.");
                break;
        }
        SetupExtraValueSlider();
    }

    private void SetupNonSolValueSlider()
    {
        nonSolValuesSlider.value = options.numberOfNonSolutionValues;
        nonSolValueText.text = "" + nonSolValuesSlider.value;
    }

    public void ChangeNonSolValueSlider()
    {
        options.numberOfNonSolutionValues = (int)nonSolValuesSlider.value;
        SetupNonSolValueSlider();
    }

    private void SetupConnectModeSlider()
    {
        connectModeSlider.value = options.subtract ? 2 : 1;
    }

    public void ChangeConnectModeSlider()
    {
        options.subtract = connectModeSlider.value == 2;
        SetupConnectModeSlider();
    }
    #endregion SelectorFunction
}
