using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// script offering a range of methods needed for story events
public class StoryEventsController : MonoBehaviour
{
    [Header("Doors - Gameplay")]
    [SerializeField] Button cockpitDoor;
    [SerializeField] Button labDoor;
    [SerializeField] Button engineDoor;

    [SerializeField] Button hubDoorCockpit;
    [SerializeField] Button hubDoorLab;
    [SerializeField] Button hubDoorEngine;

    [SerializeField] GameObject handPrefab;
    [SerializeField] private float scaleRange;
    [SerializeField] private float scaleSpeed;

    [Header("Doors - Story")]
    [SerializeField] Button cockpitDoorStory;
    [SerializeField] Button labDoorStory;
    [SerializeField] Button engineDoorStory;

    [SerializeField] Button hubDoorCockpitStory;
    [SerializeField] Button hubDoorLabStory;
    [SerializeField] Button hubDoorEngineStory;
    private StorySequenceController storyController;

    [Header("Main Menu Tutorial")]
    [SerializeField] Button toMainMenu;

    [Header("Story Reset Button")]
    [SerializeField] GameObject resetObjects;

    #region DOORS
    public void LockCockpit(string locked)
    {
        cockpitDoor.interactable = !locked.Equals("true");
    }

    public void LockLab(string locked)
    {
        labDoor.interactable = !locked.Equals("true");
    }

    public void LockEngine(string locked)
    {
        engineDoor.interactable = !locked.Equals("true");
    }

    private GameObject hand;
    private bool expand;
    private RectTransform rect;

    public void VisualizeDoor(string doorName)
    {
        if (doorName.Equals("cockpit"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, cockpitDoor.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 94 * Vector2.down + 85 * Vector2.right;
                hand.transform.rotation = Quaternion.Euler(0, 0, 35);
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else if (doorName.Equals("lab"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, labDoor.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 94 * Vector2.down + 85 * Vector2.left;
                hand.transform.rotation = Quaternion.Euler(0, 180, 35);
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else if (doorName.Equals("engine"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, engineDoor.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 35 * Vector2.down;
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else if (doorName.Equals("cockpitHub"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, hubDoorCockpit.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 94 * Vector2.down + 85 * Vector2.right;
                hand.transform.rotation = Quaternion.Euler(0, 0, 35);
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else if (doorName.Equals("labHub"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, hubDoorLab.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 94 * Vector2.down + 85 * Vector2.right;
                hand.transform.rotation = Quaternion.Euler(0, 0, 35);
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else if (doorName.Equals("engineHub"))
        {
            if (hand == null)
            {
                hand = Instantiate(handPrefab, hubDoorEngine.transform);
                rect = hand.GetComponent<RectTransform>();
                rect.anchoredPosition += 94 * Vector2.down + 85 * Vector2.left;
                hand.transform.rotation = Quaternion.Euler(0, 180, 35);
                expand = true;
            }
            else
            {
                DestroyImmediate(hand);
            }
        }
        else
        {
            Debug.LogWarning("Unknown door: " + doorName);
        }
    }

    public void ActivateDoorDuringStory(string doorName)
    {
        Debug.Log("ActivateDoorDuringStory: " + doorName);
        if (doorName.Equals("cockpit"))
        {
            storyController.ActivateDoor(cockpitDoorStory.gameObject);
        }
        else if (doorName.Equals("lab"))
        {
            storyController.ActivateDoor(labDoorStory.gameObject);
        }
        else if (doorName.Equals("engine"))
        {
            storyController.ActivateDoor(engineDoorStory.gameObject);
        }
        else if (doorName.Equals("cockpitHub"))
        {
            storyController.ActivateDoor(hubDoorCockpitStory.gameObject);
        }
        else if (doorName.Equals("labHub"))
        {
            storyController.ActivateDoor(hubDoorLabStory.gameObject);
        }
        else if (doorName.Equals("engineHub"))
        {
            storyController.ActivateDoor(hubDoorEngineStory.gameObject);
        }
        else
        {
            Debug.LogWarning("Unknown door: " + doorName);
        }
    }

    public void ToggleHomeButton(string activate)
    {
        toMainMenu.gameObject.SetActive(activate.Equals("true"));
    }
    #endregion DOORS

    #region MiniGames
    StoryGamesCanvasController canvasController;

    public void DeactivateTaskSound(string miniGameType)
    {
        if (miniGameType.Equals("insert"))
        {
            canvasController.DeactivateMiniGameTaskText(0);
        }
        else if (miniGameType.Equals("count"))
        {
            canvasController.DeactivateMiniGameTaskText(1);
        }
        else if (miniGameType.Equals("pairs"))
        {
            canvasController.DeactivateMiniGameTaskText(2);
        }
        else if (miniGameType.Equals("add"))
        {
            canvasController.DeactivateMiniGameTaskText(3);
        }
        else if (miniGameType.Equals("memory"))
        {
            canvasController.DeactivateMiniGameTaskText(4);
        }
        else if (miniGameType.Equals("connect"))
        {
            canvasController.DeactivateMiniGameTaskText(5);
        }
        else if (miniGameType.Equals("memoryVS"))
        {
            canvasController.DeactivateMiniGameTaskText(6);
        }
        else if (miniGameType.Equals("countVS"))
        {
            canvasController.DeactivateMiniGameTaskText(7);
        }
        else if (miniGameType.Equals("connectVS"))
        {
            canvasController.DeactivateMiniGameTaskText(8);
        }
        else
        {
            Debug.LogWarning("\"" + miniGameType + "\" is not valid parameter string vor DeactiateTaskSound!");
        }
    }

    #endregion MiniGames

    public void UnlockFreePlay()
    {
        PlayerPrefsController.SafeFreePlayUnlock(true);
    }

    public void ToggleStoryReset(string activate)
    {
        resetObjects.SetActive(activate.Equals("true"));
    }

    private void Awake()
    {
        canvasController = FindObjectOfType<StoryGamesCanvasController>();
        storyController = FindObjectOfType<StorySequenceController>();

        cockpitDoorStory.gameObject.SetActive(false);
        labDoorStory.gameObject.SetActive(false);
        engineDoorStory.gameObject.SetActive(false);
        hubDoorCockpitStory.gameObject.SetActive(false);
        hubDoorLabStory.gameObject.SetActive(false);
        hubDoorEngineStory.gameObject.SetActive(false);

        toMainMenu.gameObject.SetActive(false);

        resetObjects.SetActive(false);
    }

    private void Update()
    {
        if (hand != null)
        {
            if (expand)
            {
                rect.localScale += scaleSpeed * Time.deltaTime * Vector3.one;
                if (rect.localScale.x >= 1 + scaleRange)
                {
                    expand = false;
                }
            }
            else
            {
                rect.localScale -= scaleSpeed * Time.deltaTime * Vector3.one;
                if (rect.localScale.x <= 1 - scaleRange)
                {
                    expand = true;
                }
            }
        }
    }
}
