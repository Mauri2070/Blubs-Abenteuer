using UnityEngine;
using UnityEngine.UI;

// legace script to manage number audio toggle during mini games including sprite changes (not longer needed after moving number audio to help system)
public class NumberAudioToggle : MonoBehaviour
{
    private Button button;
    private bool playAudio;

    [SerializeField] private Sprite activeAudio;
    [SerializeField] private Sprite inactiveAudio;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
        playAudio = PlayerPrefsController.PlayNumberAudio();
        button.image.sprite = playAudio ? activeAudio : inactiveAudio;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ToggleAudio);
    }

    private void ToggleAudio()
    {
        if (playAudio)
        {
            PlayerPrefsController.SafePlayNumberAudio(false);
            button.image.sprite = inactiveAudio;
        }
        else
        {
            PlayerPrefsController.SafePlayNumberAudio(true);
            button.image.sprite = activeAudio;
        }
        playAudio = !playAudio;
    }
}
