using UnityEngine;
using UnityEngine.Video;

// container class to store story videos in sequences
[CreateAssetMenu(fileName = "new Story Sequence", menuName = "StorySequence")]
public class StorySequence : ScriptableObject
{
    private int currentClip = -1;
    [SerializeField] private VideoClip[] videoClips;
    private bool[] alreadyPlayed;

    [SerializeField] private bool workInProgress = false;
    public bool WorkInProgress
    {
        get { return workInProgress; }
    }

    private void Awake()
    {
        alreadyPlayed = new bool[videoClips.Length];
    }

    public bool HasNextClip()
    {
        return currentClip < videoClips.Length - 1;
    }

    public bool HasPreviousClip()
    {
        return currentClip >= 1;
    }

    public VideoClip GetCurrentVideoClip()
    {
        return videoClips[currentClip];
    }

    public VideoClip GetFirstVideoClip()
    {
        return videoClips[0];
    }

    public bool GetNextVideoClipPossible(out VideoClip clip)
    {
        if (HasNextClip())
        {
            currentClip++;
            clip = GetCurrentVideoClip();
            return true;
        }
        else
        {
            clip = GetCurrentVideoClip();
            return false;
        }
    }

    public bool GetPreviousVideoClipPossible(out VideoClip clip)
    {
        if (HasPreviousClip())
        {
            currentClip--;
            clip = GetCurrentVideoClip();
            return true;
        }
        else
        {
            clip = GetCurrentVideoClip();
            return false;
        }
    }

    public void PlayingCurrentClip()
    {
        if (currentClip < 0 || currentClip > videoClips.Length - 1)
        {
            return;
        }
        alreadyPlayed[currentClip] = true;
    }

    public bool ClipPlayed()
    {
        if (currentClip < 0 || currentClip > videoClips.Length - 1)
        {
            return false;
        }
        return alreadyPlayed[currentClip];
    }

    public void Reset()
    {
        currentClip = -1;
        alreadyPlayed = new bool[videoClips.Length];
    }
}
