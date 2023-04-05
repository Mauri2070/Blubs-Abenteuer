using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Progress Step", menuName = "LegacyProgressStep")]
public class LegacyProgressStep : ScriptableObject
{
    public enum Room
    {
        HUB, COCKPIT, ENGINE, LAB, TEST
    }

    [Header("Hub")]
    [SerializeField] public Sprite hubBackground;
    [SerializeField] public Vector3 completeGameAnchorPoint;
    [SerializeField] public MiniGameOptions completeMiniGame;
    [SerializeField] public GameObject hubGamePrefab;
    [SerializeField] public StorySequence completeStorySequence;

    [Header("Cockpit")]
    [SerializeField] public Sprite cockpitBackground;
    [SerializeField] public Vector3[] cockpitGamesAnchorPoints;
    [SerializeField] public MiniGameOptions[] cockpitMiniGames;
    [SerializeField] public GameObject cockpitGamePrefab;
    [SerializeField] public StorySequence cockpitCompleteStorySequence;
    //private bool[] cockpitGameCompleted;
    //private int cockpitToComplete;
    private int cockpitIdx;

    [Header("Engine")]
    [SerializeField] public Sprite engineBackgorund;
    [SerializeField] public Vector3[] engineGamesAnchorPoints;
    [SerializeField] public MiniGameOptions[] engineMiniGames;
    [SerializeField] public GameObject engineGamePrefab;
    [SerializeField] public StorySequence engineCompleteStorySequence;
    //private bool[] engineGameCompleted;
    //private int engineToComplete;
    private int engineIdx;

    [Header("Lab")]
    [SerializeField] public Sprite labBackground;
    [SerializeField] public Vector3[] labGamesAnchorPoints;
    [SerializeField] public MiniGameOptions[] labMiniGames;
    [SerializeField] public GameObject labGamePrefab;
    [SerializeField] public StorySequence labCompleteStorySequence;
    //private bool[] labGameCompleted;
    //private int labToComplete;
    private int labIdx;

    /** Methods only needed for random mini game order
    private void Awake()
    {
        PrepareProgressStep();
    }

    public void PrepareProgressStep()
    {
        cockpitGameCompleted = new bool[cockpitMiniGames.Length];
        if (cockpitGamesAnchorPoints.Length != cockpitMiniGames.Length)
        {
            Debug.LogWarning("ProgressStep " + this.ToString() + ": number of cockpitGamesAnchors != number of cockpitMiniGames!");
        }

        engineGameCompleted = new bool[engineMiniGames.Length];
        if (engineGamesAnchorPoints.Length != engineMiniGames.Length)
        {
            Debug.LogWarning("ProgressStep " + this.ToString() + ": number of engineGamesAnchors != number of engineMiniGames!");
        }

        labGameCompleted = new bool[labMiniGames.Length];
        if (labGamesAnchorPoints.Length != labMiniGames.Length)
        {
            Debug.LogWarning("ProgressStep " + this.ToString() + ": number of labGamesAnchors != number of labMiniGames!");
        }

        if (cockpitGameCompleted.Length + engineGameCompleted.Length + labGameCompleted.Length > 32)
        {
            Debug.LogWarning("ProgressStep " + this.ToString() + ": added number of MiniGames to large! Saving won`t work properly.");
        }
    }
    */

    public void SafeProgressStep()
    {
        int key = 0;
        key += cockpitIdx;
        key <<= 10;
        key += engineIdx;
        key <<= 10;
        key += labIdx;

        /** Old version with random game order
        for (int i = 0; i < cockpitGameCompleted.Length; i++)
        {
            if (cockpitGameCompleted[i])
            {
                key++;
            }
            key <<= 1;
        }
        for (int i = 0; i < engineGameCompleted.Length; i++)
        {
            if (engineGameCompleted[i])
            {
                key++;
            }
            key <<= 1;
        }
        for (int i = 0; i < labGameCompleted.Length; i++)
        {
            if (labGameCompleted[i])
            {
                key++;
            }
            key <<= 1;
        }
        */

        //PlayerPrefsController.SaveProgressStepStatus(key);
    }

    private static readonly int KEY_MASK = 1023;
    public void LoadProgressStep()
    {
        Debug.Log("Loading ProgressStep " + name);
        int key = 0; // PlayerPrefsController.GetProgressStepStatus();

        labIdx = key & KEY_MASK;
        key >>= 10;

        engineIdx = key & KEY_MASK;
        key >>= 10;

        cockpitIdx = key;

        /** Old version with random game order
        if (1 << cockpitGameCompleted.Length + engineGameCompleted.Length + labGameCompleted.Length < key)
        {
            Debug.Log("Invalid ProgressStepStatus for ProgressStep" + name + ". No loading possible");
            return;
        }

        labToComplete = labGameCompleted.Length;
        for (int i = labGameCompleted.Length - 1; i >= 0; i--)
        {
            if (key % 2 == 1)
            {
                labGameCompleted[i] = true;
                labToComplete--;
            }
            key >>= 1;
        }
        engineToComplete = engineGameCompleted.Length;
        for (int i = engineGameCompleted.Length - 1; i >= 0; i--)
        {
            if (key % 2 == 1)
            {
                engineGameCompleted[i] = true;
                engineToComplete--;
            }
            key >>= 1;
        }
        cockpitToComplete = cockpitGameCompleted.Length;
        for (int i = cockpitGameCompleted.Length - 1; i >= 0; i--)
        {
            if (key % 2 == 1)
            {
                cockpitGameCompleted[i] = true;
                cockpitToComplete--;
            }
            key >>= 1;
        }
        */
    }

    public void GenerateMiniGameButtons(Room room, Transform parent, StorySceneCanvasController controller)
    {
        /** Old version with random game order
        Debug.Log("Generating MiniGame Buttons for ProgressStep " + name + " in Room " + room);
        GameObject instantiated;
        switch (room)
        {
            case Room.HUB:
                // skip, if other rooms are not yet completed
                if (cockpitToComplete != 0 || engineToComplete != 0 || labToComplete != 0)
                {
                    return;
                }
                else if (hubGamePrefab == null)
                {
                    Debug.LogWarning("hubGamePrefab for ProgressStep " + name + " not set");
                    return;
                }
                instantiated = Instantiate(hubGamePrefab, parent);
                GameObject insta = instantiated;
                instantiated.GetComponent<Button>().onClick.AddListener(() =>
                {
                    controller.StartMiniGame(completeMiniGame);
                    insta.SetActive(false);
                });
                instantiated.transform.position = completeGameAnchorPoint;
                break;
            case Room.COCKPIT:
                for (int i = 0; i < cockpitMiniGames.Length; i++)
                {
                    if (!cockpitGameCompleted[i])
                    {
                        instantiated = Instantiate(cockpitGamePrefab, parent);
                        MiniGameOptions options = cockpitMiniGames[i];
                        GameObject inst = instantiated;
                        instantiated.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            controller.StartMiniGame(options);
                            inst.SetActive(false);
                        });
                        instantiated.transform.position = cockpitGamesAnchorPoints[i];
                    }
                }
                break;
            case Room.ENGINE:
                for (int i = 0; i < engineMiniGames.Length; i++)
                {
                    if (!engineGameCompleted[i])
                    {
                        instantiated = Instantiate(engineGamePrefab, parent);
                        MiniGameOptions options = engineMiniGames[i];
                        GameObject inst = instantiated;
                        instantiated.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            controller.StartMiniGame(options);
                            inst.SetActive(false);
                        });
                        instantiated.transform.position = engineGamesAnchorPoints[i];
                    }
                }
                break;
            case Room.LAB:
                for (int i = 0; i < labMiniGames.Length; i++)
                {
                    if (!labMiniGames[i])
                    {
                        instantiated = Instantiate(labGamePrefab, parent);
                        MiniGameOptions options = labMiniGames[i];
                        GameObject inst = instantiated;
                        instantiated.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            controller.StartMiniGame(options);
                            inst.SetActive(false);
                        });
                        instantiated.transform.position = labGamesAnchorPoints[i];
                    }
                }
                break;
            default: return;
        }
        **/
        Debug.Log("Generating MiniGame Buttons for ProgressStep " + name + " in Room " + room);
        GameObject instantiated;
        switch (room)
        {
            case Room.HUB:
                // skip, if other rooms are not yet completed
                if (cockpitIdx < cockpitMiniGames.Length || engineIdx < engineMiniGames.Length || labIdx < labMiniGames.Length)
                {
                    return;
                }
                else if (hubGamePrefab == null)
                {
                    Debug.LogWarning("hubGamePrefab for ProgressStep " + name + " not set");
                    return;
                }
                instantiated = Instantiate(hubGamePrefab, parent);
                GameObject insta = instantiated;
                instantiated.GetComponent<Button>().onClick.AddListener(() =>
                {
                    controller.StartMiniGame(completeMiniGame);
                    insta.SetActive(false);
                });
                instantiated.transform.position = completeGameAnchorPoint;
                break;
            case Room.COCKPIT:
                for (int i = cockpitIdx; i < cockpitMiniGames.Length; i++)
                {
                    instantiated = Instantiate(cockpitGamePrefab, parent);
                    MiniGameOptions options = cockpitMiniGames[i];
                    GameObject inst = instantiated;
                    instantiated.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        controller.StartMiniGame(options);
                        inst.SetActive(false);
                    });
                    instantiated.transform.position = cockpitGamesAnchorPoints[i];
                }
                break;
            case Room.ENGINE:
                for (int i = engineIdx; i < engineMiniGames.Length; i++)
                {
                    instantiated = Instantiate(engineGamePrefab, parent);
                    MiniGameOptions options = engineMiniGames[i];
                    GameObject inst = instantiated;
                    instantiated.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        controller.StartMiniGame(options);
                        inst.SetActive(false);
                    });
                    instantiated.transform.position = engineGamesAnchorPoints[i];
                }
                break;
            case Room.LAB:
                for (int i = labIdx; i < labMiniGames.Length; i++)
                {
                    instantiated = Instantiate(labGamePrefab, parent);
                    MiniGameOptions options = labMiniGames[i];
                    GameObject inst = instantiated;
                    instantiated.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        controller.StartMiniGame(options);
                        inst.SetActive(false);
                    });
                    instantiated.transform.position = labGamesAnchorPoints[i];
                }
                break;
            default: return;
        }
    }

    public bool GameCompleted(Room room)
    {
        /** old version for random mini game order
        switch (room)
        {
            case Room.COCKPIT:
                cockpitToComplete--;
                cockpitGameCompleted[idx] = true;
                return cockpitToComplete == 0;
            case Room.ENGINE:
                engineToComplete--;
                engineGameCompleted[idx] = true;
                return engineToComplete == 0;
            case Room.LAB:
                labToComplete--;
                labGameCompleted[idx] = true;
                return labToComplete == 0;
            case Room.HUB:
                return true;
            default:
                return false;
        }
        */
        switch (room)
        {
            case Room.HUB:
                return true;
            case Room.COCKPIT:
                cockpitIdx++;
                return cockpitIdx == cockpitMiniGames.Length;
            case Room.ENGINE:
                engineIdx++;
                return engineIdx == engineMiniGames.Length;
            case Room.LAB:
                labIdx++;
                return labIdx == labMiniGames.Length;
            default:
                return false;
        }
    }
}
