using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

// UI controller script for game UI in story mode
public class StoryGamesCanvasController : MonoBehaviour
{
    [Header("MiniGame objects")]
    [SerializeField]
    [Tooltip("Object parents in order: insert, count, paris, add, memory, connect, memoryVS, countVS, connectVS")]
    private GameObject[] gameObjectParents;

    [Header("Task texts")]
    [SerializeField] private OutputDistributer taskTextDistributer;
    [SerializeField]
    [Tooltip("OuputContainer for Task texts in order: Insert-inc, Count-inc, Pairs, Add, Memory, Connect, Insert-dec, Count-dec, memoryVS, countVS, connectVS")]
    private OutputContainer[] taskTexts;

    [Header("Menus")]
    [SerializeField] private GameObject menuParent;
    [SerializeField] private GameObject gameEndMenu;
    [SerializeField] private GameObject explanationMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject stillPlayingMenu;

    [Header("Menu Elements")]
    [SerializeField] private GameObject taskDescriptionObjects;
    [SerializeField] private GameObject increasingIndicator;
    [SerializeField] private GameObject decreasingIndicator;

    [Header("Game End Menu Elements")]
    [SerializeField] private OutputDistributer motivationalMessageDistributer;
    [SerializeField] private OutputContainer motivationalMessageDummy;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    [Header("Motivational messages")]
    [SerializeField] private OutputContainer[] motivationalMessages1;
    [SerializeField] private OutputContainer[] motivationalMessages2;
    [SerializeField] private OutputContainer[] motivationalMessages3;
    [SerializeField] private OutputContainer memoryChildWinMessage;
    [SerializeField] private OutputContainer memoryParentWinMessage;
    [SerializeField] private OutputContainer memoryDrawMessage;
    [SerializeField] private OutputContainer childFaster;
    [SerializeField] private OutputContainer parentFaster;
    private OutputContainer lastMM = null;

    protected int optimalThreashold;
    public int OptimalThreashold
    {
        set
        {
            optimalThreashold = System.Math.Max(0, value);
        }
    }

    protected int suboptimalThreshold;
    public int SuboptimalThreshold
    {
        set
        {
            suboptimalThreshold = System.Math.Max(optimalThreashold + 1, value);
        }
    }
    [SerializeField] private GameObject exerciseObjects;

    [Header("Explanation Menu Elements")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip InsertGIF;
    [SerializeField] private VideoClip CountGIF;
    [SerializeField] private VideoClip PairsGIF;
    [SerializeField] private VideoClip AddGIF;
    [SerializeField] private VideoClip MemoryGIF;
    [SerializeField] private VideoClip ConnectGIF;
    [SerializeField] private VideoClip AdditionGIF;
    [SerializeField] private VideoClip SubtractionGIF;
    [SerializeField] private GameObject additionButton;
    [SerializeField] private GameObject subtractionButton;

    private StorySceneCanvasController storyCanvasController;
    private HelpSystem helpSystem;

    private int currentIdx;
    private bool[] firstGameOfType;

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
    private MiniGameOptions currentExerciseOptions;
    private bool exerciseing;

    private void Awake()
    {
        storyCanvasController = FindObjectOfType<StorySceneCanvasController>();
        helpSystem = FindObjectOfType<HelpSystem>();

        firstGameOfType = new bool[9];
        for (int i = 0; i < firstGameOfType.Length; i++)
        {
            firstGameOfType[i] = true;
        }
    }

    public void StartMiniGame(MiniGameOptions gameOptions, MiniGameOptions exerciseGame = null)
    {
        currentOptions = gameOptions;
        currentExerciseOptions = exerciseGame;
        exerciseing = false;
        // Menu Setup
        menuParent.SetActive(false);
        taskDescriptionObjects.SetActive(true);
        exerciseObjects.SetActive(exerciseGame != null);
        additionButton.SetActive(false);
        subtractionButton.SetActive(false);
        increasingIndicator.SetActive(false);
        decreasingIndicator.SetActive(false);
        // MiniGame Setup: start, canvas setup, text setup 
        switch (currentOptions.gameType)
        {
            case MiniGameType.INSERT:
                if (insertMiniGame == null)
                {
                    insertMiniGame = FindObjectOfType<InsertMiniGame>();
                }
                insertMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 0;
                taskTextDistributer.output = (currentOptions.increasing ? taskTexts[0] : taskTexts[6]);
                if (currentOptions.increasing)
                {
                    increasingIndicator.SetActive(true);
                    increasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicator.SetActive(true);
                    decreasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.COUNT:
                if (countMiniGame == null)
                {
                    countMiniGame = FindObjectOfType<CountMiniGame>();
                }
                countMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 1;
                taskTextDistributer.output = (currentOptions.increasing ? taskTexts[1] : taskTexts[7]);
                if (currentOptions.increasing)
                {
                    increasingIndicator.SetActive(true);
                    increasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicator.SetActive(true);
                    decreasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.PAIRS:
                if (pairsMiniGame == null)
                {
                    pairsMiniGame = FindObjectOfType<PairsMiniGame>();
                }
                pairsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 2;
                taskTextDistributer.output = taskTexts[2];
                break;
            case MiniGameType.ADD:
                if (addMiniGame == null)
                {
                    addMiniGame = FindObjectOfType<AddMiniGame>();
                }
                addMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 3;
                taskTextDistributer.output = taskTexts[3];
                break;
            case MiniGameType.MEMORY:
                if (memoryMiniGame == null)
                {
                    MemoryMiniGame[] memoryGameScripts = FindObjectsOfType<MemoryMiniGame>();
                    if (typeof(MemoryVsMiniGame).IsInstanceOfType(memoryGameScripts[0]))
                    {
                        memoryMiniGame = memoryGameScripts[1];
                        memoryVsMiniGame = (MemoryVsMiniGame)memoryGameScripts[0];
                    }
                    else
                    {
                        memoryMiniGame = memoryGameScripts[0];
                        memoryVsMiniGame = (MemoryVsMiniGame)memoryGameScripts[1];
                    }
                }
                memoryMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 4;
                taskTextDistributer.output = taskTexts[4];
                break;
            case MiniGameType.CONNECT:
                if (connectMiniGame == null)
                {
                    connectMiniGame = FindObjectOfType<ConnectMiniGame>();
                }
                connectMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 5;
                taskTextDistributer.output = taskTexts[5];
                if (currentOptions.subtract)
                {
                    subtractionButton.SetActive(true);
                }
                else
                {
                    additionButton.SetActive(true);
                }
                break;
            case MiniGameType.MEMORY_VS:
                if (memoryVsMiniGame == null)
                {
                    memoryVsMiniGame = FindObjectOfType<MemoryVsMiniGame>();
                }
                memoryVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 6;
                taskTextDistributer.output = taskTexts[8];
                break;
            case MiniGameType.COUNT_VS:
                if (countVsMiniGame == null)
                {
                    countVsMiniGame = FindObjectOfType<CountVsMiniGame>();
                }
                countVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 7;
                //taskTextDistributer.output = (currentOptions.increasing ? taskTexts[1] : taskTexts[7]);
                taskTextDistributer.output = taskTexts[9];
                if (currentOptions.increasing)
                {
                    increasingIndicator.SetActive(true);
                    increasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                else
                {
                    decreasingIndicator.SetActive(true);
                    decreasingIndicator.GetComponentInChildren<OutputDistributer>().PlayAudio();
                }
                break;
            case MiniGameType.CONNECT_VS:
                if (connectVsMiniGame == null)
                {
                    connectVsMiniGame = FindObjectOfType<ConnectVsMiniGame>();
                }
                connectVsMiniGame.StartNewMiniGame(currentOptions);
                currentIdx = 8;
                taskTextDistributer.output = taskTexts[10];
                if (currentOptions.subtract)
                {
                    subtractionButton.SetActive(true);
                }
                else
                {
                    additionButton.SetActive(true);
                }
                break;
            default:
                Debug.LogError("MiniGame " + currentOptions.gameType + " is not supported by StoryGamesCanvasController");
                return;
        }
        ActivateParent(currentIdx);
        taskTextDistributer.Distribute();
        if (firstGameOfType[currentIdx])
        {
            taskTextDistributer.PlayAudio();
        }
        else if ((currentOptions.gameType == MiniGameType.MEMORY_VS || currentOptions.gameType == MiniGameType.CONNECT_VS || currentOptions.gameType == MiniGameType.COUNT_VS) && PlayerPrefsController.ShouldVsAudioPlay(currentOptions.gameType))
        {
            SoundControllerSingleton.CancelAudio();
            taskTextDistributer.PlayAudio();
            PlayerPrefsController.VsAudioPlayed(currentOptions.gameType);
        }
    }

    private void ActivateParent(int idx)
    {
        for (int i = 0; i < gameObjectParents.Length; i++)
        {
            if (i == idx)
            {
                gameObjectParents[i].SetActive(true);
            }
            else
            {
                gameObjectParents[i].SetActive(false);
            }
        }
    }

    private bool gameFinallyCompleted;
    private Coroutine finalCompletionCoroutine;

    private IEnumerator WaitForFinalCompletion()
    {
        while (!gameFinallyCompleted)
        {
            //Debug.Log("Waiting.");
            yield return null;
        }
        storyCanvasController.MiniGameCompleted();
        finalCompletionCoroutine = null;
    }

    public virtual void EndMiniGame()
    {
        //Debug.Log("Calling StoryGamesCanvasController.EndMiniGame()");
        // Setup Game End Menu
        OpenGameEndMenu();
        if (!exerciseing && finalCompletionCoroutine == null)
        {
            gameFinallyCompleted = false;
            finalCompletionCoroutine = StartCoroutine(WaitForFinalCompletion());
        }

        switch (currentOptions.gameType)
        {
            case MiniGameType.INSERT:
                DeactivateMiniGameTaskText(0);
                break;
            case MiniGameType.COUNT:
                DeactivateMiniGameTaskText(1);
                break;
            case MiniGameType.PAIRS:
                DeactivateMiniGameTaskText(2);
                break;
            case MiniGameType.ADD:
                DeactivateMiniGameTaskText(3);
                break;
            case MiniGameType.MEMORY:
                DeactivateMiniGameTaskText(4);
                break;
            case MiniGameType.CONNECT:
                DeactivateMiniGameTaskText(5);
                break;
            case MiniGameType.MEMORY_VS:
                DeactivateMiniGameTaskText(6);
                break;
            case MiniGameType.COUNT_VS:
                DeactivateMiniGameTaskText(7);
                break;
            case MiniGameType.CONNECT_VS:
                DeactivateMiniGameTaskText(8);
                break;
        }
    }

    public virtual void LeaveMiniGame()
    {
        // menu setup
        storyCanvasController.LeaveMiniGame();
        // cancel help system
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.MiniGameCompleted());
        if (finalCompletionCoroutine != null)
        {
            //Debug.Log("setting boolean");
            gameFinallyCompleted = true;
        }
    }

    public virtual void RestartMiniGame()
    {
        // cancel help system
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.MiniGameCompleted());
        // restart mini game
        if (currentOptions == null)
        {
            Debug.LogWarning("Trying to restart a MiniGame, but no MiniGame is/was active.");
            return;
        }
        StartMiniGame(currentOptions, currentExerciseOptions);
    }

    public void DeactivateMiniGameTaskText(int idx)
    {
        firstGameOfType[idx] = false;
    }
    #endregion MiniGames

    #region Menus
    // Game End Menu
    private System.Random rand = new System.Random();
    public virtual void OpenGameEndMenu()
    {
        OpenMenu();
        gameEndMenu.SetActive(true);
        taskDescriptionObjects.SetActive(false);

        // adjust+display motivational message
        OutputContainer motivationalMessage = motivationalMessageDummy;
        if (currentOptions.gameType == MiniGameType.MEMORY_VS)
        {
            switch (memoryVsMiniGame.GetWinner())
            {
                case 0:
                    // child won
                    motivationalMessage = memoryChildWinMessage;
                    break;
                case 1:
                    // draw
                    motivationalMessage = memoryDrawMessage;
                    break;
                case 2:
                    // parent won
                    motivationalMessage = memoryParentWinMessage;
                    break;
                default:
                    Debug.LogWarning("MemoryVsMiniGame.GetWinner not in Range [0,2]!");
                    break;
            }
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        else if (currentOptions.gameType == MiniGameType.CONNECT_VS)
        {
            if (connectVsMiniGame.ChildFaster)
            {
                // child faster
                motivationalMessage = childFaster;
            }
            else
            {
                // parent faster
                motivationalMessage = parentFaster;
            }
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        else if (currentOptions.gameType == MiniGameType.COUNT_VS)
        {
            if (countVsMiniGame.ChildFaster)
            {
                // child faster
                motivationalMessage = childFaster;
            }
            else
            {
                // parent faster
                motivationalMessage = parentFaster;
            }
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);

        }
        else if (helpSystem.WrongInputs <= optimalThreashold)
        {
            // 3 stars
            do
            {
                motivationalMessage = motivationalMessages3[rand.Next(0, motivationalMessages3.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
            //Debug.Log("Executing Event RegisterPerformance 3 stars");
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Good), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        else if (helpSystem.WrongInputs <= suboptimalThreshold)
        {
            // 2 stars
            do
            {
                motivationalMessage = motivationalMessages2[rand.Next(0, motivationalMessages2.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(false);
            //Debug.Log("Executing Event RegisterPerformance 2 stars");
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Normal), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        else
        {
            // 1 star
            do
            {
                motivationalMessage = motivationalMessages1[rand.Next(0, motivationalMessages1.Length)];
            } while (motivationalMessage.Equals(lastMM));
            star1.SetActive(true);
            star2.SetActive(false);
            star3.SetActive(false);
            //Debug.Log("Executing Event RegisterPerformance 1 star");
            ExecuteEvents.Execute<IHelpSystem>(helpSystem.gameObject, new HelpSystemPerformanceData(EventSystem.current, HelpSystemPerformanceData.Rating.Bad), (x, y) => x.RegisterPerformance((HelpSystemPerformanceData)y));
        }
        lastMM = motivationalMessage;
        motivationalMessageDistributer.output = motivationalMessage;
        motivationalMessageDistributer.Distribute();
        motivationalMessageDistributer.PlayAudio();
    }

    // Pause Menu
    public virtual void OpenPauseMenu()
    {
        OpenMenu();
        pauseMenu.SetActive(true);
    }

    public virtual void ClosePauseMenu()
    {
        CloseMenu();
    }

    public virtual void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    // Explanation Menu
    public virtual void OpenExplanationMenu()
    {
        OpenMenu();
        explanationMenu.SetActive(true);
        switch (currentOptions.gameType)
        {
            case MiniGameType.INSERT:
                videoPlayer.clip = InsertGIF;
                break;
            case MiniGameType.COUNT_VS:
            case MiniGameType.COUNT:
                videoPlayer.clip = CountGIF;
                break;
            case MiniGameType.PAIRS:
                videoPlayer.clip = PairsGIF;
                break;
            case MiniGameType.ADD:
                videoPlayer.clip = AddGIF;
                break;
            case MiniGameType.MEMORY_VS:
            case MiniGameType.MEMORY:
                videoPlayer.clip = MemoryGIF;
                break;
            case MiniGameType.CONNECT_VS:
            case MiniGameType.CONNECT:
                showAdditionalExplanation = false;
                videoPlayer.clip = ConnectGIF;
                break;
            default:
                Debug.LogWarning("No explanation video for " + currentOptions.gameType);
                return;
        }
        videoPlayer.Play();
    }

    public virtual void CloseExplanationMenu()
    {
        videoPlayer.Stop();
        CloseMenu();
    }

    public virtual void ToggleVideoPlayerPlaying()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
    }

    public virtual void ToggleExplanationMenu()
    {
        SoundControllerSingleton.CancelAudio();
        if (explanationMenu.activeInHierarchy)
        {
            CloseExplanationMenu();
        }
        else
        {
            OpenExplanationMenu();
        }
    }

    private bool showAdditionalExplanation = false;

    public virtual void ShowAdditionExplanation()
    {
        videoPlayer.Stop();
        if (showAdditionalExplanation)
        {
            videoPlayer.clip = ConnectGIF;

        }
        else
        {
            videoPlayer.clip = AdditionGIF;
        }
        videoPlayer.Play();
        showAdditionalExplanation = !showAdditionalExplanation;
    }

    public virtual void ShowSubtractionExplanation()
    {
        videoPlayer.Stop();
        if (showAdditionalExplanation)
        {
            videoPlayer.clip = ConnectGIF;

        }
        else
        {
            videoPlayer.clip = SubtractionGIF;
        }
        videoPlayer.Play();
        showAdditionalExplanation = !showAdditionalExplanation;
    }

    // Still Playing Menu
    public virtual void OpenStillPlayingMenu()
    {
        if (stillPlayingMenu.activeInHierarchy)
        {
            return;
        }
        OpenMenu();
        stillPlayingMenu.SetActive(true);
    }

    public virtual void CloseStillPlayingMenu()
    {
        CloseMenu();
    }

    // general
    private void OpenMenu()
    {
        gameObjectParents[currentIdx].SetActive(false);

        helpSystem.TmpHideHand();

        menuParent.SetActive(true);
        gameEndMenu.SetActive(false);
        explanationMenu.SetActive(false);
        pauseMenu.SetActive(false);
        stillPlayingMenu.SetActive(false);
    }

    private void CloseMenu()
    {
        gameObjectParents[currentIdx].SetActive(true);
        helpSystem.DisplayHandAgain();
        menuParent.SetActive(false);
    }

    // other menu-methods
    public void ExerciseGame()
    {
        StartMiniGame(currentExerciseOptions);
        exerciseing = true;
    }

    public void AbordGame()
    {
        LeaveMiniGame();
        SceneController sceneController = FindObjectOfType<SceneController>();
        sceneController.LoadMainMenu();
    }
    #endregion Menus
}
