using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyProgressController : MonoBehaviour
{
    [SerializeField] StorySequence introductionSequence;

    // The last progressStep has to have only backgrounds and one final hub-miniGame to send Blub home
    [SerializeField] private LegacyProgressStep[] progressSteps;
    private int progressIdx;

    private StorySceneCanvasController canvasController;

    private void Start()
    {
        if(canvasController == null)
        {
            canvasController = FindObjectOfType<StorySceneCanvasController>();
        }
        progressIdx = 0; //PlayerPrefsController.GetProgressStepIdx();
        if (introductionSequence == null)
        {
            Debug.LogWarning("ProgressController.introductionSequence == null. Skipping startup");
            return;
        }
        if (progressIdx == -1)
        {
            canvasController.StartStorySequence(introductionSequence);
            progressIdx++;
            //PlayerPrefsController.SaveProgressStepIdx(progressIdx);
            //progressSteps[progressIdx].PrepareProgressStep();
            progressSteps[progressIdx].LoadProgressStep();
        }
        else
        {
            //progressSteps[progressIdx].PrepareProgressStep();
            progressSteps[progressIdx].LoadProgressStep();
        }
        canvasController.ChangeBackground(progressSteps[progressIdx].hubBackground);
        canvasController.ChangeRoom("hub");
    }

    private void Awake()
    {
        if(canvasController == null)
        {
            canvasController = FindObjectOfType<StorySceneCanvasController>();
        }
    }

    public void LoadRoom(LegacyProgressStep.Room room, GameObject buttonParent)
    {
        if (progressIdx < 0 || progressIdx >= progressSteps.Length)
        {
            return;
        }

        //Debug.Log("ProgressController: changeing Background");
        switch (room)
        {
            case LegacyProgressStep.Room.HUB:
                canvasController.ChangeBackground(progressSteps[progressIdx].hubBackground);
                break;
            case LegacyProgressStep.Room.COCKPIT:
                canvasController.ChangeBackground(progressSteps[progressIdx].cockpitBackground);
                break;
            case LegacyProgressStep.Room.ENGINE:
                canvasController.ChangeBackground(progressSteps[progressIdx].engineBackgorund);
                break;
            case LegacyProgressStep.Room.LAB:
                canvasController.ChangeBackground(progressSteps[progressIdx].labBackground);
                break;
        }

        progressSteps[progressIdx].GenerateMiniGameButtons(room, buttonParent.transform, canvasController);
    }

    public void GameCompleted(LegacyProgressStep.Room room)
    {
        if (progressIdx < 0 || progressIdx >= progressSteps.Length)
        {
            Debug.LogWarning("ProgressController.GameCompleted: progressIdx out of bounds");
            return;
        }
        if (progressSteps[progressIdx].GameCompleted(room))
        {
            switch (room)
            {
                case LegacyProgressStep.Room.HUB:
                    canvasController.StartStorySequence(progressSteps[progressIdx].completeStorySequence);
                    progressIdx++;
                    //PlayerPrefsController.SaveProgressStepIdx(progressIdx);
                    if (progressIdx >= progressSteps.Length)
                    {
                        // td: Story is over ~> Anzeige, Rückkehr ins Hauptmenü
                        Debug.Log("Story beendet.");
                    }
                    else
                    {
                        canvasController.ChangeBackground(progressSteps[progressIdx].hubBackground);
                    }
                    break;
                case LegacyProgressStep.Room.COCKPIT:
                    canvasController.StartStorySequence(progressSteps[progressIdx].cockpitCompleteStorySequence);
                    canvasController.ChangeBackground(progressSteps[progressIdx + 1].cockpitBackground);
                    break;
                case LegacyProgressStep.Room.ENGINE:
                    canvasController.StartStorySequence(progressSteps[progressIdx].engineCompleteStorySequence);
                    canvasController.ChangeBackground(progressSteps[progressIdx + 1].engineBackgorund);
                    break;
                case LegacyProgressStep.Room.LAB:
                    canvasController.StartStorySequence(progressSteps[progressIdx].labCompleteStorySequence);
                    canvasController.ChangeBackground(progressSteps[progressIdx + 1].labBackground);
                    break;
            }
        }
    }

    public void SafeProgress()
    {
        //PlayerPrefsController.SaveProgressStepIdx(progressIdx);
        if(progressIdx>=0&& progressIdx < progressSteps.Length)
        {
            progressSteps[progressIdx].SafeProgressStep();
        }
    }

    public void ExerciseGame()
    {
        // td: Loade FreePlay Scene with fitting game to exercise with
    }
}
