using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

// script controlling canvas elements during mini games in free play/ quick play mode
public class FreePlayGamesCanvasController : StoryGamesCanvasController
{
    [Header("MiniGame objects")]
    [SerializeField]
    [Tooltip("Object parents in order: insert, count, paris, add, memory, connect, memoryVS, countVS, connectVS")]
    private GameObject[] gameObjectParentsFP;

    [Header("Task texts")]
    [SerializeField] private OutputDistributer taskTextDistributerFP;
    [SerializeField]
    [Tooltip("OuputContainer for Task texts in order: Insert-inc, Count-inc, Pairs, Add, Memory, Connect, Insert-dec, Count-dec, memoryVS, countVs, connectVS")]
    private OutputContainer[] taskTextsFP;

    [Header("Menus")]
    [SerializeField] private GameObject menuParentFP;
    [SerializeField] private GameObject gameEndMenuFP;
    [SerializeField] private GameObject explanationMenuFP;
    [SerializeField] private GameObject pauseMenuFP;
    [SerializeField] private GameObject stillPlayingMenuFP;

    [Header("Menu Elements")]
    [SerializeField] private GameObject taskDescriptionObjectsFP;
    [SerializeField] private GameObject increasingIndicatorFP;
    [SerializeField] private GameObject decreasingIndicatorFP;

    [Header("Game End Menu Elements")]
    [SerializeField] private OutputDistributer motivationalMessageDistributerFP;
    [SerializeField] private OutputContainer motivationalMessageDummyFP;
    [SerializeField] private GameObject star1FP;
    [SerializeField] private GameObject star2FP;
    [SerializeField] private GameObject star3FP;
    [SerializeField] private GameObject nextGameObjects;

    [Header("Motivational messages")]
    [SerializeField] private OutputContainer[] motivationalMessages1FP;
    [SerializeField] private OutputContainer[] motivationalMessages2FP;
    [SerializeField] private OutputContainer[] motivationalMessages3FP;
    [SerializeField] private OutputContainer memoryChildWinMessageFP;
    [SerializeField] private OutputContainer memoryParentWinMessageFP;
    [SerializeField] private OutputContainer memoryDrawMessageFP;
    [SerializeField] private OutputContainer childFasterFP;
    [SerializeField] private OutputContainer parentFasterFP;
    private OutputContainer lastMM = null;

    [Header("Explanation Menu Elements")]
    [SerializeField] private VideoPlayer videoPlayerFP;
    [SerializeField] private VideoClip InsertGIF_FP;
    [SerializeField] private VideoClip CountGIF_FP;
    [SerializeField] private VideoClip PairsGIF_FP;
    [SerializeField] private VideoClip AddGIF_FP;
    [SerializeField] private VideoClip MemoryGIF_FP;
    [SerializeField] private VideoClip ConnectGIF_FP;
    [SerializeField] private VideoClip AdditionGIF_FP;
    [SerializeField] private VideoClip SubtractionGIF_FP;
    [SerializeField] private GameObject additionButtonFP;
    [SerializeField] private GameObject subtractionButtonFP;

    private HelpSystem helpSystem;

    private int currentIdx;

    private void Awake()
    {
        StartMiniGame();
    }

    #region MiniGames
    private InsertMiniGame insertMiniGame;
    private CountMiniGame countMiniGame;
    private PairsMiniGame pairsMiniGame;
    private AddMiniGame addMiniGame;
    private MemoryMiniGame memoryMiniGame;
    private ConnectMiniGame connectMiniGame;

    private MemoryVsMiniGame memoryVsMiniGame;
    private CountVsMiniGame countVsMiniGame;
    private ConnectVsMiniGame connectVsMiniGame;

    private MiniGameOptions currentOptions;

    public void StartMiniGame()
    {
        menuParentFP.SetActive(false);
        taskDescriptionObjectsFP.SetActive(true);

        currentOptions = FreePlayOptionsSingleton.Instance.GameOptions;
        Debug.Log("Starting MiniGame: " + currentOptions);
        additionButtonFP.SetActive(false);
        subtractionButtonFP.SetActive(false);
        increasingIndicatorFP.SetActive(false);
        decreasingIndicatorFP.SetActive(false);
        switch (currentOptions.gameType)
        {
            case MiniGameType.INSERT:
                if (insertMiniGame == null)
                {
                    insertMiniGame = FindObjectOfType<InsertMiniGame>();
                }
                insertMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 0;
                taskTextDistributerFP.output = (currentOptions.increasing ? taskTextsFP[0] : taskTextsFP[6]);
                if (currentOptions.increasing)
                {
                    increasingIndicatorFP.SetActive(true);
                    increasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicatorFP.SetActive(true);
                    decreasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.COUNT:
                if (countMiniGame == null)
                {
                    countMiniGame = FindObjectOfType<CountMiniGame>();
                }
                countMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 1;
                taskTextDistributerFP.output = (currentOptions.increasing ? taskTextsFP[1] : taskTextsFP[7]);
                if (currentOptions.increasing)
                {
                    increasingIndicatorFP.SetActive(true);
                    increasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicatorFP.SetActive(true);
                    decreasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.PAIRS:
                if (pairsMiniGame == null)
                {
                    pairsMiniGame = FindObjectOfType<PairsMiniGame>();
                }
                pairsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 2;
                taskTextDistributerFP.output = taskTextsFP[2];
                break;
            case MiniGameType.ADD:
                if (addMiniGame == null)
                {
                    addMiniGame = FindObjectOfType<AddMiniGame>();
                }
                addMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 3;
                taskTextDistributerFP.output = taskTextsFP[3];
                break;
            case MiniGameType.MEMORY:
                if (memoryMiniGame == null)
                {
                    memoryMiniGame = FindObjectOfType<MemoryMiniGame>();
                }
                memoryMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 4;
                taskTextDistributerFP.output = taskTextsFP[4];
                break;
            case MiniGameType.CONNECT:
                if (connectMiniGame == null)
                {
                    connectMiniGame = FindObjectOfType<ConnectMiniGame>();
                }
                connectMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 5;
                taskTextDistributerFP.output = taskTextsFP[5];
                if (currentOptions.subtract)
                {
                    subtractionButtonFP.SetActive(true);
                }
                else
                {
                    additionButtonFP.SetActive(true);
                }
                break;
            case MiniGameType.MEMORY_VS:
                if (memoryVsMiniGame == null)
                {
                    memoryVsMiniGame = FindObjectOfType<MemoryVsMiniGame>();
                }
                memoryVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 6;
                taskTextDistributerFP.output = taskTextsFP[8];
                break;
            case MiniGameType.COUNT_VS:
                if (countVsMiniGame == null)
                {
                    countVsMiniGame = FindObjectOfType<CountVsMiniGame>();
                }
                countVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 7;
                //taskTextDistributer.output = (currentOptions.increasing ? taskTexts[1] : taskTexts[7]);
                taskTextDistributerFP.output = taskTextsFP[9];
                if (currentOptions.increasing)
                {
                    increasingIndicatorFP.SetActive(true);
                    increasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicatorFP.SetActive(true);
                    decreasingIndicatorFP.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.CONNECT_VS:
                if (connectVsMiniGame == null)
                {
                    connectVsMiniGame = FindObjectOfType<ConnectVsMiniGame>();
                }
                connectVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 8;
                taskTextDistributerFP.output = taskTextsFP[10];
                if (currentOptions.subtract)
                {
                    subtractionButtonFP.SetActive(true);
                }
                else
                {
                    additionButtonFP.SetActive(true);
                }
                break;
            default:
                Debug.LogError("MiniGame " + currentOptions.gameType + " is not supported by StoryGamesCanvasController");
                return;
        }
        ActivateParent(currentIdx);
        taskTextDistributerFP.Distribute();
        // activate task description for VS-Audio after defined period of not playing it
        if((currentOptions.gameType == MiniGameType.MEMORY_VS || currentOptions.gameType == MiniGameType.CONNECT_VS || currentOptions.gameType == MiniGameType.COUNT_VS) && PlayerPrefsController.ShouldVsAudioPlay(currentOptions.gameType))
        {
            SoundControllerSingleton.CancelAudio();
            taskTextDistributerFP.PlayAudio();
            PlayerPrefsController.VsAudioPlayed(currentOptions.gameType);
        }
    }

    private void ActivateParent(int idx)
    {
        for (int i = 0; i < gameObjectParentsFP.Length; i++)
        {
            if (i == idx)
            {
                gameObjectParentsFP[i].SetActive(true);
            }
            else
            {
                gameObjectParentsFP[i].SetActive(false);
            }
        }
    }

    public void NextQuickPlayGame()
    {
        FreePlayOptionsSingleton.GenerateRandomizedOptions();
        StartMiniGame();
    }

    public override void EndMiniGame()
    {
        //Debug.Log("Calling FreePlayGamesCanvasController.EndMiniGame()");
        OpenGameEndMenu();
    }

    public override void LeaveMiniGame()
    {
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.MiniGameCompleted());
        SceneController sceneController = FindObjectOfType<SceneController>();
        sceneController.LoadMainMenu();
    }

    public override void RestartMiniGame()
    {
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.MiniGameCompleted());
        FreePlayOptionsSingleton.GenerateRandomizedOptions(true);
        StartMiniGame();
    }
    #endregion MiniGames

    #region Menus
    // Game End Menu
    private System.Random rand = new System.Random();
    public override void OpenGameEndMenu()
    {
        OpenMenu();
        gameEndMenuFP.SetActive(true);
        taskDescriptionObjectsFP.SetActive(false);

        if (FreePlayOptionsSingleton.Instance.QuickPlay)
        {
            nextGameObjects.SetActive(true);
        }
        else
        {
            nextGameObjects.SetActive(false);
        }

        // djust+display motivational message
        OutputContainer motivationalMessage = motivationalMessageDummyFP;
        if (currentOptions.gameType == MiniGameType.MEMORY_VS)
        {
            switch (memoryVsMiniGame.GetWinner())
            {
                case 0:
                    // child won
                    motivationalMessage = memoryChildWinMessageFP;
                    break;
                case 1:
                    // draw
                    motivationalMessage = memoryDrawMessageFP;
                    break;
                case 2:
                    // parent won
                    motivationalMessage = memoryParentWinMessageFP;
                    break;
                default:
                    Debug.LogWarning("MemoryVsMiniGame.GetWinner not in Range [0,2]!");
                    break;
            }
            star1FP.SetActive(true);
            star2FP.SetActive(true);
            star3FP.SetActive(true);
        }
        else if (currentOptions.gameType == MiniGameType.CONNECT_VS)
        {
            if (connectVsMiniGame.ChildFaster)
            {
                // child faster
                motivationalMessage = childFasterFP;
            }
            else
            {
                // parent faster
                motivationalMessage = parentFasterFP;
            }
            star1FP.SetActive(true);
            star2FP.SetActive(true);
            star3FP.SetActive(true);
        }
        else if (currentOptions.gameType == MiniGameType.COUNT_VS)
        {
            if (countVsMiniGame.ChildFaster)
            {
                // child faster
                motivationalMessage = childFasterFP;
            }
            else
            {
                // parent faster
                motivationalMessage = parentFasterFP;
            }
            star1FP.SetActive(true);
            star2FP.SetActive(true);
            star3FP.SetActive(true);

        }
        else if (helpSystem.WrongInputs <= optimalThreashold)
        {
            // 3 stars
            do
            {
                motivationalMessage = motivationalMessages3FP[rand.Next(0, motivationalMessages3FP.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1FP.SetActive(true);
            star2FP.SetActive(true);
            star3FP.SetActive(true);
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Good), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        else if (helpSystem.WrongInputs <= suboptimalThreshold)
        {
            // 2 stars
            do
            {
                motivationalMessage = motivationalMessages2FP[rand.Next(0, motivationalMessages2FP.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1FP.SetActive(true);
            star2FP.SetActive(true);
            star3FP.SetActive(false);
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Normal), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        else
        {
            // 1 star
            do
            {
                motivationalMessage = motivationalMessages1FP[rand.Next(0, motivationalMessages1FP.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1FP.SetActive(true);
            star2FP.SetActive(false);
            star3FP.SetActive(false);
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Bad), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        lastMM = motivationalMessage;
        motivationalMessageDistributerFP.output = motivationalMessage;
        motivationalMessageDistributerFP.Distribute();
        motivationalMessageDistributerFP.PlayAudio();
    }

    // Pause Menu
    public override void OpenPauseMenu()
    {
        OpenMenu();
        pauseMenuFP.SetActive(true);
    }

    public override void ClosePauseMenu()
    {
        CloseMenu();
    }

    public override void TogglePauseMenu()
    {
        if (pauseMenuFP.activeInHierarchy)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    // Explanation Menu
    public override void OpenExplanationMenu()
    {
        OpenMenu();
        explanationMenuFP.SetActive(true);
        switch (currentOptions.gameType)
        {
            case MiniGameType.INSERT:
                videoPlayerFP.clip = InsertGIF_FP;
                break;
            case MiniGameType.COUNT_VS:
            case MiniGameType.COUNT:
                videoPlayerFP.clip = CountGIF_FP;
                break;
            case MiniGameType.PAIRS:
                videoPlayerFP.clip = PairsGIF_FP;
                break;
            case MiniGameType.ADD:
                videoPlayerFP.clip = AddGIF_FP;
                break;
            case MiniGameType.MEMORY_VS:
            case MiniGameType.MEMORY:
                videoPlayerFP.clip = MemoryGIF_FP;
                break;
            case MiniGameType.CONNECT_VS:
            case MiniGameType.CONNECT:
                showAdditionalExplanation = false;
                videoPlayerFP.clip = ConnectGIF_FP;
                break;
            default:
                Debug.LogWarning("No explanation video for " + currentOptions.gameType);
                return;
        }
        videoPlayerFP.Play();
    }

    public override void CloseExplanationMenu()
    {
        videoPlayerFP.Stop();
        CloseMenu();
    }

    public override void ToggleVideoPlayerPlaying()
    {
        if (videoPlayerFP.isPlaying)
        {
            videoPlayerFP.Pause();
        }
        else if (videoPlayerFP.isPaused)
        {
            videoPlayerFP.Play();
        }
        else
        {
            videoPlayerFP.time = 0;
            videoPlayerFP.Play();
        }
    }

    public override void ToggleExplanationMenu()
    {
        SoundControllerSingleton.CancelAudio();
        if (explanationMenuFP.activeInHierarchy)
        {
            CloseExplanationMenu();
        }
        else
        {
            OpenExplanationMenu();
        }
    }

    private bool showAdditionalExplanation = false;
    public override void ShowAdditionExplanation()
    {
        videoPlayerFP.Stop();
        if (showAdditionalExplanation)
        {
            videoPlayerFP.clip = ConnectGIF_FP;
        }
        else
        {
            videoPlayerFP.clip = AdditionGIF_FP;
        }
        videoPlayerFP.Play();
        showAdditionalExplanation = !showAdditionalExplanation;
    }

    public override void ShowSubtractionExplanation()
    {
        videoPlayerFP.Stop();
        if (showAdditionalExplanation)
        {
            videoPlayerFP.clip = ConnectGIF_FP;
        }
        else
        {
            videoPlayerFP.clip = SubtractionGIF_FP;
        }
        videoPlayerFP.Play();
        showAdditionalExplanation = !showAdditionalExplanation;
    }

    // Still Playing Menu
    public override void OpenStillPlayingMenu()
    {
        if (stillPlayingMenuFP.activeInHierarchy)
        {
            return;
        }
        OpenMenu();
        stillPlayingMenuFP.SetActive(true);
    }

    public override void CloseStillPlayingMenu()
    {
        CloseMenu();
    }

    // general
    private void OpenMenu()
    {
        gameObjectParentsFP[currentIdx].SetActive(false);
        if (helpSystem == null)
        {
            helpSystem = FindObjectOfType<HelpSystem>();
        }
        helpSystem.TmpHideHand();

        menuParentFP.SetActive(true);
        gameEndMenuFP.SetActive(false);
        explanationMenuFP.SetActive(false);
        pauseMenuFP.SetActive(false);
        stillPlayingMenuFP.SetActive(false);
    }

    private void CloseMenu()
    {
        gameObjectParentsFP[currentIdx].SetActive(true);
        helpSystem.DisplayHandAgain();
        menuParentFP.SetActive(false);
    }
    #endregion Menus
}
