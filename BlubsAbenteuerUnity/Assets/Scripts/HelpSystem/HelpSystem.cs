using UnityEngine;

// Implementation of the help system
public class HelpSystem : MonoBehaviour, IHelpSystem
{
    [Header("Help System Parameters")]
    [SerializeField] [Tooltip("Should the serialized values be used (for debugging)?")] private bool overrideGameSpecificParameters = false;
    [SerializeField]
    [Tooltip("Time since last interaction unitl help will be displayed in seconds. Negative values deactivate this feature.")]
    private int timeToHelp;
    [SerializeField]
    [Tooltip("Amount of wrong interactions since the last right one until help will be displayed. Negative values deactivate this feature.")]
    private int wrongInteractionsToHelp;

    [Header("Hand parameters")]
    [SerializeField] [Tooltip("Offset to the right of the object mid.")] private float dX;
    [SerializeField] [Tooltip("Offset down from the object mid.")] private float dY;
    [SerializeField] [Tooltip("Range for static hand scaling (used for positive and negative scaling).")] private float scaleRange;
    [SerializeField] [Tooltip("Scaling factor per second (0.01% steps)")] private float scaleSpeed;
    [SerializeField] [Tooltip("Speed of moving hand (per second).")] private float moveSpeed;
    [SerializeField] [Tooltip("Time the hand stays at one position before and after moveing.")] private float stayTime;

    private float timeSinceInteraction;
    private int wrongInteractionStreak;

    [Header("Help System Prefabs")]
    [SerializeField] private GameObject handPrefab;

    // MiniGames
    private MiniGameType currentGameType;
    private InsertMiniGame insertMiniGame;
    private CountMiniGame countMiniGame;
    private AddMiniGame addMiniGame;
    private MemoryMiniGame memoryMiniGame;
    private PairsMiniGame pairsMiniGame;
    private ConnectMiniGame connectMiniGame;
    private MemoryVsMiniGame memoryVsMiniGame;

    private bool gameActive = false;
    private bool helping;
    private bool moveHand;
    private float restTime;
    private bool expand;

    private int skillDelta;

    // used for motivation system: determines #stars received
    private int wrongInputsThisGame;
    public int WrongInputs
    {
        get
        {
            return wrongInputsThisGame;
        }
    }

    [Header("Additional Help System Variables")]
    [SerializeField] private GameObject helpingHandParent;
    private GameObject helpingHand;
    private RectTransform rect;
    private Vector2 startPosition;
    private Vector3 endPosition;

    private StoryGamesCanvasController canvasController;

    public void MiniGameCompleted()
    {
        Debug.Log("MiniGameCompleted received");

        HideHelp();
        gameActive = false;
    }

    public void MiniGameStarted(HelpSystemEventData eventData)
    {
        Debug.Log("MiniGameStarted received with MiniGame " + eventData.CurrentMiniGameType);

        currentGameType = eventData.CurrentMiniGameType;

        gameActive = true;
        helping = false;
        timeSinceInteraction = 0;
        wrongInteractionStreak = 0;

        wrongInputsThisGame = 0;

        if (!overrideGameSpecificParameters)
        {
            timeToHelp = Mathf.Max(eventData.TimeToHelp, 5);
            wrongInteractionsToHelp = Mathf.Max(eventData.WrongInteractionsToHelp, 3);
            timeSinceInteraction = -eventData.TimerDelay;
        }
    }

    public void RightInteraction()
    {
        Debug.Log("RightInteraction received");

        HideHelp();
        timeSinceInteraction = 0;
        wrongInteractionStreak = 0;
    }

    public void WrongInteraction()
    {
        Debug.Log("WrongInteraction received - wrongInteractionsToHelp: " + wrongInteractionsToHelp + ", wrongInteractionStreak: " + wrongInteractionStreak);

        wrongInteractionStreak++;
        timeSinceInteraction = 0;
        if (wrongInteractionsToHelp >= 0 && wrongInteractionStreak >= wrongInteractionsToHelp)
        {
            DisplayHelp();
        }
        wrongInputsThisGame++;
    }

    public void NeutralInteraction()
    {
        Debug.Log("NeutralInteraction received");
        timeSinceInteraction = 0;
    }

    public void DecreaseHelpBorder()
    {
        wrongInteractionsToHelp--;
        if (wrongInteractionsToHelp < 3)
        {
            wrongInteractionsToHelp = 3;
        }
    }

    private void Update()
    {
        if (!gameActive)
        {
            return;
        }

        UpdateActiveTime();
        AnimateHelpingHand();
    }

    // update time and check, if child is still playing
    private void UpdateActiveTime()
    {
        timeSinceInteraction += Time.deltaTime;

        if (timeToHelp >= 0 && timeSinceInteraction >= timeToHelp)
        {
            if (canvasController == null)
            {
                canvasController = FindObjectOfType<StoryGamesCanvasController>();
            }
            canvasController.OpenStillPlayingMenu();
        }
    }

    // animate/move helping hand
    private void AnimateHelpingHand()
    {
        if (helpingHand != null)
        {
            if (moveHand)
            {
                if (restTime < stayTime || (rect.anchoredPosition.Equals(endPosition) && restTime < 2 * stayTime))
                {
                    restTime += Time.deltaTime;
                }
                else if (restTime >= 2 * stayTime)
                {
                    rect.anchoredPosition = startPosition;
                    restTime = 0;
                }
                else
                {
                    rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, endPosition, moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (expand)
                {
                    rect.localScale += scaleSpeed * Time.deltaTime * Vector3.one;
                    if (rect.localScale.x >= 1 + scaleRange)
                    {
                        expand = false;
                    }
                }
                else
                {
                    rect.localScale -= scaleSpeed * Time.deltaTime * Vector3.one;
                    if (rect.localScale.x <= 1 - scaleRange)
                    {
                        expand = true;
                    }
                }
            }
        }
    }

    private void Start()
    {
        insertMiniGame = gameObject.GetComponent<InsertMiniGame>();
        countMiniGame = gameObject.GetComponent<CountMiniGame>();
        addMiniGame = gameObject.GetComponent<AddMiniGame>();
        memoryMiniGame = gameObject.GetComponent<MemoryMiniGame>();
        pairsMiniGame = gameObject.GetComponent<PairsMiniGame>();
        connectMiniGame = gameObject.GetComponent<ConnectMiniGame>();
        memoryVsMiniGame = gameObject.GetComponent<MemoryVsMiniGame>();

        skillDelta = 0;
    }

    private void DisplayHelp()
    {
        if (helping)
        {
            return;
        }
        Debug.Log("HelpSystem: activating help");
        helping = true;
        MiniGame currentGame;
        switch (currentGameType)
        {
            case MiniGameType.INSERT:
                currentGame = insertMiniGame;
                break;
            case MiniGameType.COUNT:
                currentGame = countMiniGame;
                break;
            case MiniGameType.PAIRS:
                currentGame = pairsMiniGame;
                break;
            case MiniGameType.ADD:
                currentGame = addMiniGame;
                break;
            case MiniGameType.CONNECT:
                currentGame = connectMiniGame;
                break;
            case MiniGameType.MEMORY_VS:
                currentGame = memoryVsMiniGame;
                break;
            case MiniGameType.MEMORY:
            default:    // to avoid comiler error after switch
                currentGame = memoryMiniGame;
                break;
        }

        // important: first call GetHelpPosition1 before calling GetHelpPosition2. Never use only one of them.
        startPosition = currentGame.GetHelpPosition1() + dX * Vector2.right + dY * Vector2.down;
        endPosition = currentGame.GetHelpPosition2() + dX * Vector2.right + dY * Vector2.down;

        helpingHand = Instantiate(handPrefab, helpingHandParent.transform);
        rect = helpingHand.GetComponent<RectTransform>();
        rect.anchoredPosition = startPosition;

        if (startPosition.Equals(endPosition))
        {
            expand = true;
            moveHand = false;
        }
        else
        {
            restTime = 0;
            moveHand = true;
        }
    }

    // deactivate help
    private void HideHelp()
    {
        if (!helping)
        {
            return;
        }
        Debug.Log("HelpSystem: deactivating help");

        helping = false;
        DestroyImmediate(helpingHand);
    }

    // temporarily disable help
    private bool bActiveTmp;

    public void TmpHideHand()
    {
        bActiveTmp = gameActive;
        if (!bActiveTmp)
        {
            return;
        }

        gameActive = false;
        if (helping)
        {
            helpingHand.SetActive(false);
        }

        timeSinceInteraction = 0;
    }

    public void DisplayHandAgain()
    {
        gameActive = bActiveTmp;
        if (!gameActive)
        {
            return;
        }

        if (helping)
        {
            helpingHand.SetActive(true);
        }
    }

    public void RegisterPerformance(HelpSystemPerformanceData eventData)
    {
        Debug.Log("Registering Performance: " + eventData.StarRating);
        switch (eventData.StarRating)
        {
            case HelpSystemPerformanceData.Rating.Good:
                skillDelta++;
                if (skillDelta < 0)
                {
                    skillDelta++;
                }
                Debug.Log("Increasing skill delta to " + skillDelta);
                break;
            case HelpSystemPerformanceData.Rating.Normal:
                if (skillDelta > 0)
                {
                    skillDelta--;
                    Debug.Log("Normalizing skill delta to " + skillDelta);
                }
                else if (skillDelta < 0)
                {
                    skillDelta++;
                    Debug.Log("Normalizing skill delta to " + skillDelta);
                }
                break;
            case HelpSystemPerformanceData.Rating.Bad:
                skillDelta--;
                if (skillDelta > 0)
                {
                    skillDelta--;
                }
                Debug.Log("Decreasing skill delta to " + skillDelta);
                break;
        }

        if (skillDelta >= 5)
        {
            PlayerPrefsController.DecreaseHelp();
            skillDelta = 0;
        }
        else if (skillDelta <= -3)
        {
            PlayerPrefsController.IncreaseHelp();
            skillDelta = 0;
        }
    }
}
