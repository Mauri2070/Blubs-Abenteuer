using UnityEngine;
using UnityEngine.UI;
using TMPro;

// script handling options made for free play mode
public class FreePlayOptionsMenuController : MonoBehaviour
{
    #region SerDecl
    [Header("Options pages")]
    [SerializeField] private GameObject gameTypeSelection;
    [SerializeField] private GameObject mandatoryOptions;
    [SerializeField] private GameObject optionalOptions;

    [Header("Mandatory Options")]
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Slider displayModeSlider;

    [Header("Optional Options")]
    [SerializeField] private Slider minValueSlider;
    [SerializeField] private TextMeshProUGUI minText;
    [SerializeField] private Slider maxValueSlider;
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private GameObject increasingSelector;
    [SerializeField] private Slider increasingSlider;
    [SerializeField] private GameObject connectModeSelector;
    [SerializeField] private Slider connectModeSlider;

    [Header("Navigation Buttons")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject startGameButton;

    [Header("Scene Navigation")]
    [SerializeField] private SceneController sceneController;
    #endregion SerDecl

    private MiniGameOptions options;
    private int currentMenu;

    private void Awake()
    {
        options = FreePlayOptionsSingleton.Instance.GameOptions;
        // setup menu
        currentMenu = 0;
        nextButton.SetActive(false);
        prevButton.SetActive(false);
        startGameButton.SetActive(false);
        gameTypeSelection.SetActive(true);
        mandatoryOptions.SetActive(false);
        optionalOptions.SetActive(false);

        // buttons setup
        difficultySlider.onValueChanged.RemoveAllListeners();
        difficultySlider.value = PlayerPrefsController.GetQuickPlayDifficulty() + 1;
        difficultySlider.onValueChanged.AddListener(delegate { ChangeFreePlayDifficulty(); });
        minValueSlider.onValueChanged.RemoveAllListeners();
        minValueSlider.onValueChanged.AddListener(delegate { ChangeMinValue(); });
        maxValueSlider.onValueChanged.RemoveAllListeners();
        maxValueSlider.onValueChanged.AddListener(delegate { ChangeMaxValue(); }); ;
        displayModeSlider.onValueChanged.RemoveAllListeners();
        displayModeSlider.onValueChanged.AddListener(delegate { ChangeDisplayMode(); });
        increasingSlider.onValueChanged.RemoveAllListeners();
        increasingSlider.onValueChanged.AddListener(delegate { ChangeIncreasingSlider(); });
        connectModeSlider.onValueChanged.RemoveAllListeners();
        connectModeSlider.onValueChanged.AddListener(delegate { ChangeConnectModeSlider(); });
    }

    #region MenuNavigation
    // Menu navigation
    public void NextMenu()
    {
        switch (currentMenu)
        {
            case 0:     // game type selection -> diff/repr
                gameTypeSelection.SetActive(false);
                mandatoryOptions.SetActive(true);
                nextButton.SetActive(true);
                prevButton.SetActive(true);
                startGameButton.SetActive(true);
                SetupDisplayModeSlider();
                ChangeFreePlayDifficulty();
                break;
            case 1:     // diff/repr -> advanced opt.
                mandatoryOptions.SetActive(false);
                optionalOptions.SetActive(true);
                SetupValueSlider();
                if (options.gameType == MiniGameType.INSERT || options.gameType == MiniGameType.COUNT || options.gameType == MiniGameType.COUNT_VS)
                {
                    increasingSelector.SetActive(true);
                    SetupIncreasingSlider();
                }
                else
                {
                    increasingSelector.SetActive(false);
                }
                if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
                {
                    connectModeSelector.SetActive(true);
                    SetupConnectModeSlider();
                }
                else
                {
                    connectModeSelector.SetActive(false);
                }
                break;
            case 2:     // adv. opt -> game start
                StartGameWithCurrentOptions();
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
            case 1:     // diff/rep -> game type selection
                mandatoryOptions.SetActive(false);
                nextButton.SetActive(false);
                prevButton.SetActive(false);
                startGameButton.SetActive(false);
                gameTypeSelection.SetActive(true);
                break;
            case 2:     // adv. opt -> diff rep
                optionalOptions.SetActive(false);
                mandatoryOptions.SetActive(true);
                SetupDisplayModeSlider();
                ChangeFreePlayDifficulty();
                break;
            default:
                Debug.LogError(currentMenu + " is not a valid FreePlay menu state to go back from!");
                return;
        }
        currentMenu--;
    }

    public void StartGameWithCurrentOptions()
    {
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
        FreePlayOptionsSingleton.Instance.ParentMode = true;
        sceneController.LoadFreePlayScene();
    }

    public void ChildStartFreePlayGame(string gameType)
    {
        // set game Type
        if (gameType.Equals("insert"))
        {
            options.gameType = MiniGameType.INSERT;
        }
        else if (gameType.Equals("count"))
        {
            options.gameType = MiniGameType.COUNT;
        }
        else if (gameType.Equals("pairs"))
        {
            options.gameType = MiniGameType.PAIRS;
        }
        else if (gameType.Equals("add"))
        {
            options.gameType = MiniGameType.ADD;
        }
        else if (gameType.Equals("memory"))
        {
            options.gameType = MiniGameType.MEMORY;
        }
        else if (gameType.Equals("connect"))
        {
            options.gameType = MiniGameType.CONNECT;
        }
        else if (gameType.Equals("connectVS"))
        {
            options.gameType = MiniGameType.CONNECT_VS;
        }
        else if (gameType.Equals("memoryVS"))
        {
            options.gameType = MiniGameType.MEMORY_VS;
        }
        else if (gameType.Equals("countVS"))
        {
            options.gameType = MiniGameType.COUNT_VS;
        }
        else
        {
            Debug.LogError(gameType + " is no valid MiniGameType!");
        }

        // set options according to help difficulty
        // larger -> more help -> easier
        int progressDif = PlayerPrefsController.LoadRoomIdx(Room.HUB) <= 11 && !PlayerPrefsController.WasStoryCompleted() ? 2 : PlayerPrefsController.LoadRoomIdx(Room.HUB) <= 13 && !PlayerPrefsController.WasStoryCompleted() ? 1 : 0;    // lock to story progress
        int dif = Mathf.Max(PlayerPrefsController.GetHelpDifficulty(), progressDif);
        //Debug.Log(dif);
        options.minValue = 1;
        options.maxValue = 10 + 5 * (2 - dif);
        options.displayMode = dif > 0 ? DisplayMode.MIXED : DisplayMode.TEXT;
        options.numberRepresentation = dif > 0 ? NumberRepresentation.RANDOM : NumberRepresentation.MIXED;
        options.secondRepresentation = options.numberRepresentation;
        options.increasing = dif == 2 ? true : Random.Range(0, 100) < 50;
        options.subtract = dif == 2 ? false : Random.Range(0, 100) < 50;
        options.numberOfNonSolutionValues = 2 * (2 - dif);

        options.VerifyMinMax();

        switch (options.gameType)
        {
            case MiniGameType.INSERT:
                options.numberOfValues = 7 - dif;
                options.numberMissingValues = 3 - dif;
                options.VerifyInsert();
                break;
            case MiniGameType.COUNT:
            case MiniGameType.COUNT_VS:
                options.numberOfValues = 5 + 5 * (2 - dif);
                options.VerifyCount();
                break;
            case MiniGameType.PAIRS:
                options.numberOfValues = 6 - 2 * dif;
                options.VerifyPairs();
                break;
            case MiniGameType.ADD:
                options.numberOfValues = 6 - 2 * (2 - dif);
                options.targetValue = Random.Range(options.minValue + 1, options.maxValue);
                options.VerifyAdd();
                break;
            case MiniGameType.MEMORY:
            case MiniGameType.MEMORY_VS:
                options.memorySize = (MemoryMiniGame.MemorySize)(2 - dif);
                options.matchSetText = dif == 2 ? false : Random.Range(0, 100) < 50;
                options.VerifyMemory();
                break;
            case MiniGameType.CONNECT:
            case MiniGameType.CONNECT_VS:
                options.numberOfValues = 6 - 2 * dif;
                options.VerifyConnect();
                break;
        }
        FreePlayOptionsSingleton.Instance.QuickPlay = false;
        FreePlayOptionsSingleton.Instance.ParentMode = false;
        sceneController.LoadFreePlayScene();
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

    public void ChangeFreePlayDifficulty()
    {
        PlayerPrefsController.SetQuickPlayDifficulty((int)difficultySlider.value - 1);
        options.alternativeRepresentation = true;
        options.numberRepresentation = NumberRepresentation.RANDOM;
        options.secondRepresentation = NumberRepresentation.RANDOM;
        switch ((int)difficultySlider.value - 1)
        {
            case 0:     // easy
                options.rightSideDisplayMode = DisplayMode.MIXED;
                options.numberAudioActive = true;
                if (options.gameType == MiniGameType.ADD)
                {
                    options.numberOfValues = 2;
                    options.numberOfNonSolutionValues = 0;
                }
                else if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
                {
                    options.numberOfValues = 2;
                }
                else if (options.gameType == MiniGameType.COUNT || options.gameType == MiniGameType.COUNT_VS)
                {
                    options.numberOfValues = 4;
                }
                else if (options.gameType == MiniGameType.INSERT)
                {
                    options.numberOfValues = 4;
                    options.numberMissingValues = 2;
                    options.numberOfNonSolutionValues = 0;
                }
                else if (options.gameType == MiniGameType.PAIRS)
                {
                    options.numberOfValues = 2;
                }
                else // MEMORY
                {
                    options.memorySize = MemoryMiniGame.MemorySize.SMALL;
                }
                break;
            case 1:     // medium
                options.rightSideDisplayMode = options.displayMode;
                options.numberAudioActive = false;
                if (options.gameType == MiniGameType.ADD)
                {
                    options.numberOfValues = 4;
                    options.numberOfNonSolutionValues = 3;
                }
                else if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
                {
                    options.numberOfValues = 4;
                }
                else if (options.gameType == MiniGameType.COUNT || options.gameType == MiniGameType.COUNT_VS)
                {
                    options.numberOfValues = 8;
                }
                else if (options.gameType == MiniGameType.INSERT)
                {
                    options.numberOfValues = 7;
                    options.numberMissingValues = 3;
                    options.numberOfNonSolutionValues = 2;
                }
                else if (options.gameType == MiniGameType.PAIRS)
                {
                    options.numberOfValues = 4;
                }
                else // MEMORY
                {
                    options.memorySize = MemoryMiniGame.MemorySize.MEDIUM;
                }
                break;
            case 2:     // hard
                options.rightSideDisplayMode = DisplayMode.TEXT;
                options.numberAudioActive = false;
                if (options.gameType == MiniGameType.ADD)
                {
                    options.numberOfValues = 7;
                    options.numberOfNonSolutionValues = 7;
                }
                else if (options.gameType == MiniGameType.CONNECT || options.gameType == MiniGameType.CONNECT_VS)
                {
                    options.numberOfValues = 4;
                }
                else if (options.gameType == MiniGameType.COUNT || options.gameType == MiniGameType.COUNT_VS)
                {
                    options.numberOfValues = 15;
                }
                else if (options.gameType == MiniGameType.INSERT)
                {
                    options.numberOfValues = 7;
                    options.numberMissingValues = 5;
                    options.numberOfNonSolutionValues = 6;
                }
                else if (options.gameType == MiniGameType.PAIRS)
                {
                    options.numberOfValues = 4;
                }
                else // MEMORY
                {
                    options.memorySize = MemoryMiniGame.MemorySize.LARGE;
                }
                break;
        }
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

        if (options.gameType == MiniGameType.ADD)
        {
            options.targetValue = Mathf.RoundToInt(options.minValue + (Random.value * options.maxValue));
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

    // display mode
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
                options.rightSideDisplayMode = DisplayMode.SET;
                options.matchSetText = false;
                break;
            case 2:
                options.displayMode = DisplayMode.MIXED;
                options.rightSideDisplayMode = DisplayMode.MIXED;
                options.matchSetText = true;
                break;
            case 3:
                options.displayMode = DisplayMode.TEXT;
                options.rightSideDisplayMode = DisplayMode.TEXT;
                options.matchSetText = true;
                break;
            default:
                Debug.LogError(((int)displayModeSlider.value) + " is not a valid value for display mode selection.");
                break;
        }
        SetupDisplayModeSlider();
    }

    // increasing/decreasing
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

    // +/-
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
