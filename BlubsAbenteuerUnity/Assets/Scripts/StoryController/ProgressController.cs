using UnityEngine;
using UnityEngine.UI;

// script for handling all things related to story progress
public class ProgressController : MonoBehaviour
{
    [Header("Hub")]
    [SerializeField] private ProgressStep[] hubSteps;
    private int hubIdx;
    [SerializeField] private Sprite[] hubBackgroundSprites;
    private int hubBackgroundIdx;
    [SerializeField] private OutputContainer defaultHubOutput;
    private OutputContainer currentHubOutput;
    //[SerializeField] private ProgressBarInformation[] hubProgressInfos;

    [Header("Lab")]
    [SerializeField] private ProgressStep[] labSteps;
    private int labIdx;
    [SerializeField] private Sprite[] labBackgroundSprites;
    private int labBackgroundIdx;
    [SerializeField] private OutputContainer defaultLabOutput;
    private OutputContainer currentLabOutput;
    [SerializeField] private ProgressBarInformation[] labProgressInfos;

    [Header("Nav")]
    [SerializeField] private ProgressStep[] navSteps;
    private int navIdx;
    [SerializeField] private Sprite[] navBackgroundSprites;
    private int navBackgroundIdx;
    [SerializeField] private OutputContainer defaultNavOutput;
    private OutputContainer currentNavOutput;
    [SerializeField] private ProgressBarInformation[] navProgressInfos;

    [Header("Engine")]
    [SerializeField] private ProgressStep[] engineSteps;
    private int engineIdx;
    [SerializeField] private Sprite[] engineBackgroundSprites;
    private int engineBackgroundIdx;
    [SerializeField] private OutputContainer defaultEngineOutput;
    private OutputContainer currentEngineOutput;
    [SerializeField] private ProgressBarInformation[] engineProgressInfos;

    [Header("Room Output Menu Objects")]
    [SerializeField] private OutputDistributer roomOutput;
    [SerializeField] private OutputContainer storyCompletedOutput;

    [Header("Game Start Button Prefab for story games")]
    [SerializeField] private GameObject selectorPrefab;

    private StorySceneCanvasController canvasController;
    private ProgressBarController progressBarController;
    private Room currentRoom;

    private void Awake()
    {
        canvasController = FindObjectOfType<StorySceneCanvasController>();
        progressBarController = FindObjectOfType<ProgressBarController>();
    }

    private void Start()
    {
        LoadProgress();
    }

    public void LoadRoom(Room room)
    {
        Debug.Log("Loading room " + room);
        currentRoom = room;
        switch (room)
        {
            case Room.HUB:
                canvasController.ChangeBackground(hubBackgroundSprites[hubBackgroundIdx]);
                progressBarController.ChangeProgressBar(null);
                if (currentHubOutput == null)
                {
                    roomOutput.output = defaultHubOutput;
                }
                else
                {
                    roomOutput.output = currentHubOutput;
                }
                roomOutput.Distribute();
                break;
            case Room.NAV:
                canvasController.ChangeBackground(navBackgroundSprites[navBackgroundIdx]);

                if (navIdx >= navSteps.Length)
                {
                    navIdx = navSteps.Length - 1;
                }

                if (!progressBarActive)
                {
                    progressBarController.ChangeProgressBar(null);
                }
                else if (typeof(GamesProgressStep).IsInstanceOfType(navSteps[navIdx]))
                {
                    progressBarController.ChangeProgressBar(navProgressInfos[Mathf.Min(navProgressInfos.Length - 1, navBackgroundIdx)], navIdx, ((GamesProgressStep)navSteps[navIdx]).Idx);
                }
                else
                {
                    progressBarController.ChangeProgressBar(navProgressInfos[Mathf.Min(navProgressInfos.Length - 1, navBackgroundIdx)], navIdx);
                }

                if (currentNavOutput == null)
                {
                    roomOutput.output = defaultNavOutput;
                }
                else
                {
                    roomOutput.output = currentNavOutput;
                }
                roomOutput.Distribute();
                break;
            case Room.LAB:
                canvasController.ChangeBackground(labBackgroundSprites[labBackgroundIdx]);

                if (labIdx >= labSteps.Length)
                {
                    labIdx = labSteps.Length - 1;
                }

                if (!progressBarActive)
                {
                    progressBarController.ChangeProgressBar(null);
                }
                else if (typeof(GamesProgressStep).IsInstanceOfType(labSteps[labIdx]))
                {
                    progressBarController.ChangeProgressBar(labProgressInfos[Mathf.Min(labProgressInfos.Length - 1, labBackgroundIdx)], labIdx, ((GamesProgressStep)labSteps[labIdx]).Idx);
                }
                else
                {
                    progressBarController.ChangeProgressBar(labProgressInfos[Mathf.Min(labProgressInfos.Length - 1, labBackgroundIdx)], labIdx);
                }

                if (currentLabOutput == null)
                {
                    roomOutput.output = defaultLabOutput;
                }
                else
                {
                    roomOutput.output = currentLabOutput;
                }
                roomOutput.Distribute();
                break;
            case Room.ENGINE:
                canvasController.ChangeBackground(engineBackgroundSprites[engineBackgroundIdx]);

                if (engineIdx >= engineSteps.Length)
                {
                    engineIdx = engineSteps.Length - 1;
                }

                if (!progressBarActive)
                {
                    progressBarController.ChangeProgressBar(null);
                }
                else if (typeof(GamesProgressStep).IsInstanceOfType(engineSteps[engineIdx]))
                {
                    progressBarController.ChangeProgressBar(engineProgressInfos[Mathf.Min(engineProgressInfos.Length - 1, engineBackgroundIdx)], engineIdx, ((GamesProgressStep)engineSteps[engineIdx]).Idx);
                }
                else
                {
                    progressBarController.ChangeProgressBar(engineProgressInfos[Mathf.Min(engineProgressInfos.Length - 1, engineBackgroundIdx)], engineIdx);
                }

                if (currentEngineOutput == null)
                {
                    roomOutput.output = defaultEngineOutput;
                }
                else
                {
                    roomOutput.output = currentEngineOutput;
                }
                roomOutput.Distribute();
                break;
        }
        Execute(room);
    }

    #region EventFunctions
    public void ResetRoomOutputEnum(Room room)
    {
        switch (room)
        {
            case Room.HUB:
                currentHubOutput = null;
                break;
            case Room.NAV:
                currentNavOutput = null;
                break;
            case Room.LAB:
                currentLabOutput = null;
                break;
            case Room.ENGINE:
                currentEngineOutput = null;
                break;
        }
    }

    // Method for easy reflection-use
    public void ResetRoomOutput(string room)
    {
        if (room.Equals("hub"))
        {
            ResetRoomOutputEnum(Room.HUB);
        }
        else if (room.Equals("nav"))
        {
            ResetRoomOutputEnum(Room.NAV);
        }
        else if (room.Equals("lab"))
        {
            ResetRoomOutputEnum(Room.LAB);
        }
        else if (room.Equals("engine"))
        {
            ResetRoomOutputEnum(Room.ENGINE);
        }
        else
        {
            Debug.LogError(room + " is not a valid room string!");
        }
    }

    public void AdvanceBackgroundEnum(Room room)
    {
        switch (room)
        {
            case Room.HUB:
                if (hubBackgroundIdx == hubBackgroundSprites.Length - 1)
                {
                    Debug.LogWarning("Hub background already at last index!");
                    return;
                }
                hubBackgroundIdx++;
                if (currentRoom == room)
                {
                    canvasController.ChangeBackground(hubBackgroundSprites[hubBackgroundIdx]);
                }
                break;
            case Room.NAV:
                if (navBackgroundIdx == navBackgroundSprites.Length - 1)
                {
                    Debug.LogWarning("Nav background already at last index!");
                    return;
                }
                navBackgroundIdx++;
                if (currentRoom == room)
                {
                    canvasController.ChangeBackground(navBackgroundSprites[navBackgroundIdx]);
                }
                break;
            case Room.LAB:
                if (labBackgroundIdx == labBackgroundSprites.Length - 1)
                {
                    Debug.LogWarning("Lab background already at last index!");
                    return;
                }
                labBackgroundIdx++;
                if (currentRoom == room)
                {
                    canvasController.ChangeBackground(labBackgroundSprites[labBackgroundIdx]);
                }
                break;
            case Room.ENGINE:
                if (engineBackgroundIdx == engineBackgroundSprites.Length - 1)
                {
                    Debug.LogWarning("Hub background already at last index!");
                    return;
                }
                engineBackgroundIdx++;
                if (currentRoom == room)
                {
                    canvasController.ChangeBackground(engineBackgroundSprites[engineBackgroundIdx]);
                }
                break;
        }
    }

    // Method for easy reflection-use
    public void AdvanceBackground(string room)
    {
        Debug.Log("Advance background for " + room);
        if (room.Equals("hub"))
        {
            AdvanceBackgroundEnum(Room.HUB);
        }
        else if (room.Equals("nav"))
        {
            AdvanceBackgroundEnum(Room.NAV);
        }
        else if (room.Equals("lab"))
        {
            AdvanceBackgroundEnum(Room.LAB);
        }
        else if (room.Equals("engine"))
        {
            AdvanceBackgroundEnum(Room.ENGINE);
        }
        else
        {
            Debug.LogError(room + " is not a valid room string!");
        }
    }

    bool playRoomAudio = true;
    public void ToggleRoomOutputOnChange()
    {
        playRoomAudio = !playRoomAudio;
    }
    #endregion EventFunctions

    #region Progression
    private void Progress(Room room)
    {
        switch (room)
        {
            case Room.HUB:
                hubIdx++;
                if (hubIdx == hubSteps.Length)
                {
                    hubIdx--;
                    return;
                }
                break;
            case Room.NAV:
                navIdx++;
                if (navIdx == navSteps.Length)
                {
                    navIdx--;
                    return;
                }
                break;
            case Room.LAB:
                labIdx++;
                if (labIdx == labSteps.Length)
                {
                    labIdx--;
                    return;
                }
                break;
            case Room.ENGINE:
                engineIdx++;
                if (engineIdx == engineSteps.Length)
                {
                    engineIdx--;
                    return;
                }
                break;
        }
        Execute(room);
    }

    private void Execute(Room room)
    {
        Debug.Log("Calling ProgressStep.Execute for " + room);
        ProgressStep currentStep = null;
        switch (room)
        {
            case Room.HUB:
                if (hubIdx < hubSteps.Length)
                {
                    //Debug.Log("current progress step for hub: " + hubSteps[hubIdx].name);
                    foreach (ProgressStep ps in hubSteps[hubIdx].Preconditions)
                    {
                        //Debug.Log("Precondition: " + ps.name + " - completed: " + ps.Completed);
                        if (!ps.Completed)
                        {
                            // display room idle -> go to other room
                            roomOutput.output = defaultHubOutput;
                            roomOutput.Distribute();
                            roomOutput.PlayAudio(!playRoomAudio);
                            return;
                        }
                    }
                    currentStep = hubSteps[hubIdx];
                    if (currentStep.HasRoomOutput(out OutputContainer newOutput))
                    {
                        currentHubOutput = newOutput;
                        roomOutput.output = currentHubOutput;
                        roomOutput.Distribute();
                        roomOutput.PlayAudio(!playRoomAudio);
                    }
                }
                else
                {
                    // room is finished completely!
                    Debug.Log("Hub story completed.");
                    roomOutput.output = storyCompletedOutput;
                    roomOutput.Distribute();
                    roomOutput.PlayAudio(!playRoomAudio);
                    return;
                }
                break;
            case Room.NAV:
                if (navIdx < navSteps.Length)
                {
                    foreach (ProgressStep ps in navSteps[navIdx].Preconditions)
                    {
                        if (!ps.Completed)
                        {
                            // display room idle -> go to other room
                            roomOutput.output = defaultNavOutput;
                            roomOutput.Distribute();
                            roomOutput.PlayAudio(!playRoomAudio);
                            return;
                        }
                    }
                    currentStep = navSteps[navIdx];
                    if (currentStep.HasRoomOutput(out OutputContainer newOutput))
                    {
                        currentNavOutput = newOutput;
                        roomOutput.output = currentNavOutput;
                        roomOutput.Distribute();
                        roomOutput.PlayAudio(!playRoomAudio);
                    }
                }
                else
                {
                    // room is finished completely!
                    Debug.Log("Nav story completed.");
                    roomOutput.output = defaultNavOutput;
                    roomOutput.Distribute();
                    roomOutput.PlayAudio(!playRoomAudio);
                    return;
                }
                break;
            case Room.LAB:
                if (labIdx < labSteps.Length)
                {
                    foreach (ProgressStep ps in labSteps[labIdx].Preconditions)
                    {
                        if (!ps.Completed)
                        {
                            // display room idle -> go to other room
                            roomOutput.output = defaultLabOutput;
                            roomOutput.Distribute();
                            roomOutput.PlayAudio(!playRoomAudio);
                            return;
                        }
                    }
                    currentStep = labSteps[labIdx];
                    if (currentStep.HasRoomOutput(out OutputContainer newOutput))
                    {
                        currentLabOutput = newOutput;
                        roomOutput.output = currentLabOutput;
                        roomOutput.Distribute();
                        roomOutput.PlayAudio(!playRoomAudio);
                    }
                }
                else
                {
                    // room is finished completely!
                    Debug.Log("Lab story completed.");
                    roomOutput.output = defaultLabOutput;
                    roomOutput.Distribute();
                    roomOutput.PlayAudio(!playRoomAudio);
                    return;
                }
                break;
            case Room.ENGINE:
                if (engineIdx < engineSteps.Length)
                {
                    foreach (ProgressStep ps in engineSteps[engineIdx].Preconditions)
                    {
                        if (!ps.Completed)
                        {
                            // display room idle -> go to other room
                            roomOutput.output = defaultEngineOutput;
                            roomOutput.Distribute();
                            roomOutput.PlayAudio(!playRoomAudio);
                            return;
                        }
                    }
                    currentStep = engineSteps[engineIdx];
                    if (currentStep.HasRoomOutput(out OutputContainer newOutput))
                    {
                        currentEngineOutput = newOutput;
                        roomOutput.output = currentEngineOutput;
                        roomOutput.Distribute();
                        roomOutput.PlayAudio(!playRoomAudio);
                    }
                }
                else
                {
                    // room is finished completely!
                    Debug.Log("Engine story completed.");
                    roomOutput.output = defaultEngineOutput;
                    roomOutput.Distribute();
                    roomOutput.PlayAudio(!playRoomAudio);
                    return;
                }
                break;
        }

        Debug.Log("ProgressStep " + currentStep.name + " will be executed");
        if (typeof(GamesProgressStep).IsInstanceOfType(currentStep))
        {
            ((GamesProgressStep)currentStep).GenerateMiniGameButtons(canvasController.GameStartButtonsParent.transform, canvasController);
        }
        else if (typeof(StoryProgressStep).IsInstanceOfType(currentStep))
        {
            canvasController.StartStorySequence(((StoryProgressStep)currentStep).StorySequence);
        }
        else
        {
            ((ScriptedProgressStep)currentStep).Execute();
            currentStep.Completed = true;
            Progress(room);
        }
    }
    #endregion Progression

    #region MiniGames
    public void GameCompleted()
    {
        ProgressStep currentStep = null;
        switch (currentRoom)
        {
            case Room.HUB:
                currentStep = hubSteps[hubIdx];
                break;
            case Room.NAV:
                currentStep = navSteps[navIdx];
                break;
            case Room.LAB:
                currentStep = labSteps[labIdx];
                break;
            case Room.ENGINE:
                currentStep = engineSteps[engineIdx];
                break;
        }

        progressBarController.GameProgressed();

        if (typeof(StoryProgressStep).IsInstanceOfType(currentStep))
        {
            currentStep.Completed = true;
        }
        else
        {
            ((GamesProgressStep)currentStep).GameCompleted();
        }

        if (currentStep.Completed)
        {
            Progress(currentRoom);
        }
    }
    #endregion MiniGames

    #region Story
    public void EndStorySequence()
    {
        StoryProgressStep storyStep = null;
        switch (currentRoom)
        {
            case Room.HUB:
                storyStep = (StoryProgressStep)hubSteps[hubIdx];
                break;
            case Room.NAV:
                storyStep = (StoryProgressStep)navSteps[navIdx];
                break;
            case Room.LAB:
                storyStep = (StoryProgressStep)labSteps[labIdx];
                break;
            case Room.ENGINE:
                storyStep = (StoryProgressStep)engineSteps[engineIdx];
                break;
        }
        if (storyStep.HasFinalGame(out MiniGameOptions options))
        {
            GameObject instantiated = Instantiate(selectorPrefab, canvasController.GameStartButtonsParent.transform);
            instantiated.GetComponent<Button>().onClick.AddListener(() =>
            {
                canvasController.StartMiniGame(options, instantiated);
            });
            instantiated.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            instantiated.GetComponentsInChildren<Image>()[1].sprite = GameStartSpriteDistributer.instance.GetSprite(currentRoom);
            canvasController.StartMiniGame(options, instantiated);
        }
        else
        {
            storyStep.Completed = true;
            Progress(currentRoom);
        }
    }

    public void ResetFromStoryMode()
    {
        hubIdx = 0;
        hubBackgroundIdx = 0;
        navIdx = 0;
        navBackgroundIdx = 0;
        labIdx = 0;
        labBackgroundIdx = 0;
        engineIdx = 0;
        engineBackgroundIdx = 0;
        foreach (ProgressStep p in hubSteps)
        {
            p.Completed = false;
        }
        foreach (ProgressStep p in navSteps)
        {
            p.Completed = false;
        }
        foreach (ProgressStep p in labSteps)
        {
            p.Completed = false;
        }
        foreach (ProgressStep p in engineSteps)
        {
            p.Completed = false;
        }
        LoadRoom(Room.HUB);
    }
    #endregion Story

    #region Safe
    public void SafeProgress()
    {
        //Debug.Log("Safeing");
        PlayerPrefsController.SafeRoomIdx(Room.HUB, hubIdx);
        if (hubIdx < hubSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(hubSteps[hubIdx]))
        {
            PlayerPrefsController.SafeGameIdx(Room.HUB, ((GamesProgressStep)hubSteps[hubIdx]).Idx);
        }
        PlayerPrefsController.SafeRoomIdx(Room.NAV, navIdx);
        if (navIdx < navSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(navSteps[navIdx]))
        {
            PlayerPrefsController.SafeGameIdx(Room.NAV, ((GamesProgressStep)navSteps[navIdx]).Idx);
        }
        PlayerPrefsController.SafeRoomIdx(Room.LAB, labIdx);
        if (labIdx < labSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(labSteps[labIdx]))
        {
            PlayerPrefsController.SafeGameIdx(Room.LAB, ((GamesProgressStep)labSteps[labIdx]).Idx);
        }
        PlayerPrefsController.SafeRoomIdx(Room.ENGINE, engineIdx);
        if (engineIdx < engineSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(engineSteps[engineIdx]))
        {
            PlayerPrefsController.SafeGameIdx(Room.ENGINE, ((GamesProgressStep)engineSteps[engineIdx]).Idx);
        }
    }

    public void LoadProgress()
    {
        Debug.Log("Loading Progress");
        // Hub
        hubIdx = PlayerPrefsController.LoadRoomIdx(Room.HUB);
        Debug.Log("Hub idx: " + hubIdx);
        for (int i = 0; i < hubIdx; i++)
        {
            hubSteps[i].Completed = true;
            hubSteps[i].HasRoomOutput(out currentHubOutput);
            if (typeof(ScriptedProgressStep).IsInstanceOfType(hubSteps[i]))
            {
                ((ScriptedProgressStep)hubSteps[i]).Execute();
            }
        }
        bool checking = true;
        for (int i = hubIdx; i < hubSteps.Length; i++)
        {
            // reset completed-status if app is not restarted after resetting
            if (checking && typeof(ScriptedProgressStep).IsInstanceOfType(hubSteps[i]))
            {
                ((ScriptedProgressStep)hubSteps[i]).Execute();
                hubSteps[i].Completed = true;
                hubIdx++;
            }
            else
            {
                hubSteps[i].Completed = false;
                checking = false;
            }
        }
        if (hubIdx < hubSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(hubSteps[hubIdx]))
        {
            ((GamesProgressStep)hubSteps[hubIdx]).Idx = PlayerPrefsController.LoadGameIdx(Room.HUB);
            //Debug.Log("Loading Hub Games: " + PlayerPrefsController.LoadGameIdx(Room.HUB) + " - " + ((GamesProgressStep)hubSteps[hubIdx]).Idx);
            // avoid skipping MiniGames on load
            PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
        }
        // Nav
        navIdx = PlayerPrefsController.LoadRoomIdx(Room.NAV);
        Debug.Log("Nav idx: " + navIdx);
        for (int i = 0; i < navIdx; i++)
        {
            navSteps[i].Completed = true;
            navSteps[i].HasRoomOutput(out currentNavOutput);
            if (typeof(ScriptedProgressStep).IsInstanceOfType(navSteps[i]))
            {
                ((ScriptedProgressStep)navSteps[i]).Execute();
            }
        }
        checking = true;
        for (int i = navIdx; i < navSteps.Length; i++)
        {
            // reset completed-status if app is not restarted after resetting
            if (checking && typeof(ScriptedProgressStep).IsInstanceOfType(navSteps[i]))
            {
                ((ScriptedProgressStep)navSteps[i]).Execute();
                navSteps[i].Completed = true;
                navIdx++;
            }
            else
            {
                navSteps[i].Completed = false;
                checking = false;
            }

        }
        if (navIdx < navSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(navSteps[navIdx]))
        {
            ((GamesProgressStep)navSteps[navIdx]).Idx = PlayerPrefsController.LoadGameIdx(Room.NAV);
            // avoid skipping MiniGames on load
            PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
        }
        // Lab
        labIdx = PlayerPrefsController.LoadRoomIdx(Room.LAB);
        Debug.Log("Lab idx: " + labIdx);
        for (int i = 0; i < labIdx; i++)
        {
            labSteps[i].Completed = true;
            labSteps[i].HasRoomOutput(out currentLabOutput);
            if (typeof(ScriptedProgressStep).IsInstanceOfType(labSteps[i]))
            {
                ((ScriptedProgressStep)labSteps[i]).Execute();
            }
        }
        checking = true;
        for (int i = labIdx; i < labSteps.Length; i++)
        {
            // reset completed-status if app is not restarted after resetting
            if (checking && typeof(ScriptedProgressStep).IsInstanceOfType(labSteps[i]))
            {
                ((ScriptedProgressStep)labSteps[i]).Execute();
                labSteps[i].Completed = true;
                labIdx++;
            }
            else
            {
                labSteps[i].Completed = false;
                checking = false;
            }

        }
        if (labIdx < labSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(labSteps[labIdx]))
        {
            ((GamesProgressStep)labSteps[labIdx]).Idx = PlayerPrefsController.LoadGameIdx(Room.LAB);
            //Debug.Log("Loading Lab Games: " + PlayerPrefsController.LoadGameIdx(Room.LAB) + " - " + ((GamesProgressStep)labSteps[labIdx]).Idx);
            // avoid skipping MiniGames on load
            PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
        }
        // Engine
        engineIdx = PlayerPrefsController.LoadRoomIdx(Room.ENGINE);
        Debug.Log("Engine idx: " + engineIdx);
        for (int i = 0; i < engineIdx; i++)
        {
            engineSteps[i].Completed = true;
            engineSteps[i].HasRoomOutput(out currentEngineOutput);
            if (typeof(ScriptedProgressStep).IsInstanceOfType(engineSteps[i]))
            {
                ((ScriptedProgressStep)engineSteps[i]).Execute();
            }
        }
        checking = true;
        for (int i = engineIdx; i < engineSteps.Length; i++)
        {
            // reset completed-status if app is not restarted after resetting
            if (checking && typeof(ScriptedProgressStep).IsInstanceOfType(engineSteps[i]))
            {
                ((ScriptedProgressStep)engineSteps[i]).Execute();
                engineSteps[i].Completed = true;
                engineIdx++;
            }
            else
            {
                engineSteps[i].Completed = false;
                checking = false;
            }
        }
        if (engineIdx < engineSteps.Length && typeof(GamesProgressStep).IsInstanceOfType(engineSteps[engineIdx]))
        {
            ((GamesProgressStep)engineSteps[engineIdx]).Idx = PlayerPrefsController.LoadGameIdx(Room.ENGINE);
            // avoid skipping MiniGames on load
            PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
        }

        // Background Setup
        LoadRoom(Room.HUB);
    }

    public bool IsStepCompleted(Room room, int idx)
    {
        if (idx < 0)
        {
            return false;
        }
        switch (room)
        {
            case Room.HUB:
                if (idx < hubSteps.Length)
                {
                    return hubSteps[idx].Completed;
                }
                break;
            case Room.NAV:
                if (idx < navSteps.Length)
                {
                    return navSteps[idx].Completed;
                }
                break;
            case Room.LAB:
                if (idx < labSteps.Length)
                {
                    return labSteps[idx].Completed;
                }
                break;
            case Room.ENGINE:
                if (idx < engineSteps.Length)
                {
                    return engineSteps[idx].Completed;
                }
                break;
        }
        return false;
    }

    private bool progressBarActive = true;
    public void ToggleProgressBar(string activate)
    {
        progressBarActive = activate.Equals("true");
    }

    private void OnDisable()
    {
        SafeProgress();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SafeProgress();
        }
    }

    /**
    private void OnApplicationQuit()
    {
        SafeProgress();
    }
    */
    #endregion Safe
}