using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// implementation of add mini game
public class AddMiniGame : MiniGame
{
    [SerializeField] private Canvas canvas;

    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject numberObjectPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject numberObjectParent;
    [SerializeField] private GameObject targetValueParent;

    private int possiblePairs;

    private Dictionary<int, List<GameObject>> buttons;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearGrid();
        base.StartNewMiniGame(options);
        numberObjectPrefab.GetComponent<NumberObject>().Mode = gameOptions.displayMode;
        buttons = new Dictionary<int, List<GameObject>>();
        GenerateNumberObjects();
        // FB: adjust thresholds
        base.SetOptimalThreshold(1);
        base.SetSuboptimalThreshold(Mathf.RoundToInt(possiblePairs / 3.0f));
        // FB: Task delay, ttH
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, possiblePairs, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    private System.Random random = new System.Random();

    private void GenerateNumberObjects()
    {
        // Generate the gameObject to display the searched Value
        GameObject numberObject = Instantiate(numberObjectPrefab, targetValueParent.transform);
        numberObject.GetComponent<NumberObject>().Value = gameOptions.targetValue;
        numberObject.GetComponent<NumberObject>().IsInteractable = false;
        numberObject.GetComponent<NumberObject>().VisualSprite = representationProvider.GetNumber(gameOptions.targetValue, gameOptions.alternativeRepresentation);
        numberObject.GetComponent<NumberObject>().SetSpecialColor();

        // generate safe-space Array
        bool[,] safeSpace = new bool[4, gameOptions.displayMode == DisplayMode.MIXED ? 7 : 12];
        int x, y;

        // generate lower half of possible add-partners
        List<int> gameValues = new List<int>();
        for (int i = 1; i <= gameOptions.targetValue / 2; i++)
        {
            gameValues.Add(i);
        }

        // prune random values till reaching the wanted amount of pairs
        while (gameValues.Count > gameOptions.numberOfValues)
        {
            gameValues.RemoveAt(random.Next(0, gameValues.Count));
        }

        possiblePairs = gameValues.Count;

        // add add-partners to all values
        List<int> allValues = new List<int>(gameValues);
        foreach (int v in gameValues)
        {
            allValues.Add(gameOptions.targetValue - v);
        }

        // generate bait values
        List<int> baitValues = new List<int>();
        HashSet<int> forbiddenValues = new HashSet<int>();
        while (baitValues.Count < gameOptions.numberOfNonSolutionValues)
        {
            x = random.Next(gameOptions.minValue, gameOptions.maxValue);
            if (!forbiddenValues.Contains(x) && !baitValues.Contains(x))
            {
                baitValues.Add(x);
                forbiddenValues.Add(gameOptions.targetValue - x);
            }
        }
        allValues.AddRange(baitValues);

        // generate the numberObjects
        foreach (int v in allValues)
        {
            do
            {
                y = random.Next(0, 4);
                x = random.Next(0, safeSpace.Length / 4);
            } while (safeSpace[y, x]);
            numberObject = Instantiate(numberObjectPrefab, numberObjectParent.transform);
            numberObject.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
            numberObject.GetComponent<NumberObjectLineDnD>().Value = v;
            numberObject.GetComponent<NumberObjectLineDnD>().Canvas = canvas;
            numberObject.GetComponent<NumberObjectLineDnD>().VisualSprite = representationProvider.GetNumber(v, gameOptions.alternativeRepresentation);
            numberObject.GetComponent<NumberObjectTarget>().TargetKey = v;
            numberObject.GetComponent<NumberObjectTarget>().GameType = MiniGameType.ADD;
            safeSpace[y, x] = true;

            if (!buttons.TryGetValue(v, out List<GameObject> objs))
            {
                objs = new List<GameObject>();
                buttons.Add(v, objs);
            }
            objs.Add(numberObject);
        }
    }

    private Vector2 CalculateAnchoredPosition(int x, int y, bool mixed)
    {
        return new Vector2(x * (mixed ? 250 : 147) - (mixed ? -20 : 39), y * -190);
    }

    public override void EndMiniGame()
    {
        ClearGrid();
        base.EndMiniGame();
    }

    public void ClearGrid()
    {
        while (numberObjectParent.transform.childCount > 0)
        {
            DestroyImmediate(numberObjectParent.transform.GetChild(0).gameObject);
        }
        while (targetValueParent.transform.childCount > 0)
        {
            DestroyImmediate(targetValueParent.transform.GetChild(0).gameObject);
        }
    }

    public override bool ShouldAccept(int targetValue, int eventValue)
    {
        if (targetValue + eventValue == gameOptions.targetValue)
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
            return true;
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
            return false;
        }
    }

    public void PairMatched()
    {
        possiblePairs--;
        if (possiblePairs <= 0)
        {
            Debug.Log("MiniGame erfolgreich abgeschlossen");
            StartCoroutine(DelayEnd());
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
        }
    }

    private IEnumerator DelayEnd()
    {
        // end for current method calls on children to finish
        yield return new WaitForSeconds(1);
        EndMiniGame();
    }

    private readonly Vector2 parentOffset = new Vector2(-775, 186);
    private GameObject other;

    public override Vector2 GetHelpPosition1()
    {
        for (int i = 0; i < buttons.Keys.Count; i++)
        {
            int firstValue = buttons.Keys.ElementAt(i);
            int secondValue = gameOptions.targetValue - firstValue;
            buttons.TryGetValue(firstValue, out List<GameObject> firstList);
            buttons.TryGetValue(secondValue, out List<GameObject> secondList);
            if (firstList == null || secondList == null || (firstValue == secondValue && firstList.Count == 1))
            {
                continue;
            }

            if (firstValue == secondValue)
            {
                other = firstList[1];
                return firstList[0].GetComponent<RectTransform>().anchoredPosition + parentOffset;
            }
            else
            {
                other = secondList[0];
                return firstList[0].GetComponent<RectTransform>().anchoredPosition + parentOffset;
            }
        }
        Debug.LogError("HelpSystem: Could not find matching pair for Add MiniGame!");
        return Vector2.zero;
    }

    public override Vector2 GetHelpPosition2()
    {
        if (other == null)
        {
            Debug.LogError("HelpSystem: 2nd position in Add MiniGame was null!");
            return Vector2.zero;
        }
        return other.GetComponent<RectTransform>().anchoredPosition + parentOffset;
    }

    public void RemoveElementsFromHelp(int v1, GameObject g1, int v2, GameObject g2)
    {
        buttons.TryGetValue(v1, out List<GameObject> objs);
        objs.Remove(g1);
        if (objs.Count == 0)
        {
            buttons.Remove(v1);
        }
        buttons.TryGetValue(v2, out objs);
        objs.Remove(g2);
        if (objs.Count == 0)
        {
            buttons.Remove(v2);
        }
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    public void FillGrid(bool mixed)
    {
        Debug.Log("Filling grid for Add MiniGame");
        ClearGrid();

        numberObjectPrefab.GetComponent<NumberObject>().Mode = mixed ? DisplayMode.MIXED : DisplayMode.TEXT;

        GameObject numberObject = Instantiate(numberObjectPrefab, targetValueParent.transform);

        bool[,] safeSpace = new bool[4, mixed ? 7 : 12];

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < safeSpace.Length / 4; x++)
            {
                numberObject = Instantiate(numberObjectPrefab, numberObjectParent.transform);
                numberObject.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, mixed);
                numberObject.GetComponent<NumberObjectLineDnD>().Value = 1;
                numberObject.GetComponent<NumberObject>().Start();
            }
        }
    }
#endif
    #endregion Editor
}
