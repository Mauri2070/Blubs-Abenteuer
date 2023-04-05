using UnityEngine;

// 2-player implementation of Memory mini game
public class MemoryVsMiniGame : MemoryMiniGame
{
    [Header("VS components")]
    [SerializeField] private NumberObject childPointsObject;
    [SerializeField] private NumberObject parentPointsObject;
    [SerializeField] private GameObject childsTurnIndicator;
    [SerializeField] private GameObject parentsTurnIndicator;

    private int childPoints;
    private int parentPoints;
    public override void StartNewMiniGame(MiniGameOptions options)
    {
        Debug.Log("Starting new Memory VS game");
        base.StartNewMiniGame(options);
        childPoints = 0;
        parentPoints = 0;
        childsTurn = true;
        childPointsObject.gameObject.SetActive(false);
        // childPointsObject.Mode = options.displayMode;
        childPointsObject.gameObject.SetActive(true);
        childPointsObject.SetFeedbackColor(NumberObject.completedColor);
        childPointsObject.Value = childPoints;
        childPointsObject.VisualSprite = representationProvider.GetNumber(childPoints, gameOptions.alternativeRepresentation);

        parentPointsObject.gameObject.SetActive(false);
        // parentPointsObject.Mode = options.displayMode;
        parentPointsObject.gameObject.SetActive(true);
        parentPointsObject.SetFeedbackColor(parentsPairColor);
        parentPointsObject.Value = parentPoints;
        parentPointsObject.VisualSprite = representationProvider.GetNumber(parentPoints, gameOptions.alternativeRepresentation);

        childsTurnIndicator.SetActive(true);
        parentsTurnIndicator.SetActive(false);
        // FB: set thresholds necessary? (-> help-System necessary in vs mode?)
    }

    protected override void PairInteraction(bool pairFound)
    {
        if (pairFound)
        {
            if (childsTurn)
            {
                childPoints++;
                childPointsObject.Value = childPoints;
                childPointsObject.VisualSprite = representationProvider.GetNumber(childPoints, gameOptions.alternativeRepresentation);
            }
            else
            {
                parentPoints++;
                parentPointsObject.Value = parentPoints;
                parentPointsObject.VisualSprite = representationProvider.GetNumber(parentPoints, gameOptions.alternativeRepresentation);
            }
        }
        else
        {
            if (childsTurn)
            {
                childsTurnIndicator.SetActive(false);
                parentsTurnIndicator.SetActive(true);
                childsTurn = false;
            }
            else
            {
                childsTurnIndicator.SetActive(true);
                parentsTurnIndicator.SetActive(false);
                childsTurn = true;
            }
        }
    }

    public int GetWinner()
    {
        // 0 -> child, 1 -> draw, 2 -> parent
        return childPoints > parentPoints ? 0 : childPoints == parentPoints ? 1 : 2;
    }
}
