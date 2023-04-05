using UnityEngine;
using UnityEngine.UI;

// controller script for managing options made for quick play
public class QuickPlayOptionsController : MonoBehaviour
{
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Slider displayModeSlider;

    private MiniGameOptions options;

    private void Awake()
    {
        options = FreePlayOptionsSingleton.Instance.GameOptions;

        difficultySlider.onValueChanged.RemoveAllListeners();
        difficultySlider.value = PlayerPrefsController.GetQuickPlayDifficulty() + 1;
        difficultySlider.onValueChanged.AddListener(delegate { ChangeQuickPlayDifficulty(); });
        displayModeSlider.onValueChanged.RemoveAllListeners();
        options.displayMode = DisplayMode.MIXED;
        SetupDisplayModeSlider();
        displayModeSlider.onValueChanged.AddListener(delegate { ChangeDisplayMode(); });
    }

    public void ChangeQuickPlayDifficulty()
    {
        PlayerPrefsController.SetQuickPlayDifficulty((int)difficultySlider.value - 1);
    }

    private void SetupDisplayModeSlider()
    {
        switch (options.displayMode)
        {
            case DisplayMode.SET:
                displayModeSlider.value = 1;
                break;
            case DisplayMode.MIXED:
                displayModeSlider.value = 2;
                break;
            case DisplayMode.TEXT:
                displayModeSlider.value = 3;
                break;
        }
    }

    public void ChangeDisplayMode()
    {
        switch ((int)displayModeSlider.value)
        {
            case 1:
                options.displayMode = DisplayMode.SET;
                break;
            case 2:
                options.displayMode = DisplayMode.MIXED;
                break;
            case 3:
                options.displayMode = DisplayMode.TEXT;
                break;
            default:
                Debug.LogError(((int)displayModeSlider.value) + " is not a valid value for display mode selection.");
                break;
        }
        SetupDisplayModeSlider();
    }

    public void StartQuickPlay()
    {
        FreePlayOptionsSingleton.Instance.QuickPlay = true;
        FreePlayOptionsSingleton.GenerateRandomizedOptions();
        FreePlayOptionsSingleton.Instance.ParentMode = true;
        FindObjectOfType<SceneController>().LoadFreePlayScene();

    }

    public void StartQuickPlayChildMode()
    {
        PlayerPrefsController.SetQuickPlayDifficulty(PlayerPrefsController.LoadRoomIdx(Room.HUB) <= 11 && !PlayerPrefsController.WasStoryCompleted() ? 0 : PlayerPrefsController.LoadRoomIdx(Room.HUB) <= 13 && !PlayerPrefsController.WasStoryCompleted() ? 1 : 2);
        options.displayMode = DisplayMode.MIXED;
        StartQuickPlay();
        FreePlayOptionsSingleton.Instance.ParentMode = false;
    }
}
