using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 2-player implementation of the count mini game
public class CountVsMiniGame : MiniGame
{
    [Header("Number Object Prefab")]
    [SerializeField] private GameObject numberObjectButtonPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject buttonParent;

    private List<NumberObjectButton> correctChildOrder;
    private int nextChildValueIdx;
    private List<NumberObjectButton> correctParentOrder;
    private int nextParentValueIdx;

    private GameObject lastPressedChild;
    private GameObject lastPressedParent;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearGrid();
        base.StartNewMiniGame(options);
        numberObjectButtonPrefab.GetComponent<NumberObjectButton>().Mode = gameOptions.displayMode;
        correctChildOrder = new List<NumberObjectButton>();
        correctParentOrder = new List<NumberObjectButton>();
        GenerateNumberObjects();
        correctChildOrder.Sort((b1, b2) => b1.Value.CompareTo(b2.Value));
        correctParentOrder.Sort((b1, b2) => b1.Value.CompareTo(b2.Value));
        if (!gameOptions.increasing)
        {
            correctChildOrder.Reverse();
            correctParentOrder.Reverse();
        }
        nextChildValueIdx = 0;
        nextParentValueIdx = 0;
        lastPressedChild = null;
        lastPressedParent = null;
        /* removed: already mark first object
        lastPressedChild.GetComponent<NumberObject>().SetSpecialColor();
        lastPressedChild.GetComponent<NumberObject>().IsInteractable = false;
        lastPressedParent.GetComponent<NumberObject>().SetSpecialColor();
        lastPressedParent.GetComponent<NumberObject>().IsInteractable = false;
        */
        // FB: adjust threshold
        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(correctChildOrder.Count / 3.0f));
        // FB: Task delay, wrongInteractionToHelp adjustment
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, correctChildOrder.Count - 1, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
        //DebugValueLists();
    }

    private void DebugValueLists()
    {
        string ret = "Child Values: ";
        foreach (NumberObjectButton i in correctChildOrder)
        {
            ret += i.Value + " ";
        }
        ret += "\nParent Values: ";
        foreach (NumberObjectButton i in correctParentOrder)
        {
            ret += i.Value + " ";
        }
        Debug.Log(ret);
    }

    private System.Random random = new System.Random();
    private void GenerateNumberObjects()
    {
        // generate child values
        bool[,] safeSpace = new bool[5, gameOptions.displayMode == DisplayMode.MIXED ? 3 : 6];  // half space compared to single player mode
        int x, y;
        if (gameOptions.useExplicitValues)
        {
            //int firstToPress = gameOptions.increasing ? gameOptions.explicitValues.Min() : gameOptions.explicitValues.Max();  // removed: mark first object
            for (int i = 0; i < gameOptions.explicitValues.Length; i++)
            {
                do
                {
                    y = random.Next(0, 5);
                    x = random.Next(0, safeSpace.Length / 5);
                } while (safeSpace[y, x]);
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
                button.GetComponent<NumberObjectButton>().Value = i;
                button.GetComponent<NumberObjectButton>().VisualSprite = representationProvider.GetNumber(i, gameOptions.alternativeRepresentation);
                button.GetComponent<NumberObjectButton>().MiniGame = this;
                button.GetComponent<NumberObjectButton>().childButton = true;
                correctChildOrder.Add(button.GetComponent<NumberObjectButton>());
                safeSpace[y, x] = true;
                /* removed: first marked
                if(gameOptions.explicitValues[i] == firstToPress)
                {
                    lastPressedChild = button;
                }
                */
            }
        }
        else
        {
            List<int> valuesToUse = new List<int>();
            while (valuesToUse.Count < gameOptions.numberOfValues)
            {
                x = random.Next(gameOptions.minValue, gameOptions.maxValue + 1);
                if (!valuesToUse.Contains(x))
                {
                    valuesToUse.Add(x);
                }
            }
            //int firstToPress = gameOptions.increasing ? valuesToUse.Min() : valuesToUse.Max(); // removed: mark first
            foreach (int i in valuesToUse)
            {
                do
                {
                    y = random.Next(0, 5);
                    x = random.Next(0, safeSpace.Length / 5);
                } while (safeSpace[y, x]);
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
                button.GetComponent<NumberObjectButton>().Value = i;
                button.GetComponent<NumberObjectButton>().VisualSprite = representationProvider.GetNumber(i, gameOptions.alternativeRepresentation);
                button.GetComponent<NumberObjectButton>().MiniGame = this;
                button.GetComponent<NumberObjectButton>().childButton = true;
                correctChildOrder.Add(button.GetComponent<NumberObjectButton>());
                safeSpace[y, x] = true;
                /* removed: first marked
                if(i == firstToPress)
                {
                    lastPressedChild = button;
                }
                */
            }
        }

        // generateParentValues
        numberObjectButtonPrefab.GetComponent<NumberObjectButton>().Mode = DisplayMode.TEXT;

        int parentValueCount = Mathf.Min(30, gameOptions.useExplicitValues ? gameOptions.explicitValues.Length * 2 : gameOptions.numberOfValues * 2);
        safeSpace = new bool[5, 6];
        List<int> parentValues = new List<int>();
        while (parentValues.Count < parentValueCount)
        {
            x = random.Next(1, 100);
            if (!parentValues.Contains(x))
            {
                parentValues.Add(x);
            }
        }
        //int pressFirst = gameOptions.increasing ? parentValues.Min() : parentValues.Max();    // removed: first marked
        foreach (int i in parentValues)
        {
            do
            {
                y = random.Next(0, 5);
                x = random.Next(0, safeSpace.Length / 5);
            } while (safeSpace[y, x]);
            GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
            button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, false, false);
            button.GetComponent<NumberObjectButton>().Value = i;
            button.GetComponent<NumberObjectButton>().MiniGame = this;
            button.GetComponent<NumberObjectButton>().childButton = false;
            button.GetComponent<NumberObjectButton>().DisableAudio = true;
            correctParentOrder.Add(button.GetComponent<NumberObjectButton>());
            safeSpace[y, x] = true;
            /* removed: first marked
            if (i == pressFirst)
            {
                lastPressedParent = button;
            }
            */
        }
    }

    private static Vector2 parentOffset = new Vector2(900, 0);
    private Vector2 CalculateAnchoredPosition(int x, int y, bool mixed, bool child = true)
    {
        return (new Vector2(x * (mixed ? 280 : 143) - (mixed ? -40 : 39), y * -180)) + (child ? Vector2.zero : parentOffset);
    }

    private bool childFaster;
    public bool ChildFaster
    {
        get { return childFaster; }
    }

    public override void ReceiveNumberObjectButtonData(NumberObjectButton caller)
    {
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.NeutralInteraction());
        if (caller.childButton)
        {
            if (caller.Value == correctChildOrder[nextChildValueIdx].Value)
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
                caller.OnRightInteraction();
                nextChildValueIdx++;

                if (lastPressedChild != null)
                {
                    lastPressedChild.GetComponent<NumberObject>().RightInteraction();
                }
                caller.SetSpecialColor();
                lastPressedChild = caller.gameObject;

                if (nextChildValueIdx < correctChildOrder.Count)
                {
                    ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
                }
                else if (nextParentValueIdx < correctParentOrder.Count)
                {
                    childFaster = true;
                }
            }
            else
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
                caller.OnWrongInteraction();
            }
        }
        else
        {
            if (caller.Value == correctParentOrder[nextParentValueIdx].Value)
            {
                caller.OnRightInteraction();
                nextParentValueIdx++;

                if (lastPressedParent != null)
                {
                    lastPressedParent.GetComponent<NumberObject>().RightInteraction();
                }
                caller.SetSpecialColor();
                lastPressedParent = caller.gameObject;

                if (nextParentValueIdx == correctParentOrder.Count && nextChildValueIdx < correctChildOrder.Count)
                {
                    childFaster = false;
                }
            }
            else
            {
                caller.OnWrongInteraction();
            }
        }

        if (nextChildValueIdx == correctChildOrder.Count && nextParentValueIdx == correctParentOrder.Count)
        {
            StartCoroutine(DelayEnd());
        }
    }

    public override void EndMiniGame()
    {
        ClearGrid();
        base.EndMiniGame();
    }

    private IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(1);
        EndMiniGame();
    }

    public void ClearGrid()
    {
        while (buttonParent.transform.childCount > 0)
        {
            DestroyImmediate(buttonParent.transform.GetChild(0).gameObject);
        }
    }

    private readonly Vector2 numberParentOffset = new Vector2(-776, 331);

    public override Vector2 GetHelpPosition1()
    {
        return correctChildOrder[nextChildValueIdx].GetComponent<RectTransform>().anchoredPosition + numberParentOffset;
    }

    public override Vector2 GetHelpPosition2()
    {
        return GetHelpPosition1();
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    public void FillGrid(bool mixed)
    {
        ClearGrid();
        numberObjectButtonPrefab.GetComponent<NumberObjectButton>().Mode = mixed ? DisplayMode.MIXED : DisplayMode.TEXT;
        bool[,] safeSpace = new bool[5, mixed ? 3 : 6];

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < safeSpace.Length / 5; x++)
            {
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, mixed);
                button.GetComponent<NumberObjectButton>().Value = 1;
                button.GetComponent<NumberObject>().Start();
            }
        }

        numberObjectButtonPrefab.GetComponent<NumberObjectButton>().Mode = DisplayMode.TEXT;
        safeSpace = new bool[5, 6];

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < safeSpace.Length / 5; x++)
            {
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, false, false);
                button.GetComponent<NumberObjectButton>().Value = 999;
                button.GetComponent<NumberObjectButton>().Start();
            }
        }
    }
#endif
    #endregion Editor
}
