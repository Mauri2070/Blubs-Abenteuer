using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 2-player implementation of the connect mini game
public class ConnectVsMiniGame : MiniGame
{
    [SerializeField] private Canvas canvas;

    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject connectTaskObjectPrefab;
    [SerializeField] private GameObject connectSolutionObjectPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject childLeftSideParent;
    [SerializeField] private GameObject childRightSideParent;
    [SerializeField] private GameObject parentLeftSideParent;
    [SerializeField] private GameObject parentRightSideParent;

    private int unconnectedPairsChild;
    private int unconnectedPairsParent;

    private List<int> childValues;
    private Dictionary<int, GameObject> rightSideObjects;
    private Dictionary<GameObject, GameObject> matchRightLeftObjects;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearField();
        base.StartNewMiniGame(options);
        rightSideObjects = new Dictionary<int, GameObject>();
        matchRightLeftObjects = new Dictionary<GameObject, GameObject>();
        GenerateNumberObjects();
        SetOptimalThreshold(unconnectedPairsChild);
        // FB: adjust thresholds
        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(unconnectedPairsChild / 3.0f));
        // FB: Task delay, wrong Interactions help
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, unconnectedPairsChild, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    private System.Random rand = new System.Random();
    private void GenerateNumberObjects()
    {
        // generate Child pairs
        List<int> values = new List<int>();
        if (gameOptions.useExplicitValues)
        {
            values.AddRange(gameOptions.explicitValues);
        }
        else
        {
            while (values.Count < gameOptions.numberOfValues)
            {
                int newVal = rand.Next(2 * gameOptions.minValue, gameOptions.maxValue + 1);
                if (gameOptions.subtract)
                {
                    newVal = rand.Next(gameOptions.minValue, Mathf.Max(gameOptions.maxValue / 2, gameOptions.maxValue - gameOptions.minValue) + 1);
                }

                if (!values.Contains(newVal))
                {
                    values.Add(newVal);
                }
            }
        }
        values = values.OrderBy(item => rand.Next()).ToList<int>();
        unconnectedPairsChild = values.Count();

        // child right side
        connectSolutionObjectPrefab.GetComponent<NumberObjectLineDnD>().Mode = gameOptions.rightSideDisplayMode;
        foreach (int i in values)
        {
            GameObject dndObject = Instantiate(connectSolutionObjectPrefab, childRightSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject.GetComponent<NumberObjectLineDnD>().VisualSprite = representationProvider.GetNumberSecondSet(i, gameOptions.alternativeRepresentation);
            dndObject.GetComponent<NumberObjectTarget>().TargetKey = i;
            dndObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT_VS;

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
            GameObject mdnd = Instantiate(connectTaskObjectPrefab, childLeftSideParent.transform);
            mdnd.GetComponent<NumberObject>().Value = i;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Canvas = canvas;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = x;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = y;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().VisualSprite = representationProvider.GetNumber(x, gameOptions.alternativeRepresentation);
            mdnd.GetComponent<NumberObjectMultiLineDnD>().visualSprite2 = representationProvider.GetNumber(y, gameOptions.alternativeRepresentation);
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Subtract = gameOptions.subtract;
            mdnd.GetComponent<NumberObjectTarget>().TargetKey = i;
            mdnd.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT_VS;

            rightSideObjects.TryGetValue(i, out GameObject right);
            matchRightLeftObjects.Add(right, mdnd);
        }

        // generate parent pairs
        childValues = values;
        values = new List<int>();
        while (values.Count < gameOptions.numberOfValues)
        {
            int newVal = rand.Next(2, 100);
            if (gameOptions.subtract)
            {
                newVal = rand.Next(1, 99);
            }

            if (!values.Contains(newVal) && !childValues.Contains(newVal))
            {
                values.Add(newVal);
            }
        }
        values = values.OrderBy(item => rand.Next()).ToList<int>();
        unconnectedPairsParent = values.Count();

        // parent right side
        connectSolutionObjectPrefab.GetComponent<NumberObjectLineDnD>().Mode = DisplayMode.TEXT;
        foreach (int i in values)
        {
            GameObject dndObject = Instantiate(connectSolutionObjectPrefab, parentRightSideParent.transform);
            dndObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            dndObject.GetComponent<NumberObjectLineDnD>().Value = i;
            dndObject.GetComponent<NumberObjectTarget>().TargetKey = i;
            dndObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT_VS;
            dndObject.GetComponent<NumberObject>().childButton = false;
        }

        // shuffle and generate for left side
        values = values.OrderBy(item => rand.Next()).ToList<int>();
        connectTaskObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.TEXT;
        foreach (int i in values)
        {
            int x, y;
            if (gameOptions.subtract)
            {
                x = rand.Next(i + 1, 100);
                y = x - i;
            }
            else
            {
                x = rand.Next(1, i / 2 + 1);
                y = i - x;
            }
            GameObject mdnd = Instantiate(connectTaskObjectPrefab, parentLeftSideParent.transform);
            mdnd.GetComponent<NumberObject>().Value = i;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Canvas = canvas;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = x;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = y;
            mdnd.GetComponent<NumberObjectMultiLineDnD>().Subtract = gameOptions.subtract;
            mdnd.GetComponent<NumberObjectTarget>().TargetKey = i;
            mdnd.GetComponent<NumberObjectTarget>().GameType = MiniGameType.CONNECT_VS;
            mdnd.GetComponent<NumberObject>().childButton = false;
        }

        SetLayoutGroupOptions();
    }

    private bool childFaster;
    public bool ChildFaster
    {
        get { return childFaster; }
    }

    public void ConnectPair(bool child)
    {
        if (child)
        {
            unconnectedPairsChild--;
            if (unconnectedPairsChild > 0)
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
            }
            else if (unconnectedPairsParent > 0)
            {
                childFaster = true;
            }
        }
        else
        {
            unconnectedPairsParent--;
            if (unconnectedPairsParent == 0 && unconnectedPairsChild > 0)
            {
                childFaster = false;
            }
        }

        if (unconnectedPairsChild == 0 && unconnectedPairsParent == 0)
        {
            StartCoroutine(DelayEnd());
        }
    }

    public override void EndMiniGame()
    {
        ClearField();
        base.EndMiniGame();
    }

    private IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(1);
        EndMiniGame();
    }

    public override bool ShouldAccept(int targetValue, int eventValue)
    {
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.NeutralInteraction());
        if (targetValue == eventValue)
        {
            if (childValues.Contains(targetValue))
            {
                rightSideObjects.TryGetValue(targetValue, out GameObject right);
                matchRightLeftObjects.Remove(right);
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
            }
            return true;
        }
        else
        {
            if (childValues.Contains(targetValue))
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
            }
            return false;
        }
    }

    public void SetLayoutGroupOptions()
    {
        switch (childLeftSideParent.transform.childCount)
        {
            case 1:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 350;
                return;
            case 2:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 175;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 175;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 200;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 250;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 200;
                return;
            case 3:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 180;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 60;
                return;
            case 4:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 100;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 30;
                return;
            case 5:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 55;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                return;
            case 6:
            default:
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                childLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                childRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;

                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                parentLeftSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().spacing = 19;
                parentRightSideParent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
                return;
        }
    }

    public void ClearField()
    {
        while (childLeftSideParent.transform.childCount > 0)
        {
            DestroyImmediate(childLeftSideParent.transform.GetChild(0).gameObject);
        }
        while (childRightSideParent.transform.childCount > 0)
        {
            DestroyImmediate(childRightSideParent.transform.GetChild(0).gameObject);
        }
        while (parentLeftSideParent.transform.childCount > 0)
        {
            DestroyImmediate(parentLeftSideParent.transform.GetChild(0).gameObject);
        }
        while (parentRightSideParent.transform.childCount > 0)
        {
            DestroyImmediate(parentRightSideParent.transform.GetChild(0).gameObject);
        }
    }

    private readonly Vector2 parentOffsetLeft = new Vector2(-611, 403);
    private readonly Vector2 parentOffsetRight = new Vector2(-106, 403);
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

    public void GenerateObjects(bool mixed)
    {
        ClearField();

        for (int i = 0; i < debugPairs; i++)
        {
            if (mixed)
            {
                connectSolutionObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.MIXED;
                connectTaskObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.MIXED;
            }
            else
            {
                connectSolutionObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.TEXT;
                connectTaskObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.TEXT;
            }
            GameObject dnd = Instantiate(connectTaskObjectPrefab, childLeftSideParent.transform);
            dnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = 1;
            dnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = 2;
            dnd.GetComponent<NumberObjectMultiLineDnD>().Start();
            dnd = Instantiate(connectSolutionObjectPrefab, childRightSideParent.transform);
            dnd.GetComponent<NumberObjectLineDnD>().Value = 3;
            dnd.GetComponent<NumberObject>().Start();
            connectSolutionObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.TEXT;
            connectTaskObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.TEXT;
            dnd = Instantiate(connectTaskObjectPrefab, parentLeftSideParent.transform);
            dnd.GetComponent<NumberObjectMultiLineDnD>().LeftValue = 1;
            dnd.GetComponent<NumberObjectMultiLineDnD>().RightValue = 2;
            dnd.GetComponent<NumberObjectMultiLineDnD>().Start();
            dnd = Instantiate(connectSolutionObjectPrefab, parentRightSideParent.transform);
            dnd.GetComponent<NumberObjectLineDnD>().Value = 3;
            dnd.GetComponent<NumberObject>().Start();
        }
        SetLayoutGroupOptions();
    }
#endif
    #endregion Editor
}
