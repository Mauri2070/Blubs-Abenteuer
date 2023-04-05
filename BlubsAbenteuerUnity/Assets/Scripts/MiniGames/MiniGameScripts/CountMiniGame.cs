using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// implementation of count mini game
public class CountMiniGame : MiniGame
{
    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject numberObjectButtonPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject buttonParent;

    private List<NumberObjectButton> correctOrder;
    private int nextValueIndex;

    private GameObject lastPressed;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearGrid();
        base.StartNewMiniGame(options);
        numberObjectButtonPrefab.GetComponent<NumberObjectButton>().Mode = gameOptions.displayMode;
        correctOrder = new List<NumberObjectButton>();
        GenerateNumberObjects();
        correctOrder.Sort((b1, b2) => b1.Value.CompareTo(b2.Value));
        if (!gameOptions.increasing)
        {
            correctOrder.Reverse();
        }
        // nextValueIndex = 1; // removed: First value is already marked -> start with second value
        nextValueIndex = 0;
        lastPressed = null;
        /* removed: first marked
        lastPressed.GetComponent<NumberObject>().SetSpecialColor();
        lastPressed.GetComponent<NumberObject>().IsInteractable = false;
        */
        // FB: adjust threshold
        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(correctOrder.Count / 3.0f));
        // FB: Task delay, wrongInteractionToHelp adjustment
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, correctOrder.Count - 1, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    private System.Random random = new System.Random();

    private void GenerateNumberObjects()
    {
        bool[,] safeSpace = new bool[5, gameOptions.displayMode == DisplayMode.MIXED ? 7 : 12];
        int x, y;
        if (gameOptions.useExplicitValues)
        {
            //int firstToPress = gameOptions.increasing ? gameOptions.explicitValues.Min() : gameOptions.explicitValues.Max(); // removed: first marked
            for (int i = 0; i < gameOptions.explicitValues.Length; i++)
            {
                do
                {
                    y = random.Next(0, 5);
                    x = random.Next(0, safeSpace.Length / 5);
                } while (safeSpace[y, x]);
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
                button.GetComponent<NumberObjectButton>().Value = gameOptions.explicitValues[i];
                button.GetComponent<NumberObjectButton>().VisualSprite = representationProvider.GetNumber(gameOptions.explicitValues[i], gameOptions.alternativeRepresentation);
                button.GetComponent<NumberObjectButton>().MiniGame = this;
                correctOrder.Add(button.GetComponent<NumberObjectButton>());
                safeSpace[y, x] = true;
                /* removed: first marked
                if (gameOptions.explicitValues[i] == firstToPress)
                {
                    lastPressed = button.gameObject;
                    //Debug.Log("Setting Button " + firstToPress + " to button with value " + button.gameObject.GetComponent<NumberObject>().Value);
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
            //int firstToPress = gameOptions.increasing ? valuesToUse.Min() : valuesToUse.Max(); // removed: first marked
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
                correctOrder.Add(button.GetComponent<NumberObjectButton>());
                safeSpace[y, x] = true;
                /* removed: first marked
                if (i == firstToPress)
                {
                    lastPressed = button.gameObject;
                    //Debug.Log("Setting Button " + firstToPress+" to button with value "+button.gameObject.GetComponent<NumberObject>().Value);
                }
                */
            }
        }
    }

    private Vector2 CalculateAnchoredPosition(int x, int y, bool mixed)
    {
        return new Vector2(x * (mixed ? 250 : 147) - (mixed ? -20 : 39), y * -180);
    }

    public override void ReceiveNumberObjectButtonData(NumberObjectButton caller)
    {
        if (caller.Value == correctOrder[nextValueIndex].Value)
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
            caller.OnRightInteraction();
            nextValueIndex++;
            // Set colors for Buttons
            if (lastPressed != null)
            {
                lastPressed.GetComponent<NumberObject>().RightInteraction();
            }
            caller.SetSpecialColor();
            lastPressed = caller.gameObject;

            if (nextValueIndex == correctOrder.Count)
            {
                StartCoroutine(DelayEnd());
            }
            else
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
            }
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
            caller.OnWrongInteraction();
        }
    }

    public override void EndMiniGame()
    {
        ClearGrid();
        base.EndMiniGame();
    }

    private IEnumerator DelayEnd()
    {
        // end for current method calls on children to finish
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

    private readonly Vector2 parentOffset = new Vector2(-776, 331);

    public override Vector2 GetHelpPosition1()
    {
        return correctOrder[nextValueIndex].GetComponent<RectTransform>().anchoredPosition + parentOffset;
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

        bool[,] safeSpace = new bool[5, mixed ? 7 : 12];

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < safeSpace.Length / 5; x++)
            {
                GameObject button = Instantiate(numberObjectButtonPrefab, buttonParent.transform);
                button.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, mixed);
                //button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * (gameOptions.displayMode == DisplayMode.MIXED ? 270 : 125), y * -160);
                button.GetComponent<NumberObjectButton>().Value = 1;
                button.GetComponent<NumberObject>().Start();
            }
        }
    }
#endif
    #endregion Editor
}
