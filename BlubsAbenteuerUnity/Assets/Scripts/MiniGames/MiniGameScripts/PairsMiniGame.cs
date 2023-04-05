using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// implementation of pairs mini game
public class PairsMiniGame : MiniGame
{
    [SerializeField] private Canvas canvas;

    [Header("Number Object Prefabs")]
    [SerializeField]
    [Tooltip("Prefab containing NumberObjectLineDnD and NumberObject component.")]
    private GameObject pairNumberObjectPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject leftSideParent;
    [SerializeField] private GameObject rightSideParent;

    private int unmatchedPairs;

    private Dictionary<int, GameObject> leftSideObjects;
    private Dictionary<GameObject, GameObject> matchLeftRightObjects;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearField();
        base.StartNewMiniGame(options);
        matchLeftRightObjects = new Dictionary<GameObject, GameObject>();
        GenerateNumberObjects();
        // FB: adjust thresholds
        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(unmatchedPairs / 3.0f));
        // FB: Task delay, ttH
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, unmatchedPairs, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    private System.Random random = new System.Random();

    private void GenerateNumberObjects()
    {
        // initialize and shuffle list for left side
        List<int> valueList;
        if (gameOptions.useExplicitValues)
        {
            valueList = new List<int>(gameOptions.explicitValues);
        }
        else
        {
            valueList = new List<int>();
            int cur;
            while (valueList.Count < gameOptions.numberOfValues)
            {
                cur = random.Next(gameOptions.minValue, gameOptions.maxValue + 1);
                if (!valueList.Contains(cur))
                {
                    valueList.Add(cur);
                }
            }
        }
        valueList = valueList.OrderBy(item => random.Next()).ToList<int>();
        unmatchedPairs = valueList.Count();

        // generate Objects for left side
        leftSideObjects = new Dictionary<int, GameObject>();
        pairNumberObjectPrefab.GetComponent<NumberObjectLineDnD>().Mode = gameOptions.displayMode;
        foreach (int i in valueList)
        {
            GameObject dndObject = Instantiate(pairNumberObjectPrefab, leftSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject.GetComponent<NumberObjectLineDnD>().VisualSprite = representationProvider.GetNumber(i, gameOptions.alternativeRepresentation);
            dndObject.GetComponent<NumberObjectTarget>().TargetKey = i;
            dndObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.PAIRS;

            leftSideObjects.Add(i, dndObject);
        }

        // shuffle and generate for right side
        valueList = valueList.OrderBy(item => random.Next()).ToList<int>();
        pairNumberObjectPrefab.GetComponent<NumberObjectLineDnD>().Mode = gameOptions.rightSideDisplayMode;
        foreach (int i in valueList)
        {
            GameObject dndObject = Instantiate(pairNumberObjectPrefab, rightSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject.GetComponent<NumberObjectLineDnD>().VisualSprite = representationProvider.GetNumberSecondSet(i, gameOptions.alternativeRepresentation);
            dndObject.GetComponent<NumberObjectTarget>().TargetKey = i;
            dndObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.PAIRS;

            leftSideObjects.TryGetValue(i, out GameObject left);
            matchLeftRightObjects.Add(left, dndObject);
        }

        SetLayoutGroupOptions();
    }

    public void PairMatched()
    {
        unmatchedPairs--;
        if (unmatchedPairs == 0)
        {
            Debug.Log("MiniGame erfolgreich abgschlossen");
            StartCoroutine(DelayEnd());
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
        }
    }

    public override void EndMiniGame()
    {
        ClearField();
        base.EndMiniGame();
    }

    public override bool ShouldAccept(int targetValue, int eventValue)
    {
        if (targetValue == eventValue)
        {
            leftSideObjects.TryGetValue(targetValue, out GameObject left);
            matchLeftRightObjects.Remove(left);
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
            return true;
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
            return false;
        }
    }

    private IEnumerator DelayEnd()
    {
        // end for current method calls on children to finish
        yield return new WaitForSeconds(1);
        EndMiniGame();
    }

    public void ClearField()
    {
        while (leftSideParent.transform.childCount > 0)
        {
            DestroyImmediate(leftSideParent.transform.GetChild(0).gameObject);
        }
        while (rightSideParent.transform.childCount > 0)
        {
            DestroyImmediate(rightSideParent.transform.GetChild(0).gameObject);
        }
    }

    private void SetLayoutGroupOptions()
    {
        switch (leftSideParent.transform.childCount)
        {
            case 1:
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;
                return;
            case 2:
                leftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 175;
                rightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 175;
                return;
            case 3:
                leftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;
                rightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;
                return;
            case 4:
                leftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;
                rightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;
                return;
            case 5:
                leftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                rightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                return;
            case 6:
            default:
                leftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                leftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                rightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                rightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                return;
        }

    }

    private readonly Vector2 parentOffsetLeft = new Vector2(-550, 403);
    private readonly Vector2 parentOffsetRight = new Vector2(550, 403);
    private GameObject right;

    public override Vector2 GetHelpPosition1()
    {
        GameObject left = matchLeftRightObjects.Keys.First();
        matchLeftRightObjects.TryGetValue(left, out right);
        return left.GetComponent<RectTransform>().anchoredPosition + parentOffsetLeft;
    }

    public override Vector2 GetHelpPosition2()
    {
        return right.GetComponent<RectTransform>().anchoredPosition + parentOffsetRight;
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    [SerializeField] [Range(1, 10)] private int debugPairs;
    // for testing
    public void GenerateObjects()
    {
        ClearField();

        for (int i = 0; i < debugPairs; i++)
        {
            GameObject dndObject = Instantiate(pairNumberObjectPrefab, leftSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject = Instantiate(pairNumberObjectPrefab, rightSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
        }

        SetLayoutGroupOptions();
    }
#endif
    #endregion Editor
}