using UnityEngine;
using UnityEngine.UI;
using TMPro;

// script for realizing text+audio outputs
public class OutputDistributer : MonoBehaviour
{
    public OutputContainer output;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button repeatButton;
    [SerializeField] private Button alternativeRepeatButton;

    [SerializeField] private bool playOnEnable;
    public bool PlayOnEnable
    {
        get { return playOnEnable; }
        set { playOnEnable = value; }
    }

    private void OnEnable()
    {
        Distribute();
    }

    public void PlayAudio()
    {
        PlayAudio(false);
    }

    public void PlayAudio(bool skip)
    {
        if (skip)
        {
            return;
        }
        SoundControllerSingleton.PlayAudio(output);
    }

    public void Distribute()
    {
        if (output == null)
        {
            Debug.LogWarning("OutputDistributer \"" + gameObject.name + "\" trying to distribute null!");
            return;
        }

        if (text != null)
        {
            text.text = output.text;
        }

        repeatButton.onClick.RemoveAllListeners();
        repeatButton.onClick.AddListener(PlayAudio);
        if (alternativeRepeatButton != null)
        {
            alternativeRepeatButton.onClick.RemoveAllListeners();
            alternativeRepeatButton.onClick.AddListener(PlayAudio);
        }


        if (playOnEnable)
        {
            PlayAudio();
        }
    }
}
