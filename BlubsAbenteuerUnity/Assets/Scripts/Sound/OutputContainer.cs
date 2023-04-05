using UnityEngine;

// Class to safe pairs of text and audio
[CreateAssetMenu(fileName = "new Output Container", menuName = "OutputContainer")]
public class OutputContainer : ScriptableObject
{
    public string text;
    public AudioClip audio;
    // priority of audio: starting audio with heigher priority will stop lower priority audio from playing
    [Tooltip("Audio with higher priority will interrupt audio with lower priority. Use 0 as standard priority, 1 for MiniGame explanations and 2 for menu text.")]
    public int priority;
}
