using UnityEngine;
using UnityEngine.Audio;

// script to manage most audios in the game
public class SoundControllerSingleton : MonoBehaviour
{
    // Singleton base code from: http://www.unitygeek.com/unity_c_singleton/
    private static SoundControllerSingleton instance = null;
    public static SoundControllerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundControllerSingleton>();
                if (instance == null)
                {
                    GameObject go = new GameObject("Sound Controller");
                    instance = go.AddComponent<SoundControllerSingleton>();
                    if (instance.audioSource == null)
                    {
                        instance.audioSource = go.AddComponent<AudioSource>();
                    }
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private AudioMixer menuMixerGroup;
    private AudioMixer numberMixerGroup;

    private AudioSource audioSource;
    private int currentPrio = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            menuMixerGroup = Resources.Load("AudioMixer/MenuAudioMixer") as AudioMixer;
            numberMixerGroup = Resources.Load("AudioMixer/NumberAudioMixer") as AudioMixer;
            if (instance.audioSource == null)
            {
                instance.audioSource = instance.gameObject.GetComponent<AudioSource>();
                if (instance.audioSource == null)
                {
                    instance.audioSource = instance.gameObject.AddComponent<AudioSource>();
                }
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void CancelAudio()
    {
        Instance.audioSource.Stop();
    }

    public static void PlayAudio(OutputContainer toPlay)
    {
        Instance.PlayAudioI(toPlay);
    }

    private void PlayAudioI(OutputContainer toPlay)
    {
        if (audioSource.isPlaying)
        {
            if (currentPrio > toPlay.priority)
            {
                Debug.Log("Already playing audio with higher priority");
                return;
            }
            else
            {
                Debug.Log("Playing new audio with higher or same priority");
                audioSource.Stop();
            }
        }

        if (toPlay.audio == null)
        {
            Debug.LogWarning("The used OuputContainer \"" + toPlay.name + "\" has no AudioClip!");
            return;
        }

        if (toPlay.priority == 0)
        {
            audioSource.outputAudioMixerGroup = numberMixerGroup.FindMatchingGroups("Master")[0];
        }
        else
        {
            audioSource.outputAudioMixerGroup = menuMixerGroup.FindMatchingGroups("Master")[0];
        }

        currentPrio = toPlay.priority;
        audioSource.clip = toPlay.audio;
        audioSource.Play();
    }
}
