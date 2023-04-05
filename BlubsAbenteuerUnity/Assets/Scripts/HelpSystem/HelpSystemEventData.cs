using UnityEngine;
using UnityEngine.EventSystems;

// EventData for help system related events
public class HelpSystemEventData : BaseEventData
{
    private MiniGameType currentMiniGameType;
    public MiniGameType CurrentMiniGameType
    {
        get
        {
            return currentMiniGameType;
        }

        set
        {
            currentMiniGameType = value;
        }
    }

    private int timeToHelp;
    public int TimeToHelp
    {
        get
        {
            return timeToHelp;
        }

        set
        {
            timeToHelp = value;
        }
    }

    private int wrongInteractionsToHelp;
    public int WrongInteractionsToHelp
    {
        get
        {
            return wrongInteractionsToHelp;
        }

        set
        {
            wrongInteractionsToHelp = value;
        }
    }

    private float timerDelay;
    public float TimerDelay
    {
        get
        {
            return timerDelay;
        }
    }

    public HelpSystemEventData(EventSystem eventSystem, MiniGameType miniGame) : base(eventSystem)
    {
        currentMiniGameType = miniGame;
        // Default params
        timeToHelp = 60;
        wrongInteractionsToHelp = 3;
        timerDelay = 0;
        ApplyDifficultySettings();
    }

    public HelpSystemEventData(EventSystem eventSystem, MiniGameType miniGame, int wrongInteractionsToHelp, float timerDelay, int timeToHelp = 60) : base(eventSystem)
    {
        currentMiniGameType = miniGame;
        this.timeToHelp = timeToHelp;
        this.wrongInteractionsToHelp = wrongInteractionsToHelp;
        this.timerDelay = timerDelay;
        ApplyDifficultySettings();
    }

    private void ApplyDifficultySettings()
    {
        switch (PlayerPrefsController.GetHelpDifficulty())
        {
            case 0:     // hard -> less help
                if (currentMiniGameType != MiniGameType.MEMORY)
                {
                    wrongInteractionsToHelp = (int)Mathf.Ceil(wrongInteractionsToHelp * 1.33f);
                }
                PlayerPrefsController.SafePlayNumberAudio(false);
                break;
            case 1:     // default -> normal help
                break;
            case 2:     // easy -> more help
                wrongInteractionsToHelp = (int)Mathf.Ceil(wrongInteractionsToHelp * 0.66f);
                PlayerPrefsController.SafePlayNumberAudio(true);
                break;
            default:
                Debug.LogError(PlayerPrefsController.GetHelpDifficulty() + " is not a valid difficulty setting!");
                break;
        }
    }
}
