using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// implementation of insert mini game
public class InsertMiniGame : MiniGame
{
    [SerializeField] private Canvas canvas;

    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject numberLineObjectPrefab;
    [SerializeField] private GameObject dndNumberObjectPrefab;
    [SerializeField] private GameObject dndTargetPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject numberLineParent;
    [SerializeField] private GameObject dndObjectParent; // top left
    [SerializeField] private GameObject skipReadingButton;

    private int unfilledGaps;
    Dictionary<int, NumberInterval> acceptingIntervals;
    Dictionary<int, GameObject> dndObjects;
    List<int> availableNumbers;
    List<NumberObjectTarget> targets;

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        ClearParents();
        unfilledGaps = 0;
        base.StartNewMiniGame(options);
        SetPrefabOptions();
        acceptingIntervals = new Dictionary<int, NumberInterval>();
        NumberInterval.maxVal = options.maxValue;
        NumberInterval.minVal = options.minValue;
        availableNumbers = new List<int>();
        dndObjects = new Dictionary<int, GameObject>();
        targets = new List<NumberObjectTarget>();
        GenerateNumberObjects();
        availableNumbers.Sort();
        if (!options.increasing)
        {
            availableNumbers.Reverse();
        }
        NumberInterval.allValues = availableNumbers;
        PrepareAcceptingValues();

        SetOptimalThreshold(1);
        SetSuboptimalThreshold(Mathf.RoundToInt(unfilledGaps / 3.0f));
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, unfilledGaps, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));

        skipReadingButton.SetActive(false);
        //acceptingIntervals.TryGetValue(0, out NumberInterval interval);
        //Debug.Log(interval.ToString());
    }

    private void SetPrefabOptions()
    {
        numberLineObjectPrefab.GetComponent<NumberObject>().Mode = gameOptions.displayMode;
        dndNumberObjectPrefab.GetComponent<NumberObjectDnD>().Mode = gameOptions.displayMode;
    }

    private System.Random random = new System.Random();
    private static readonly Vector3 singleDisplayScale = new Vector3(0.5f, 1f, 1f);
    private static readonly Vector3 childScale = new Vector3(1.75f, 1, 1);

    private void GenerateNumberObjects()
    {
        // generate safe-space Array
        bool[,] safeSpace = new bool[4, gameOptions.displayMode == DisplayMode.MIXED ? 7 : 12];
        int x, y;

        // generate value-list
        List<int> gameValues;
        if (gameOptions.useExplicitValues)
        {
            gameValues = new List<int>(gameOptions.explicitValues);
        }
        else
        {
            gameValues = new List<int>();
            if (gameOptions.maxValue - gameOptions.minValue + 1 <= gameOptions.numberOfValues)
            {
                for (int i = gameOptions.minValue; i <= gameOptions.maxValue; i++)
                {
                    gameValues.Add(i);
                }
            }
            else
            {
                while (gameValues.Count < gameOptions.numberOfValues)
                {
                    int valueToAdd = random.Next(gameOptions.minValue, gameOptions.maxValue + 1);
                    if (!gameValues.Contains(valueToAdd))
                    {
                        gameValues.Add(valueToAdd);
                    }
                }
            }
        }
        gameValues.Sort();
        if (!gameOptions.increasing)
        {
            gameValues.Reverse();
        }

        // generate missing-value-list
        List<int> missingValues;
        if (gameOptions.useExplicitMissingValues)
        {
            missingValues = new List<int>(gameOptions.missingValues);
        }
        else
        {
            missingValues = new List<int>();
            while (missingValues.Count < gameOptions.numberMissingValues)
            {
                int valueToAdd = gameValues[random.Next(0, gameValues.Count)];
                if (!missingValues.Contains(valueToAdd))
                {
                    missingValues.Add(valueToAdd);
                }
            }
        }

        // generate number line and fitting values
        int value;
        NumberInterval current, prev = null;
        for (int i = 0; i < gameValues.Count; i++)
        {
            value = gameValues[i];
            if (missingValues.Contains(value))
            {
                do
                {
                    y = random.Next(0, 4);
                    x = random.Next(0, safeSpace.Length / 4);
                } while (safeSpace[y, x]);
                GameObject dndObject = Instantiate(dndNumberObjectPrefab, dndObjectParent.transform);
                dndObject.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
                dndObject.GetComponent<NumberObjectDnD>().Canvas = canvas;
                dndObject.GetComponent<NumberObjectDnD>().Value = value;
                dndObject.GetComponent<NumberObjectDnD>().VisualSprite = representationProvider.GetNumber(value, gameOptions.alternativeRepresentation);
                safeSpace[y, x] = true;
                dndObjects.Add(value, dndObject);
                NumberObjectTarget target = Instantiate(dndTargetPrefab, numberLineParent.transform).GetComponent<NumberObjectTarget>();
                target.GameType = MiniGameType.INSERT;
                target.TargetKey = i;
                if (gameOptions.displayMode != DisplayMode.MIXED)
                {
                    target.gameObject.transform.localScale = singleDisplayScale;
                    target.gameObject.transform.GetChild(0).transform.localScale = childScale;
                }
                targets.Add(target);
                unfilledGaps++;
                current = new NumberInterval(prev);
                acceptingIntervals.Add(i, current);
                if (prev != null)
                {
                    prev.next = current;
                }
                prev = current;
            }
            else
            {
                GameObject newLineObject = Instantiate(numberLineObjectPrefab, numberLineParent.transform);
                newLineObject.GetComponent<NumberObject>().Value = value;
                newLineObject.GetComponent<NumberObject>().VisualSprite = representationProvider.GetNumber(value, gameOptions.alternativeRepresentation);
                newLineObject.GetComponent<NumberObject>().SetSpecialColor();
                current = new NumberInterval(prev, value);
                acceptingIntervals.Add(i, current);
                if (prev != null)
                {
                    prev.next = current;
                }
                prev = current;
            }
        }

        availableNumbers.AddRange(gameValues);
        // generate additional Objects
        int baitValue;
        List<int> baitValues = new List<int>();
        while (baitValues.Count() < gameOptions.numberOfNonSolutionValues && baitValues.Count() < (gameOptions.maxValue - gameOptions.minValue + 1) - gameValues.Count())
        {
            baitValue = random.Next(gameOptions.minValue, gameOptions.maxValue + 1);
            if (!gameValues.Contains(baitValue) && !baitValues.Contains(baitValue))
            {
                baitValues.Add(baitValue);
            }
        }

        for (int i = 0; i < baitValues.Count(); i++)
        {
            do
            {
                y = random.Next(0, 4);
                x = random.Next(0, safeSpace.Length / 4);
            } while (safeSpace[y, x]);
            GameObject dndObject = Instantiate(dndNumberObjectPrefab, dndObjectParent.transform);
            dndObject.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.displayMode == DisplayMode.MIXED);
            dndObject.GetComponent<NumberObjectDnD>().Canvas = canvas;
            baitValue = baitValues[i];
            dndObject.GetComponent<NumberObjectDnD>().Value = baitValue;
            dndObject.GetComponent<NumberObjectDnD>().VisualSprite = representationProvider.GetNumber(baitValue, gameOptions.alternativeRepresentation);
            safeSpace[y, x] = true;

            if (!availableNumbers.Contains(baitValue))
            {
                dndObjects.Add(baitValue, dndObject);
                availableNumbers.Add(baitValue);
            }
        }

        SetLayoutGroupOptions(gameOptions.displayMode == DisplayMode.MIXED);
    }

    private Vector2 CalculateAnchoredPosition(int x, int y, bool mixed)
    {
        return new Vector2(x * (mixed ? 250 : 145) - (mixed ? -12 : 35), y * -190);
    }

    private void SetLayoutGroupOptions(bool mixed)
    {
        if (mixed)
        {
            switch (numberLineParent.transform.childCount)
            {
                case 2:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 250;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 525;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 10;
                    return;
                case 3:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 200;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 325;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 10;
                    return;
                case 4:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 145;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 150;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 10;
                    return;
                case 5:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 87;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 80;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 10;
                    return;
                case 6:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 50;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 0;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 20;
                    return;
                default:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 9;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -24;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.top = 22;
                    return;
            }
        }
        else
        {
            switch (numberLineParent.transform.childCount)
            {
                case 2:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 100;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 580;
                    return;
                case 3:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 100;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 410;
                    return;
                case 4:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 95;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 220;
                    return;
                case 5:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 50;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 125;
                    return;
                case 6:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 20;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 70;
                    return;
                case 7:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = 0;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = 0;
                    return;
                case 8:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = -20;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -45;
                    return;
                case 9:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = -45;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -55;
                    return;
                case 10:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = -65;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -65;
                    return;
                case 11:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = -82;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -70;
                    return;
                default:
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().spacing = -96;
                    numberLineParent.GetComponent<HorizontalLayoutGroup>().padding.left = -75;
                    return;
            }
        }
    }

    public override bool ShouldAccept(int targetValue, int eventValue)
    {
        if (!acceptingIntervals.TryGetValue(targetValue, out NumberInterval target))
        {
            Debug.Log("Missing NumberInterval for NumberTarget");
            return false;
        }

        //Debug.Log(target.ToString());
        //Debug.Log(DebugList(availableNumbers));

        int minIdx = target.Min();
        int maxIdx = target.Max();

        //Debug.Log("[" + minIdx + "," + maxIdx + "] -> " + eventValue);

        if (gameOptions.increasing)
        {
            if (minIdx != -1 && availableNumbers[minIdx] > eventValue || (maxIdx < availableNumbers.Count && availableNumbers[maxIdx] < eventValue))
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
                return false;
            }
        }
        else
        {
            if (minIdx != -1 && availableNumbers[minIdx] < eventValue || (maxIdx < availableNumbers.Count && availableNumbers[maxIdx] > eventValue))
            {
                ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
                return false;
            }
        }

        target.val = eventValue;
        minIdx = minIdx < 0 ? 0 : minIdx;
        if (gameOptions.increasing)
        {
            while (availableNumbers[minIdx] < eventValue)
            {
                minIdx++;
            }
        }
        else
        {
            while (availableNumbers[minIdx] > eventValue)
            {
                minIdx++;
            }
        }

        target.idx = minIdx;

        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
        return true;
    }

    public void GapFilled()
    {
        unfilledGaps--;
        if (unfilledGaps == 0)
        {
            Debug.Log("MiniGame erfolgreich abgeschlossen.");
            StartCoroutine(DelayEnd());
        }
        else
        {
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.DecreaseHelpBorder());
        }
    }

    public override void EndMiniGame()
    {
        ClearParents();
        base.EndMiniGame();
    }

    private bool skipReading;

    private IEnumerator DelayEnd()
    {
        // end for current method calls on children to finish
        yield return new WaitForSeconds(0.5f);
        ClearParents();

        // read completed number line to user and animate it
        numberLineParent.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 300);
        skipReading = false;
        skipReadingButton.SetActive(true);

        acceptingIntervals.TryGetValue(0, out NumberInterval first);
        List<int> finalValues = first.GetValueList();
        List<NumberObjectDnD> obj = new List<NumberObjectDnD>();
        foreach (int value in finalValues)
        {
            GameObject newLineObject = Instantiate(numberLineObjectPrefab, numberLineParent.transform);
            newLineObject.GetComponent<NumberObject>().Value = value;
            newLineObject.GetComponent<NumberObject>().VisualSprite = representationProvider.GetNumber(value, gameOptions.alternativeRepresentation);
            newLineObject.GetComponent<NumberObject>().SetSpecialColor();
            obj.Add(newLineObject.GetComponent<NumberObjectDnD>());
        }
        SetLayoutGroupOptions(gameOptions.displayMode == DisplayMode.MIXED);

        foreach (NumberObjectDnD no in obj)
        {
            SoundControllerSingleton.PlayAudio(NumberRepresentationProvider.NumberAudio[no.Value - 1]);
            no.Animate(NumberRepresentationProvider.NumberAudio[no.Value - 1].audio.length);
            yield return new WaitForSeconds(NumberRepresentationProvider.NumberAudio[no.Value - 1].audio.length + 0.5f);
            if (skipReading)
            {
                break;
            }
        }
        yield return new WaitForSeconds(1);
        numberLineParent.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 300);
        skipReadingButton.SetActive(false);

        EndMiniGame();
    }

    public void SkipEndAnimation()
    {
        skipReading = true;
    }

    private void PrepareAcceptingValues()
    {
        int idx = 0;
        acceptingIntervals.TryGetValue(0, out NumberInterval current);
        while (current != null && idx < availableNumbers.Count)
        {
            if (current.val >= 0)
            {
                if (gameOptions.increasing)
                {
                    while (availableNumbers[idx] < current.val)
                    {
                        idx++;
                    }
                }
                else
                {
                    while (availableNumbers[idx] > current.val)
                    {
                        idx++;
                    }
                }
                current.idx = idx;
            }
            current = current.next;
        }
    }

    public void ClearParents()
    {
        while (numberLineParent.transform.childCount > 0)
        {
            DestroyImmediate(numberLineParent.transform.GetChild(0).gameObject);
        }
        while (dndObjectParent.transform.childCount > 0)
        {
            DestroyImmediate(dndObjectParent.transform.GetChild(0).gameObject);
        }
    }

    private readonly Vector2 dndOffset = new Vector2(-775, 177);
    private readonly Vector2 lineOffset = new Vector2(-857, 347);

    public override Vector2 GetHelpPosition1()
    {
        NumberInterval current = acceptingIntervals.Values.First();
        while (current.prev != null)
        {
            current = current.prev;
        }
        while (current.val >= 0)
        {
            current = current.next;
        }
        int idx = current.Min();
        GameObject obj;
        while (!dndObjects.TryGetValue(availableNumbers[idx], out obj))
        {
            idx++;
        }
        return obj.GetComponent<RectTransform>().anchoredPosition + dndOffset;
    }

    public override Vector2 GetHelpPosition2()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].AcceptElement)
            {
                return targets[i].gameObject.GetComponent<RectTransform>().anchoredPosition + lineOffset;
            }
        }
        Debug.LogError("HelpSystem: Could not find active DnDTarget for Insert MiniGame.");
        return Vector2.zero;
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    public void FillGrid(bool mixed)
    {
        ClearParents();

        // set options
        numberLineObjectPrefab.GetComponent<NumberObject>().Mode = mixed ? DisplayMode.MIXED : DisplayMode.TEXT;
        dndNumberObjectPrefab.GetComponent<NumberObjectDnD>().Mode = mixed ? DisplayMode.MIXED : DisplayMode.TEXT;

        bool[,] safeSpace = new bool[4, mixed ? 7 : 12];

        // fill number line
        for (int i = 0; i < (mixed ? 7 : 12); i++)
        {
            GameObject newLineObject = Instantiate(numberLineObjectPrefab, numberLineParent.transform);
            newLineObject.GetComponent<NumberObject>().Value = i + 1;
            newLineObject.GetComponent<NumberObject>().SetSpecialColor();
            newLineObject.GetComponent<NumberObject>().Start();
        }

        // fill grid
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < safeSpace.Length / 4; x++)
            {
                GameObject dndObject = Instantiate(dndNumberObjectPrefab, dndObjectParent.transform);
                dndObject.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, mixed); ;
                dndObject.GetComponent<NumberObjectDnD>().Canvas = canvas;
                dndObject.GetComponent<NumberObjectDnD>().Value = 1;
                dndObject.GetComponent<NumberObject>().Start();
            }
        }

        SetLayoutGroupOptions(mixed);
    }
#endif
    #endregion Editor

    // class for managing all possible versions of a number line
    private class NumberInterval
    {
        public static int minVal;
        public static int maxVal;
        public static List<int> allValues;

        public int val; // < 0 -> empty place in number line, >= 0 -> fixed value
        public int idx; // index in availableNumbers

        public NumberInterval prev;
        public NumberInterval next;

        public NumberInterval(NumberInterval prev, int value = -1)
        {
            this.prev = prev;
            next = null;
            val = value;
            idx = -2;
        }

        public int Min()
        {
            if (val >= 0)
            {
                return idx;
            }
            if (prev == null)
            {
                return 0;
            }
            return prev.Min() + 1;
        }

        public int Max()
        {
            if (val >= 0)
            {
                return idx;
            }
            if (next == null)
            {
                return allValues.Count - 1;
            }
            return next.Max() - 1;
        }

        // for debugging
        public override string ToString()
        {
            string ret = "";
            NumberInterval tmp = prev;
            while (tmp != null)
            {
                if (tmp.val >= 0)
                {
                    ret = "(" + tmp.val + "," + tmp.idx + ")" + " -> " + ret;
                }
                else
                {
                    ret = "x -> " + ret;
                }
                tmp = tmp.prev;
            }
            ret = "List: START -> " + ret;
            if (val >= 0)
            {
                ret += "(" + val + "," + idx + ")" + " -> ";
            }
            else
            {
                ret += "x -> ";
            }
            tmp = next;
            while (tmp != null)
            {
                if (tmp.val >= 0)
                {
                    ret += "(" + tmp.val + "," + tmp.idx + ")" + " -> ";
                }
                else
                {
                    ret += "x -> ";
                }
                tmp = tmp.next;
            }
            ret += "END";
            return ret;
        }

        public List<int> GetValueList()
        {
            List<int> ret = new List<int>();

            NumberInterval tmp = this;
            while (tmp.prev != null)
            {
                tmp = tmp.prev;
            }

            while (tmp != null)
            {
                ret.Add(tmp.val);
                tmp = tmp.next;
            }

            return ret;
        }
    }

    private string DebugList(List<int> list)
    {
        string ret = "List: ";
        for (int i = 0; i < list.Count; i++)
        {
            ret += "(" + list[i] + "," + i + ") -> ";
        }
        return ret;
    }
}
