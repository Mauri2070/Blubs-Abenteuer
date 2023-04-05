using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// implementation of memory mini game
public class MemoryMiniGame : MiniGame
{
    public enum MemorySize
    {
        SMALL, MEDIUM, LARGE
    }

    [Header("Number Object Prefabs")]
    [SerializeField] private GameObject memoryObjectPrefab;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject numberObjectParent;

    private NumberObjectMemoryCard card1;
    private int pairsToMatch;

    private List<NumberObjectMemoryCard> activeCards;
    private Dictionary<NumberObjectMemoryCard, NumberObjectMemoryCard> cardPairs;

    Coroutine waitingToTurnCoroutine;

    // Attributes used for VS-Mode
    protected bool childsTurn = true;
    protected Color parentsPairColor = new Color(0.6f, 0.0f, 1f);

    public override void StartNewMiniGame(MiniGameOptions options)
    {
        Debug.Log("Starting new Memory game");
        ClearGrid();
        base.StartNewMiniGame(options);
        memoryObjectPrefab.GetComponent<NumberObject>().Mode = DisplayMode.SET;
        card1 = null;
        activeCards = new List<NumberObjectMemoryCard>();
        cardPairs = new Dictionary<NumberObjectMemoryCard, NumberObjectMemoryCard>();
        GenerateNumberObjects();
        waitingToTurnCoroutine = null;
        // FB: adjust thresholds
        SetOptimalThreshold(pairsToMatch);
        SetSuboptimalThreshold(3 * pairsToMatch);
        // FB: task delay, ttH
        ExecuteEvents.Execute<IHelpSystem>(gameObject, new HelpSystemEventData(EventSystem.current, options.gameType, pairsToMatch, 5.0f), (x, y) => x.MiniGameStarted((HelpSystemEventData)y));
    }

    public override void EndMiniGame()
    {
        ClearGrid();
        base.EndMiniGame();
    }

    public void ClearGrid()
    {
        Debug.Log("Clearing memory grid");
        while (numberObjectParent.transform.childCount > 0)
        {
            DestroyImmediate(numberObjectParent.transform.GetChild(0).gameObject);
        }
    }

    private System.Random random = new System.Random();
    private void GenerateNumberObjects()
    {
        Debug.Log("Generating Memory Number Objects");
        // generate the numbers to use
        List<int> values = new List<int>();
        if (gameOptions.useExplicitValues)
        {
            values.AddRange(gameOptions.explicitValues);
        }
        else
        {
            if (gameOptions.maxValue - gameOptions.minValue + 1 == (gameOptions.memorySize == MemorySize.SMALL ? 4 : gameOptions.memorySize == MemorySize.MEDIUM ? 10 : 14))
            {
                for (int i = gameOptions.minValue; i <= gameOptions.maxValue; i++)
                {
                    values.Add(i);
                }
            }
            else
            {
                while (values.Count < (gameOptions.memorySize == MemorySize.SMALL ? 4 : gameOptions.memorySize == MemorySize.MEDIUM ? 10 : 14))
                {
                    int value = random.Next(gameOptions.minValue, gameOptions.maxValue + 1);
                    if (!values.Contains(value))
                    {
                        values.Add(value);
                    }
                }
            }
        }
        pairsToMatch = values.Count();
        Debug.Log(pairsToMatch + " values to generate");
        // instantiate the gameObjects
        List<GameObject> objects = new List<GameObject>();
        GameObject memoryObject;
        NumberObjectMemoryCard tmp;
        foreach (int value in values)
        {
            // generate first
            memoryObject = Instantiate(memoryObjectPrefab, numberObjectParent.transform);
            memoryObject.GetComponent<NumberObjectMemoryCard>().Value = value;
            memoryObject.GetComponent<NumberObjectMemoryCard>().VisualSprite = representationProvider.GetNumber(value, gameOptions.alternativeRepresentation);
            memoryObject.GetComponent<NumberObjectMemoryCard>().miniGame = this;
            objects.Add(memoryObject);
            tmp = memoryObject.GetComponent<NumberObjectMemoryCard>();
            // generate second
            memoryObject = Instantiate(memoryObjectPrefab, numberObjectParent.transform);
            memoryObject.GetComponent<NumberObjectMemoryCard>().Value = value;
            memoryObject.GetComponent<NumberObjectMemoryCard>().VisualSprite = representationProvider.GetNumberSecondSet(value, gameOptions.alternativeRepresentation);
            if (gameOptions.matchSetText)
            {
                memoryObject.GetComponent<NumberObjectMemoryCard>().Mode = DisplayMode.TEXT;
            }
            memoryObject.GetComponent<NumberObjectMemoryCard>().miniGame = this;
            objects.Add(memoryObject);
            cardPairs.Add(tmp, memoryObject.GetComponent<NumberObjectMemoryCard>());
            cardPairs.Add(memoryObject.GetComponent<NumberObjectMemoryCard>(), tmp);
        }

        // shuffle the gameObjects
        objects = objects.OrderBy<GameObject, int>(item => random.Next()).ToList<GameObject>();
        Debug.Log("Generated and shuffeled " + objects.Count + " objects");
        // position the gameObjects
        int x = 0, y = 0;
        foreach (GameObject obj in objects)
        {

            obj.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, gameOptions.memorySize);
            x++;
            if (x >= (gameOptions.memorySize == MemorySize.SMALL ? 3 : gameOptions.memorySize == MemorySize.MEDIUM ? 5 : 7))
            {
                x = 0;
                y++;
            }
            else if (gameOptions.memorySize == MemorySize.SMALL && x == 1 && y == 1)
            {
                x++;
            }
            activeCards.Add(obj.GetComponent<NumberObjectMemoryCard>());
        }
        Debug.Log("Memory game generation completed.");
    }

    public void AcceptCard(NumberObjectMemoryCard card)
    {
        ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.NeutralInteraction());
        if (waitingToTurnCoroutine != null)
        {
            return;
        }
        if (card1 == null)
        {
            // Debug.LogWarning("Evaluating new Pair.");
            card1 = card;
            card.busy = true;
            card.TurnCard();
            return;
        }
        else if (card1 == card)
        {
            return;
        }

        card.busy = true;
        card.TurnCard();
        waitingToTurnCoroutine = StartCoroutine(WaitingToTurn(card));
    }

    private IEnumerator WaitingToTurn(NumberObjectMemoryCard card)
    {
        while (card.busy || card1.busy)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (card.Value == card1.Value)
        {
            PairInteraction(true);
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.RightInteraction());
            card.IsInteractable = false;
            card1.IsInteractable = false;
            pairsToMatch--;
            card.RightInteraction();
            card1.RightInteraction();
            // only affects Vs-Mode
            if (!childsTurn)
            {
                card.SetFeedbackColor(parentsPairColor);
                card1.SetFeedbackColor(parentsPairColor);
            }
            activeCards.Remove(card);
            activeCards.Remove(card1);
            if (pairsToMatch == 0)
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
            // give player time to remember values
            yield return new WaitForSeconds(1f);
            PairInteraction(false);
            ExecuteEvents.Execute<IHelpSystem>(gameObject, null, (x, y) => x.WrongInteraction());
            card1.busy = true;
            card.busy = true;
            StartCoroutine(card1.WrongInteraction());
            StartCoroutine(card.WrongInteraction());

        }

        while (card1.busy || card.busy)
        {
            yield return new WaitForSeconds(0.1f);
        }
        card1 = null;
        waitingToTurnCoroutine = null;
    }

    protected virtual void PairInteraction(bool pairFound)
    {
        // Dummy for VS-mode
    }

    private IEnumerator DelayEnd()
    {
        // end for current method calls on children to finish
        yield return new WaitForSeconds(1);
        while (waitingToTurnCoroutine != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EndMiniGame();
    }

    private static readonly Vector2 smallSetOf = new Vector2(545, -100);
    private static readonly Vector2 mediumSetOf = new Vector2(280, -20);
    private static readonly Vector2 largeSetOf = new Vector2(150, -20);

    private Vector2 CalculateAnchoredPosition(int x, int y, MemorySize size)
    {
        switch (size)
        {
            case MemorySize.SMALL:
                return new Vector2(x * 250, y * -250) + smallSetOf;
            case MemorySize.MEDIUM:
                return new Vector2(x * 260, y * -230) + mediumSetOf;
            case MemorySize.LARGE:
                // original version: return new Vector2(x * 230, y * -230) + largeSetOf;
                return new Vector2(x * 215, y * -230) + largeSetOf;
        }
        return Vector2.zero;
    }

    private readonly Vector2 parentOffset = new Vector2(-800, 330);

    public override Vector2 GetHelpPosition1()
    {
        return activeCards[0].gameObject.GetComponent<RectTransform>().anchoredPosition + parentOffset;
    }

    public override Vector2 GetHelpPosition2()
    {
        if (!cardPairs.TryGetValue(activeCards[0].GetComponent<NumberObjectMemoryCard>(), out NumberObjectMemoryCard secondCard))
        {
            Debug.LogWarning("Could not find counterpart for active memory Card!");
            return GetHelpPosition1();
        }
        return secondCard.gameObject.GetComponent<RectTransform>().anchoredPosition + parentOffset;
    }

    #region Editor
    // editor functionality to debug number object layout
#if UNITY_EDITOR
    public void FillGrid(MemorySize size)
    {
        ClearGrid();

        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < (size == MemorySize.SMALL ? 4 : size == MemorySize.MEDIUM ? 10 : 14); i++)
        {
            objects.Add(Instantiate(memoryObjectPrefab, numberObjectParent.transform));
            objects.Add(Instantiate(memoryObjectPrefab, numberObjectParent.transform));
        }

        int x = 0, y = 0;
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<NumberObjectMemoryCard>().Mode = DisplayMode.TEXT;
            obj.GetComponent<NumberObjectMemoryCard>().Start();

            obj.GetComponent<RectTransform>().anchoredPosition = CalculateAnchoredPosition(x, y, size);
            x++;
            if (x >= (size == MemorySize.SMALL ? 3 : size == MemorySize.MEDIUM ? 5 : 7))
            {
                x = 0;
                y++;
            }
            else if (size == MemorySize.SMALL && x == 1 && y == 1)
            {
                x++;
            }
        }
    }
#endif
    #endregion Editor
}