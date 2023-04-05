using UnityEngine;

// progress step to store story sequences
[CreateAssetMenu(fileName = "new Story Progress Step", menuName = "Progress/StoryProgressStep")]
public class StoryProgressStep : ProgressStep
{
    [Header("Story")]
    [SerializeField] private StorySequence storySequence;
    public StorySequence StorySequence
    {
        get
        {
            return storySequence;
        }
    }

    [Header("MiniGames")]
    [SerializeField] private bool hasFinalMiniGame;
    [SerializeField] private MiniGameOptions finalMiniGame;

    public bool HasFinalGame(out MiniGameOptions finalGame)
    {
        finalGame = finalMiniGame;
        return hasFinalMiniGame;
    }
}
