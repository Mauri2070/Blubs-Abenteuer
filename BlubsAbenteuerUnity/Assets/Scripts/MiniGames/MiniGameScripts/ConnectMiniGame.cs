using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// implementatino of connect mini game
public class ConnectMiniGame : MiniGame
{
    [SerializeField] private Canvas canvas;

    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject connectTaskObjectPrefab;
    [SerializeField] private GameObject connectSolutionObjectPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject leftSideParent;
    [SerializeField] private GameObject rightSideParent;

    private int unconnectedPairs;

    private Dictionary<int, GameObject> rightSideObjects;
    private Dictionary<GameObject, GameObject> matchRightLeftObjects;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearField();
        base.StartNewMiniGame(options);
        rightSideObjects = new Dictionary<int, GameObject>();
        matchRightLeftObjects = new Dictionary<GameObject, GameObject>();
        GenerateNumberObjects();
        // FB: adjust thresholds
        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(unconnectedPairs / 3.0f));
        // FB: Task delay, ttH
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, unconnectedPairs, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    private System.Random rand = new System.Random();

    private void GenerateNumberObjects()
    {
        // no check of explicit values -> value range!
        // initialize and shuffle value list
        List<int> values = new List<int>();
        if (gameOptions.useExplicitValues)
        {
            values.AddRange(gameOptions.explicitValues);
        }
        else
        {
            while (values.Count() < gameOptions.numberOfValues)
            {
                int newVal = rand.Next(2 * gameOptions.minValue, gameOptions.maxValue + 1);
                if (gameOptions.subtract)
                {
                    newVal = rand.Next(gameOptions.minValue, Mathf.Max(gameOptions.maxValue / 2, gameOptions.maxValue - gameOptions.minValue) + 1);
                }
                else
                {
                    newVal = rand.Next(2 * gameOptions.minValue, gameOptions.maxValue + 1);
                }
                if (!values.Contains(newVal))
                {
                    values.Add(newVal);
                }
            }
        }
        values = values.OrderBy(item => rand.Next()).ToList<int>();
        unconnectedPairs = values.Count();

        // generate objects for right side
        connectSolutionObjectPrefab.GetComponent<NumberObjectLineDnD>().Mode = gameOptions.rightSideDisplayMode;
        foreach (int i in values)
        {
            GameObject dndObject = Instantiate(connectSolutionObjectPrefab, rightSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject.GetComponent<NumberObjectLineDnD>().VisualSprite = representationProvider.GetNumberSecondSet(i, gameOptions.alternativeRepresentation);
            dndObject.GetComponent<NumberObjectTarget>().TargetKey = i;
            dndObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT;

            rightSideObjects.Add(i, dndObject);
        }

        // shuffle and generate for left side
        values = values.OrderBy(item => rand.Next()).ToList<int>();
        connectTaskObjectPrefab.GetComponent<NumberObject>().Mode = gameOptions.displayMode;
        foreach (int i in values)
        {
            int x, y;
            if (gameOptions.subtract)
            {
                x = rand.Next(i + gameOptions.minValue, gameOptions.maxValue + 1);
                y = x - i;
            }
            else
            {
                x = rand.Next(gameOptions.minValue, i / 2 + 1);
                y = i - x;
            }
            GameObject mdnd = Instantiate(connectTaskObjectPrefab, leftSideParent.transform);
            mdnd.GetComponent<NumberObject>().Value = i;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Canvas = canvas;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = x;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = y;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().VisualSprite = representationProvider.GetNumber(x, gameOptions.alternativeRepresentation);
            mdnd.GetComponent<NumberObjectMultiLineDnD>().visualSprite2 = representationProvider.GetNumber(y, gameOptions.alternativeRepresentation);
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Subtract = gameOptions.subtract;
            mdnd.GetComponent<NumberObjectTarget>().TargetKey = i;
            mdnd.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT;

            rightSideObjects.TryGetValue(i, out GameObject right);
            matchRightLeftObjects.Add(right, mdnd);
        }

        SetLayoutGroupOptions();
    }

    public void ConnectPair()
    {
        unconnectedPairs--;
        if (unconnectedPairs == 0)
        {
            Debug.Log("MiniGame erfolgreich abgeschlossen");
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
            rightSideObjects.TryGetValue(targetValue, out GameObject right);
            matchRightLeftObjects.Remove(right);
            //Debug.Log(matchRightLeftObjects.Count);
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
        right = matchRightLeftObjects.Keys.First();
        matchRightLeftObjects.TryGetValue(right, out GameObject left);
        return left.GetComponent<RectTransform>().anchoredPosition + parentOffsetLeft;
    }

    public override Vector2 GetHelpPosition2()
    {
        return right.GetComponent<RectTransform>().anchoredPosition + parentOffsetRight;
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    [Header("Debugging")]
    [SerializeField] [Range(1, 10)] private int debugPairs;

    public void GenerateObjects()
    {
        ClearField();

        for (int i = 0; i < debugPairs; i++)
        {
            GameObject dnd = Instantiate(connectTaskObjectPrefab, leftSideParent.transform);
            dnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = 1;
            dnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = 2;
            dnd = Instantiate(connectSolutionObjectPrefab, rightSideParent.transform);
            dnd.GetComponent<NumberObjectLineDnD>().Value = 3;
        }

        SetLayoutGroupOptions();
    }
#endif
    #endregion
}
