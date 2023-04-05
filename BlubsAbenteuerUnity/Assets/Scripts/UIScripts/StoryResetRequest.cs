using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryResetRequest : MonoBehaviour
{
    [SerializeField] private OutputDistributer questionDistributer;
    [SerializeField] private GameObject requestMenu;
    [SerializeField] private GameObject menuOpenButton;

    private int count;
    private const int timesToConfirm = 2;

    private void Awake()
    {
        requestMenu.SetActive(false);
    }

    public void OpenRequestMenu()
    {
        requestMenu.SetActive(true);
        count = 0;
        menuOpenButton.SetActive(false);
    }

    public void Confirm()
    {
        count++;
        if (count >= timesToConfirm)
        {
            PlayerPrefsController.ResetProgress();
            FindObjectOfType<ProgressController>().ResetFromStoryMode();
            FindObjectOfType<StorySequenceController>().DeactivateMainMenuSwap();
            menuOpenButton.SetActive(false);
            requestMenu.SetActive(false);
        }
        else
        {
            questionDistributer.PlayAudio();
        }
    }

    public void Cancel()
    {
        count = 0;
        requestMenu.SetActive(false);
        menuOpenButton.SetActive(true);
    }
}
