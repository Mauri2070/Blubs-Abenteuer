using UnityEngine;
using UnityEngine.EventSystems;

// common base class for all mini games
public class MiniGame : MonoBehaviour
{
    private StoryGamesCanvasController canvasController;
    protected MiniGameOptions gameOptions;
    [SerializeField] protected NumberRepresentationProvider representationProvider;

    public void Awake()
    {
        if (canvasController == null)
        {
            canvasController = FindObjectOfType<StoryGamesCanvasController>();
        }
        if (representationProvider == null)
        {
            representationProvider = FindObjectOfType<NumberRepresentationProvider>();
        }
    }

    public virtual void StartNewMiniGame(MiniGameOptions options)
    {
        gameOptions = options;
        representationProvider.ChooseRepresentationSet(gameOptions.numberRepresentation);
        representationProvider.ChooseRepresentationSet2(gameOptions.secondRepresentation);

        PlayerPrefsController.SafePlayNumberAudio(options.numberAudioActive);
    }

    public virtual void EndMiniGame()
    {
        canvasController.EndMiniGame();
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.MiniGameCompleted());
    }

    public virtual void ReceiveNumberObjectButtonData(NumberObjectButton caller)
    {
        Debug.LogWarning("ReceiveNumberObjectButtonData has no function in MiniGame.cs");
    }

    public virtual bool ShouldAccept(int targetValue, int eventValue)
    {
        Debug.LogWarning("ShouldAccept always returns false in MiniGame.cs");
        return false;
    }

    public virtual Vector2 GetHelpPosition1()
    {
        Debug.LogWarning("MiniGame.GetHelpPosition1() only returns (0,0)! Use MiniGame-specific implementations for correct Vectors.");
        return Vector2.zero;
    }

    public virtual Vector2 GetHelpPosition2()
    {
        Debug.LogWarning("MiniGame.GetHelpPosition2() only returns (0,0)! Use MiniGame-specific implementations for correct Vectors.");
        return Vector2.zero;
    }

    protected void SetOptimalThreshold(int value)
    {
        canvasController.OptimalThreashold = value;
    }

    protected void SetSuboptimalThreshold(int value)
    {
        canvasController.SuboptimalThreshold = value;
    }
}
