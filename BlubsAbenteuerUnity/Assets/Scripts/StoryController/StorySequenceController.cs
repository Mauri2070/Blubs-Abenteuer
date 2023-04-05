using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// script to controll video sequences in story mode
public class StorySequenceController : MonoBehaviour
{
    public StorySequence storySequence;

    [Header("Controlling UI Buttons")]
    [SerializeField] Button nextClipButton;
    [SerializeField] Button previousClipButton;
    [SerializeField] Button repeatClipButton;

    [Header("Video Player UI Elements")]
    [SerializeField] VideoPlayer videoPlayer;

    [Header("Other UI Elements")]
    [SerializeField] GameObject WIPText;

    private StorySceneCanvasController canvasController;
    private ButtonPulser nextClipPulser;

    private VideoClip videoClip;
    private Coroutine coroutine;

    private void Awake()
    {
        canvasController = FindObjectOfType<StorySceneCanvasController>();
        nextClipPulser = nextClipButton.GetComponent<ButtonPulser>();
    }

    public void StartNewStorySequence(StorySequence sequence)
    {
        SoundControllerSingleton.CancelAudio();
        Debug.Log("Starting Story Sequence " + sequence + " (StorySequenceController)");
        storySequence = sequence;
        storySequence.Reset();

        WIPText.SetActive(storySequence.WorkInProgress);

        nextClipButton.interactable = false;
        previousClipButton.interactable = false;
        repeatClipButton.interactable = false;

        nextClipButton.onClick.RemoveAllListeners();
        previousClipButton.onClick.RemoveAllListeners();
        repeatClipButton.onClick.RemoveAllListeners();

        nextClipButton.onClick.AddListener(this.NextVideoClip);
        previousClipButton.onClick.AddListener(this.PreviousVideoClip);
        repeatClipButton.onClick.AddListener(this.StartVideoClip);

        videoPlayer.isLooping = false;

        Debug.Log("Story Sequence setup completed.");

        videoPlayer.targetTexture.Release();
        NextVideoClip();
    }

    public void NextVideoClip()
    {
        if (storySequence.GetNextVideoClipPossible(out videoClip))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            StartVideoClip();
        }
        else
        {
            Debug.Log("Story Sequence " + storySequence.name + " completed!");
            activateDoor = false;
            if (door != null)
            {
                door.SetActive(false);
            }
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            if (swapToMainMenu)
            {
                // do not optimize!!!!
                // first check swapToMainMenu, then advance StorySteps! (otherwise, MainMenu will load one clip early)
                canvasController.EndStorySequence();
                swapToMainMenu = false;
                FindObjectOfType<SceneController>().LoadMainMenuFreePlayOpened();
            }
            else
            {
                canvasController.EndStorySequence();
            }
        }
    }

    public void PreviousVideoClip()
    {
        StopCoroutine(coroutine);
        if (storySequence.GetPreviousVideoClipPossible(out videoClip))
        {
            StartVideoClip();
        }
        if (activateDoor)
        {
            door.SetActive(false);
        }
    }

    private GameObject door;
    private bool activateDoor;
    public void ActivateDoor(GameObject door)
    {
        Debug.Log("ActivateDoor: " + door.name);
        activateDoor = true;
        this.door = door;
    }

    public void DeactivateDoor()
    {
        Debug.Log("Deactivating door");
        activateDoor = false;
        //door = null;
    }

    private bool swapToMainMenu = false;
    public void ActivateMainMenuSwap()
    {
        swapToMainMenu = true;
    }

    public void DeactivateMainMenuSwap()
    {
        // Bugfix: return to free play after story reset still active
        swapToMainMenu = false;
    }

    private void StartVideoClip()
    {
        videoPlayer.Stop();
        videoPlayer.clip = videoClip;
        videoPlayer.Prepare();
        Debug.Log("Starting video clip");
        coroutine = StartCoroutine(ClipStartingAndButtonActivatorCoroutine());
    }

    private IEnumerator ClipStartingAndButtonActivatorCoroutine()
    {
        nextClipPulser.Pulse = false;
        // Wait until player is ready
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();

        nextClipButton.interactable = storySequence.ClipPlayed() || PlayerPrefsController.ShouldSkipVideo();
        if (!storySequence.HasNextClip() && activateDoor && nextClipButton.interactable)
        {
            Debug.Log("activating door " + door.name);
            door.SetActive(true);
        }
        repeatClipButton.interactable = false;
        previousClipButton.interactable = storySequence.HasPreviousClip();
        // wait until clip is finished (earliest possible)
        yield return new WaitForSeconds((float)videoClip.length);
        Debug.Log("Video clip length reached.");
        // wait until clip is finished due to loading delay (shouldn't be necessary after waiting for preparation)
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        Debug.Log("Video clip completed playing.");
        nextClipPulser.Pulse = true;
        repeatClipButton.interactable = true;
        nextClipButton.interactable = true;
        storySequence.PlayingCurrentClip();
        if (!storySequence.HasNextClip() && activateDoor)
        {
            Debug.Log("activating door " + door.name);
            door.SetActive(true);
        }
    }
}
