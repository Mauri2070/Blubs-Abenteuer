using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// video sequence controller script for main menu
public class MainMenuVideoController : MonoBehaviour
{
    [Header("Menu Button")]
    [SerializeField] Button openVideoMenuButton;

    [Header("Canvas Elements")]
    [SerializeField] GameObject videoSelection;
    [SerializeField] GameObject videoDisplay;
    [SerializeField] Button[] videoSelectionButtons;

    [Header("Video Player")]
    [SerializeField] VideoPlayer videoPlayer;

    [Header("Videos")]
    [SerializeField] StorySequence[] videos;

    private StorySequence storySequence;
    private VideoClip videoClip;

    private void Awake()
    {
        videoSelection.SetActive(false);
        videoDisplay.SetActive(false);

        for (int i = videos.Length - 1; i > PlayerPrefsController.VideosUnlockedTo() && i >= 0; i--)
        {
            videoSelectionButtons[i].interactable = false;
        }

        if (PlayerPrefsController.VideosUnlockedTo() == -1)
        {
            openVideoMenuButton.interactable = false;
        }
        else
        {
            openVideoMenuButton.interactable = true;
        }
    }

    public void StartStorySequence(int idx)
    {
        if (idx < 0 || idx >= videos.Length)
        {
            Debug.LogWarning(idx + " is out of bounds for videos.");
            return;
        }
        storySequence = videos[idx];
        storySequence.Reset();

        videoPlayer.isLooping = false;

        videoSelection.SetActive(false);
        videoDisplay.SetActive(true);

        NextVideoClip();
    }

    public void NextVideoClip()
    {
        if (storySequence.GetNextVideoClipPossible(out videoClip))
        {
            StartVideoClip();
        }
        else
        {
            videoSelection.SetActive(true);
            videoDisplay.SetActive(false);
        }
    }

    public void PreviousVideoClip()
    {
        if (storySequence.GetPreviousVideoClipPossible(out videoClip))
        {
            StartVideoClip();
        }
        else
        {
            videoSelection.SetActive(true);
            videoDisplay.SetActive(false);
        }
    }

    public void StartVideoClip()
    {
        videoPlayer.Stop();
        videoPlayer.clip = videoClip;
        videoPlayer.Prepare();
        videoPlayer.Play();
    }
}
