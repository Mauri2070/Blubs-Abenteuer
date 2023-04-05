using UnityEngine;
using UnityEngine.SceneManagement;

// script responsible for scene loading
public class SceneController : MonoBehaviour
{
    [SerializeField] private ProgressController progressController;

    private static string lastScene = "";

    private void Start()
    {
        //Debug.Log("SceneController.Start: lastScene = " + lastScene);
        if (lastScene.Equals("FreePlay"))
        {
            MainMenuController mmc = FindObjectOfType<MainMenuController>();
            if (mmc == null)
            {
                Debug.LogError("Could not find MainMenuController");
                return;
            }
            if (FreePlayOptionsSingleton.Instance.QuickPlay)
            {
                if (FreePlayOptionsSingleton.Instance.ParentMode)
                {
                    mmc.startMenuChild.SetActive(false);
                    mmc.quickPlayButton.onClick.Invoke();
                }
            }
            else
            {
                if (FreePlayOptionsSingleton.Instance.ParentMode)
                {
                    mmc.startMenuChild.SetActive(false);
                    mmc.freePlayButton.onClick.Invoke();
                }
                else
                {
                    mmc.freePlayButtonChildren.onClick.Invoke();
                }
            }
        }
    }

    // Main Menu to Story Scene
    public void LoadStoryScene()
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("StoryScene");
    }

    // Game Scene to Main-Menu
    public void LoadMainMenu()
    {
        lastScene = SceneManager.GetActiveScene().name;
        if (SceneManager.GetActiveScene().name.Equals("StoryScene"))
        {
            progressController.SafeProgress();
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene("MainMenu");
        SoundControllerSingleton.CancelAudio();
    }

    public void LoadMainMenuFreePlayOpened()
    {
        Debug.Log("Loading Main Menu with Free Play opened");
        lastScene = "FreePlay";
        FreePlayOptionsSingleton.Instance.QuickPlay = false;
        FreePlayOptionsSingleton.Instance.ParentMode = false;
        SceneManager.LoadScene("MainMenu");
        SoundControllerSingleton.CancelAudio();
    }

    // Main Menu to FreePlay Scene
    public void LoadFreePlayScene()
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("FreePlay");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
