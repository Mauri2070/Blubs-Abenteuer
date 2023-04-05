using UnityEngine;

// UI controller script for not game related UI in story scene
public class StorySceneCanvasController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject storyDisplayCanvas;
    [SerializeField] private GameObject ufoRoomCanvas;
    [SerializeField] private GameObject miniGameCanvas;

    [Header("UFO Room Canvas Elements")]
    [SerializeField] private SpriteRenderer roomBackground;

    [Header("Room Changeing")]
    [SerializeField] private GameObject hubNavButton;
    [SerializeField] private GameObject navHubButton;
    [SerializeField] private GameObject hubLabButton;
    [SerializeField] private GameObject labHubButton;
    [SerializeField] private GameObject hubEngineButton;
    [SerializeField] private GameObject engineHubButton;

    [Header("MiniGame Selection/debugging")]
    [SerializeField] private GameObject cutOnMiniGame;
    [SerializeField] private GameObject gameStartButtonsParent;
    public GameObject GameStartButtonsParent
    {
        get
        {
            return gameStartButtonsParent;
        }
    }

    [Header("Menus")]
    [SerializeField] private GameObject toMainMenuMenu;
    [SerializeField] private GameObject vsGameAskMenu;
    [SerializeField] private GameObject resetStoryModeRequestButton;
    private bool storyResetStatus;

    private ProgressController progressController;
    private StorySequenceController storySequenceController;

    private void Awake()
    {
        // Canvas setup
        storyDisplayCanvas.SetActive(false);
        ufoRoomCanvas.SetActive(true);
        miniGameCanvas.SetActive(false);
        // Menu setup
        cutOnMiniGame.SetActive(true);
        toMainMenuMenu.SetActive(false);
        vsGameAskMenu.SetActive(false);
        // Button setup
        hubNavButton.SetActive(true);
        hubLabButton.SetActive(true);
        hubEngineButton.SetActive(true);
        navHubButton.SetActive(false);
        labHubButton.SetActive(false);
        engineHubButton.SetActive(false);

        // Object fetshing
        progressController = FindObjectOfType<ProgressController>();
        storySequenceController = FindObjectOfType<StorySequenceController>();
    }

    #region Story
    public void StartStorySequence(StorySequence storySequence)
    {
        Debug.Log("Starting StorySequence " + storySequence + " (CanvasController)");
        // Menu Setup
        storyDisplayCanvas.SetActive(true);
        ufoRoomCanvas.SetActive(false);
        miniGameCanvas.SetActive(false);

        // Start Story
        storySequenceController.StartNewStorySequence(storySequence);
    }

    public void EndStorySequence()
    {
        // Menu Setup
        ufoRoomCanvas.SetActive(true);
        storyDisplayCanvas.SetActive(false);
        progressController.EndStorySequence();
    }
    #endregion Story

    #region RoomChanging
    public void ChangeRoom(string targetRoom)
    {
        // Debug.Log("SSCC: Changing room to " + targetRoom);
        // delete start buttons for current room
        while (gameStartButtonsParent.transform.childCount > 0)
        {
            DestroyImmediate(gameStartButtonsParent.transform.GetChild(0).gameObject);
        }
        // change room: setup buttons, load room from progressController
        if (targetRoom.Equals("hub"))
        {
            hubNavButton.SetActive(true);
            hubEngineButton.SetActive(true);
            hubLabButton.SetActive(true);
            navHubButton.SetActive(false);
            engineHubButton.SetActive(false);
            labHubButton.SetActive(false);
            progressController.LoadRoom(Room.HUB);
        }
        else if (targetRoom.Equals("nav"))
        {
            hubNavButton.SetActive(false);
            hubEngineButton.SetActive(false);
            hubLabButton.SetActive(false);
            navHubButton.SetActive(true);
            engineHubButton.SetActive(false);
            labHubButton.SetActive(false);
            progressController.LoadRoom(Room.NAV);
        }
        else if (targetRoom.Equals("engine"))
        {
            hubNavButton.SetActive(false);
            hubEngineButton.SetActive(false);
            hubLabButton.SetActive(false);
            navHubButton.SetActive(false);
            engineHubButton.SetActive(true);
            labHubButton.SetActive(false);
            progressController.LoadRoom(Room.ENGINE);
        }
        else if (targetRoom.Equals("lab"))
        {
            hubNavButton.SetActive(false);
            hubEngineButton.SetActive(false);
            hubLabButton.SetActive(false);
            navHubButton.SetActive(false);
            engineHubButton.SetActive(false);
            labHubButton.SetActive(true);
            progressController.LoadRoom(Room.LAB);
        }
        else
        {
            Debug.LogError(targetRoom + " is not a valid room string!");
        }
    }

    public void ChangeBackground(Sprite newBackground)
    {
        roomBackground.sprite = newBackground;
    }
    #endregion RoomChanging

    #region MiniGames
    private StoryGamesCanvasController gamesCanvasController;
    private GameObject startingParent;
    public void StartMiniGame(MiniGameOptions gameOptions, GameObject startingButton = null, MiniGameOptions exerciseOptions = null)
    {
        SoundControllerSingleton.CancelAudio();
        startingParent = startingButton;

        switch (gameOptions.gameType)
        {
            case MiniGameType.MEMORY_VS:
            case MiniGameType.COUNT_VS:
            case MiniGameType.CONNECT_VS:
                vsGame = gameOptions;
                skipGame = exerciseOptions;
                OpenVSAskMenu();
                return;
            default:
                break;
        }
        // Menu Setup
        miniGameCanvas.SetActive(true);
        cutOnMiniGame.SetActive(false);
        // Start MiniGame -> delegate to StoryGamesCanvasController
        if (gamesCanvasController == null)
        {
            gamesCanvasController = FindObjectOfType<StoryGamesCanvasController>();
        }
        gamesCanvasController.StartMiniGame(gameOptions, exerciseOptions);
    }

    // Debug
    public void StartMiniGameTest(MiniGameOptions gameOptions)
    {
        StartMiniGame(gameOptions);
    }

    private MiniGameOptions vsGame;
    private MiniGameOptions skipGame;

    public void StartVSGame()
    {
        CloseAskVSMenu();
        // Menu Setup
        miniGameCanvas.SetActive(true);
        cutOnMiniGame.SetActive(false);
        // Start MiniGame -> delegate to StoryGamesCanvasController
        if (gamesCanvasController == null)
        {
            gamesCanvasController = FindObjectOfType<StoryGamesCanvasController>();
        }
        gamesCanvasController.StartMiniGame(vsGame, skipGame);
        SoundControllerSingleton.CancelAudio();
    }

    public void SkipVSGame()
    {
        CloseAskVSMenu();
        // Menu Setup
        miniGameCanvas.SetActive(true);
        cutOnMiniGame.SetActive(false);
        // Start MiniGame -> delegate to StoryGamesCanvasController
        if (gamesCanvasController == null)
        {
            gamesCanvasController = FindObjectOfType<StoryGamesCanvasController>();
        }
        gamesCanvasController.StartMiniGame(skipGame);
        SoundControllerSingleton.CancelAudio();
    }

    public void LeaveMiniGame()
    {
        // Menu Setup
        miniGameCanvas.SetActive(false);
        cutOnMiniGame.SetActive(true);
    }

    public void MiniGameCompleted()
    {
        if (startingParent != null)
        {
            startingParent.SetActive(false);
        }
        progressController.GameCompleted();
    }
    #endregion MiniGames

    #region Menu
    private bool cutOutStatus;
    public void OpenMenu()
    {
        // Menu Setup
        toMainMenuMenu.SetActive(true);
        storyResetStatus = resetStoryModeRequestButton.activeSelf;
        resetStoryModeRequestButton.SetActive(false);
        if (cutOutStatus = cutOnMiniGame.activeSelf)
        {
            cutOnMiniGame.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        // Menu Setup
        toMainMenuMenu.SetActive(false);
        resetStoryModeRequestButton.SetActive(storyResetStatus);
        if (cutOutStatus)
        {
            cutOnMiniGame.SetActive(true);
        }
    }

    public void ToggleMenu()
    {
        if (toMainMenuMenu.activeSelf)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenVSAskMenu()
    {
        cutOnMiniGame.SetActive(false);
        vsGameAskMenu.SetActive(true);
    }

    private void CloseAskVSMenu()
    {
        vsGameAskMenu.SetActive(false);
    }
    #endregion Menu
}