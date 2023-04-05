using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// drag-and-drop target implementation
public class NumberObjectTarget : MonoBehaviour, IDropHandler
{
    private int targetKey;
    public int TargetKey
    {
        get { return targetKey; }
        set { targetKey = value; }
    }

    private MiniGameType gameType;
    public MiniGameType GameType
    {
        get { return gameType; }
        set { gameType = value; }
    }

    private bool acceptElement = true;
    public bool AcceptElement
    {
        get
        {
            return acceptElement;
        }
    }

    private static InsertMiniGame insertMiniGame;
    private static PairsMiniGame pairsMiniGame;
    private static AddMiniGame addMiniGame;
    private static ConnectMiniGame connectMiniGame;
    private static ConnectVsMiniGame connectVsMiniGame;

    private void Awake()
    {
        if (insertMiniGame == null)
        {
            insertMiniGame = FindObjectOfType<InsertMiniGame>();
            //Debug.Log("Finding InsertMiniGame");
        }
        if (pairsMiniGame == null)
        {
            pairsMiniGame = FindObjectOfType<PairsMiniGame>();
            //Debug.Log("Finding PaisMiniGame");
        }
        if (addMiniGame == null)
        {
            addMiniGame = FindObjectOfType<AddMiniGame>();
            //Debug.Log("Finding AddMiniGame");
        }
        if (connectMiniGame == null)
        {
            connectMiniGame = FindObjectOfType<ConnectMiniGame>();
            //Debug.Log("Finding ConnectMiniGame");
        }
        if (connectVsMiniGame == null)
        {
            connectVsMiniGame = FindObjectOfType<ConnectVsMiniGame>();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        NumberObject numberObject;
        if (eventData.pointerDrag != null && eventData.pointerDrag != gameObject)
        {
            switch (gameType)
            {
                case MiniGameType.INSERT:
                    if ((numberObject = eventData.pointerDrag.GetComponent<NumberObjectDnD>()) != null && insertMiniGame.ShouldAccept(targetKey, numberObject.Value))
                    {
                        if (acceptElement)
                        {
                            numberObject.IsInteractable = false;
                            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
                            numberObject.RightInteraction();
                            insertMiniGame.GapFilled();
                            ElementAccepted();
                        }
                        else
                        {
                            StartCoroutine(numberObject.WrongInteraction());
                            numberObject.GetComponent<NumberObjectDnD>().ResetDnD();
                        }
                    }
                    else
                    {
                        WrongInteractionOrMissingScript(eventData.pointerDrag.GetComponent<NumberObject>());
                    }
                    break;
                case MiniGameType.PAIRS:
                    if ((numberObject = eventData.pointerDrag.GetComponent<NumberObjectLineDnD>()) != null && pairsMiniGame.ShouldAccept(targetKey, numberObject.Value))
                    {
                        if (acceptElement)
                        {
                            // deactivate both numberObjects
                            numberObject.IsInteractable = false;
                            numberObject.gameObject.GetComponent<NumberObjectTarget>().acceptElement = false;
                            gameObject.GetComponent<NumberObject>().IsInteractable = false;

                            ((NumberObjectLineDnD)numberObject).SetLinePosition(GetComponent<RectTransform>().position, 1);
                            ((NumberObjectLineDnD)numberObject).SetLinePosition(numberObject.gameObject.GetComponent<RectTransform>().position, 0);

                            numberObject.RightInteraction();
                            gameObject.GetComponent<NumberObject>().RightInteraction();

                            pairsMiniGame.PairMatched();

                            // deactivate both numberObjectTargets
                            eventData.pointerDrag.GetComponent<NumberObjectTarget>().acceptElement = false;
                            ElementAccepted();
                        }
                        else
                        {
                            StartCoroutine(numberObject.WrongInteraction());
                            eventData.pointerDrag.GetComponent<NumberObjectLineDnD>().ResetLineDnD();
                        }
                    }
                    else
                    {
                        if (!gameObject.GetComponent<NumberObject>().IsInteractable || numberObject != null && !numberObject.IsInteractable)
                        {
                            return;
                        }
                        if (acceptElement)
                        {
                            StartCoroutine(gameObject.GetComponent<NumberObject>().WrongInteraction());
                        }
                        WrongInteractionOrMissingScript(eventData.pointerDrag.GetComponent<NumberObject>());
                    }
                    break;
                case MiniGameType.ADD:
                    if ((numberObject = eventData.pointerDrag.GetComponent<NumberObjectLineDnD>()) != null && addMiniGame.ShouldAccept(targetKey, numberObject.Value))
                    {
                        if (acceptElement)
                        {
                            addMiniGame.RemoveElementsFromHelp(targetKey, gameObject, numberObject.Value, numberObject.gameObject);

                            // deactivate both numberObjects
                            numberObject.IsInteractable = false;
                            gameObject.GetComponent<NumberObject>().IsInteractable = false;

                            ((NumberObjectLineDnD)numberObject).SetLinePosition(GetComponent<RectTransform>().position, 1);
                            ((NumberObjectLineDnD)numberObject).SetLinePosition(numberObject.gameObject.GetComponent<RectTransform>().position, 0);

                            numberObject.RightInteraction();
                            gameObject.GetComponent<NumberObject>().RightInteraction();

                            addMiniGame.PairMatched();

                            // deactivate both numberObjectTargets
                            eventData.pointerDrag.GetComponent<NumberObjectTarget>().acceptElement = false;
                            ElementAccepted();

                            StartCoroutine(AddFeedbackWaiter(numberObject.gameObject, gameObject));
                        }
                        else
                        {
                            // this code is only executed for very fast interactions!
                            StartCoroutine(numberObject.WrongInteraction());
                            eventData.pointerDrag.GetComponent<NumberObjectLineDnD>().ResetLineDnD();
                        }
                    }
                    else
                    {
                        StartCoroutine(gameObject.GetComponent<NumberObject>().WrongInteraction());
                        WrongInteractionOrMissingScript(eventData.pointerDrag.GetComponent<NumberObject>());
                    }
                    break;
                case MiniGameType.CONNECT:
                    if ((numberObject = eventData.pointerDrag.GetComponent<NumberObjectLineDnD>()) != null && connectMiniGame.ShouldAccept(targetKey, numberObject.Value))
                    {
                        if (acceptElement)
                        {
                            // deactivate both numberObjects
                            numberObject.IsInteractable = false;
                            numberObject.gameObject.GetComponent<NumberObjectTarget>().acceptElement = false;
                            gameObject.GetComponent<NumberObject>().IsInteractable = false;

                            ((NumberObjectLineDnD)numberObject).SetLinePosition(GetComponent<RectTransform>().position, 1);
                            //Debug.Log("P1"+GetComponent<RectTransform>().position);
                            ((NumberObjectLineDnD)numberObject).SetLinePosition(numberObject.gameObject.GetComponent<RectTransform>().position, 0);
                            //Debug.Log("P2"+numberObject.gameObject.GetComponent<RectTransform>().position);

                            numberObject.RightInteraction();
                            gameObject.GetComponent<NumberObject>().RightInteraction();

                            connectMiniGame.ConnectPair();

                            // deactivate both numberObjectTargets
                            eventData.pointerDrag.GetComponent<NumberObjectTarget>().acceptElement = false;
                            ElementAccepted();
                        }
                        else
                        {
                            StartCoroutine(numberObject.WrongInteraction());
                            eventData.pointerDrag.GetComponent<NumberObjectLineDnD>().ResetLineDnD();
                        }
                    }
                    else
                    {
                        if (!gameObject.GetComponent<NumberObject>().IsInteractable || numberObject != null && !numberObject.IsInteractable)
                        {
                            return;
                        }
                        if (acceptElement)
                        {
                            StartCoroutine(gameObject.GetComponent<NumberObject>().WrongInteraction());
                        }
                        WrongInteractionOrMissingScript(eventData.pointerDrag.GetComponent<NumberObject>());
                    }
                    break;
                case MiniGameType.CONNECT_VS:
                    if ((numberObject = eventData.pointerDrag.GetComponent<NumberObjectLineDnD>()) != null && connectVsMiniGame.ShouldAccept(targetKey, numberObject.Value))
                    {
                        if (acceptElement)
                        {
                            // deactivate both numberObjects
                            numberObject.IsInteractable = false;
                            numberObject.gameObject.GetComponent<NumberObjectTarget>().acceptElement = false;
                            gameObject.GetComponent<NumberObject>().IsInteractable = false;

                            ((NumberObjectLineDnD)numberObject).SetLinePosition(GetComponent<RectTransform>().position, 1);
                            ((NumberObjectLineDnD)numberObject).SetLinePosition(numberObject.gameObject.GetComponent<RectTransform>().position, 0);

                            numberObject.RightInteraction();
                            gameObject.GetComponent<NumberObject>().RightInteraction();

                            connectVsMiniGame.ConnectPair(eventData.pointerDrag.GetComponent<NumberObject>().childButton);

                            // deactivate both numberObjectTargets
                            eventData.pointerDrag.GetComponent<NumberObjectTarget>().acceptElement = false;
                            ElementAccepted();
                        }
                        else
                        {
                            StartCoroutine(numberObject.WrongInteraction());
                            eventData.pointerDrag.GetComponent<NumberObjectLineDnD>().ResetLineDnD();
                        }
                    }
                    else
                    {
                        if (!gameObject.GetComponent<NumberObject>().IsInteractable || numberObject != null && !numberObject.IsInteractable)
                        {
                            return;
                        }
                        if (acceptElement)
                        {
                            StartCoroutine(gameObject.GetComponent<NumberObject>().WrongInteraction());
                        }
                        WrongInteractionOrMissingScript(eventData.pointerDrag.GetComponent<NumberObject>());
                    }
                    break;
                default:
                    Debug.Log("NumberObjectTarget.OnDrop has no functionality for game Type " + gameType.ToString());
                    break;
            }
        }
    }

    private void WrongInteractionOrMissingScript(NumberObject numberObject)
    {
        if (numberObject != null)
        {
            StartCoroutine(numberObject.WrongInteraction());
            Debug.Log("Falsche Interaktion oder falsches NumberObject-Script.");
        }
        else
        {
            Debug.Log("Ein NumberObject-Script fehlt.");
        }
    }

    private void ElementAccepted()
    {
        Debug.Log("Accepting Element");
        acceptElement = false;
    }

    private IEnumerator AddFeedbackWaiter(GameObject other, GameObject current)
    {
        yield return new WaitForSeconds(1f);
        other.SetActive(false);
        current.SetActive(false);
    }
}
