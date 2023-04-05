using UnityEngine;

// access script for PlayerPrefs
public class PlayerPrefsController : MonoBehaviour
{
    #region story_progress
    private const string ROOM_IDX = "room_idx";
    private const string GAME_IDX = "game_idx";
    private const string FREE_PLAY_UNLOCK = "free_play_unlock";
    private const string STORY_COMPLETED = "story_completed";
    private const string VIDEO_UNLOCK = "video_unlock";

    public static void SafeRoomIdx(Room room, int value)
    {
        PlayerPrefs.SetInt(ROOM_IDX + GetStringEnd(room), value);
    }

    public static int LoadRoomIdx(Room room)
    {
        return PlayerPrefs.GetInt(ROOM_IDX + GetStringEnd(room), 0);
    }

    public static void SafeGameIdx(Room room, int value)
    {
        //Debug.Log("-> ("+room+") "+value);
        PlayerPrefs.SetInt(GAME_IDX + GetStringEnd(room), value);
    }

    public static int LoadGameIdx(Room room)
    {
        //Debug.Log("<- ("+room+")"+PlayerPrefs.GetInt(GAME_IDX + GetStringEnd(room), 0));
        return PlayerPrefs.GetInt(GAME_IDX + GetStringEnd(room), 0);
    }

    public static void SafeFreePlayUnlock(bool unlock)
    {
        PlayerPrefs.SetInt(FREE_PLAY_UNLOCK, unlock ? 1 : 0);
    }

    public static bool IsFreePlayUnlocked()
    {
        return PlayerPrefs.GetInt(FREE_PLAY_UNLOCK, 0) == 1;
    }

    public static void ResetProgress()
    {
        Debug.Log("Resetting Progress");
        SafeRoomIdx(Room.HUB, 0);
        SafeRoomIdx(Room.LAB, 0);
        SafeRoomIdx(Room.NAV, 0);
        SafeRoomIdx(Room.ENGINE, 0);
        SafeGameIdx(Room.HUB, 0);
        SafeGameIdx(Room.LAB, 0);
        SafeGameIdx(Room.NAV, 0);
        SafeGameIdx(Room.ENGINE, 0);
        PlayerPrefs.Save();
    }

    private static string GetStringEnd(Room room)
    {
        switch (room)
        {
            case Room.HUB:
                return "_hub";
            case Room.NAV:
                return "_nav";
            case Room.LAB:
                return "_lab";
            case Room.ENGINE:
                return "_engine";
        }
        return "_unknown";
    }

    public static void CompleteStory()
    {
        //Debug.Log("Calling CompleteStory");
        PlayerPrefs.SetInt(STORY_COMPLETED, 1);
        if (!ShouldSkipVideo())
        {
            ToggleVideoSkip();
        }
    }

    public static bool WasStoryCompleted()
    {
        return PlayerPrefs.GetInt(STORY_COMPLETED, 0) == 1;
    }

    public static void UnlockVideo(string idx)
    {
        PlayerPrefs.SetInt(VIDEO_UNLOCK, int.Parse(idx));
    }

    public static int VideosUnlockedTo()
    {
        return PlayerPrefs.GetInt(VIDEO_UNLOCK, -1);
    }
    #endregion story_progress

    #region free_play
    // small -> easier
    private const string QUICK_PLAY_DIFFICULTY = "quickPlayDifficulty";
    private const string LAST_PLAYED_AUDIO = "lastPlayedAudio";

    public static int GetQuickPlayDifficulty()
    {
        return PlayerPrefs.GetInt(QUICK_PLAY_DIFFICULTY, 1);
    }

    public static void SetQuickPlayDifficulty(int value)
    {
        PlayerPrefs.SetInt(QUICK_PLAY_DIFFICULTY, value);
    }

    public static void VsAudioPlayed(MiniGameType gameType)
    {
        System.DateTime currentTime = System.DateTime.Now;
        PlayerPrefs.SetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_day", currentTime.Day);
        PlayerPrefs.SetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_month", currentTime.Month);
        PlayerPrefs.SetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_year", currentTime.Year);
    }

    public static bool ShouldVsAudioPlay(MiniGameType gameType)
    {
        System.DateTime currentTime = System.DateTime.Now;
        return currentTime.Year > PlayerPrefs.GetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_year", 0) 
            || currentTime.Month > PlayerPrefs.GetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_month", 0) 
            || currentTime.Day > PlayerPrefs.GetInt(LAST_PLAYED_AUDIO + "_" + gameType + "_day", -1);
    }

    #endregion free_play

    #region options
    private const string SHOW_INFORMATION = "showInformation";
    private const string NUMBER_AUDIO = "numberAudio";
    private const string NUMBER_AUDIO_OVERRIDE = "numberAudioOverride";
    private const string HELP_DIFFICULTY = "helpDifficulty";
    private const string VIDEO_SKIP = "videoSkip";

    public static bool ShowInformation()
    {
        return PlayerPrefs.GetInt(SHOW_INFORMATION, 0) == 1;
    }

    public static void SafeShowInformation(bool val)
    {
        PlayerPrefs.SetInt(SHOW_INFORMATION, val ? 1 : 0);
    }

    // buffer settings to not always read PlayerPrefs Data
    private static bool playNumberAudioBuffer = PlayerPrefs.GetInt(NUMBER_AUDIO, 1) == 1;

    public static bool PlayNumberAudio()
    {
        return playNumberAudioBuffer;
    }

    public static void SafePlayNumberAudio(bool val)
    {
        PlayerPrefs.SetInt(NUMBER_AUDIO, val ? 1 : 0);
        playNumberAudioBuffer = val;
    }
    public static int GetOverridePlayNumberAudio()
    {
        return PlayerPrefs.GetInt(NUMBER_AUDIO_OVERRIDE, 1);
    }

    public static void SafeNumberAudioOverride(int val)
    {
        PlayerPrefs.SetInt(NUMBER_AUDIO_OVERRIDE, val);
    }

    public static bool ShouldPlayAudio()
    {
        return GetOverridePlayNumberAudio() == 2 || GetOverridePlayNumberAudio() == 1 && PlayNumberAudio();
    }

    // larger -> more help -> easyier
    public static int GetHelpDifficulty()
    {
        return PlayerPrefs.GetInt(HELP_DIFFICULTY, 1);
    }

    public static void SetHelpDifficulty(int dif)
    {
        PlayerPrefs.SetInt(HELP_DIFFICULTY, dif);
    }

    public static void IncreaseHelp()
    {
        if (GetHelpDifficulty() < 2)
        {
            Debug.Log("Increasing help.");
            SetHelpDifficulty(GetHelpDifficulty() + 1);
        }
        SetQuickPlayDifficulty(Mathf.Max(GetQuickPlayDifficulty() - 1, 0));
    }

    public static void DecreaseHelp()
    {
        if (GetHelpDifficulty() > 0)
        {
            Debug.Log("Decreasing help.");
            SetHelpDifficulty(GetHelpDifficulty() - 1);
        }
        SetQuickPlayDifficulty(Mathf.Min(GetQuickPlayDifficulty() + 1, 2));
    }

    public static void ToggleVideoSkip()
    {
        if (ShouldSkipVideo())
        {
            PlayerPrefs.SetInt(VIDEO_SKIP, 0);
        }
        else
        {
            PlayerPrefs.SetInt(VIDEO_SKIP, 1);
        }
    }

    public static bool ShouldSkipVideo()
    {
        return PlayerPrefs.GetInt(VIDEO_SKIP, 0) == 1;
    }
    #endregion options

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.Save();
        }
    }
}
