using UnityEngine;

// class to manage access to number sprites stored in NumberRepresentationSets and Audio
public class NumberRepresentationProvider : MonoBehaviour
{
    [Header("Visual representation")]
    [SerializeField] private NumberRepresentationSet[] representationSets;

    [Header("Audio representation")]
    [SerializeField] private OutputContainer[] numberAudio;
    [SerializeField] private OutputContainer plus;
    [SerializeField] private OutputContainer minus;

    private static OutputContainer[] audios;
    public static OutputContainer[] NumberAudio
    {
        get
        {
            if (audios == null)
            {
                OutputContainer[] numberAudio = FindObjectOfType<NumberRepresentationProvider>().numberAudio;
                if (numberAudio == null)
                {
                    return null;
                }
                audios = new OutputContainer[numberAudio.Length];
                System.Array.Copy(numberAudio, audios, numberAudio.Length);
            }
            return audios;
        }
    }

    private static OutputContainer plusAudio;
    public static OutputContainer PlusAudio
    {
        get
        {
            if (plusAudio == null)
            {
                plusAudio = FindObjectOfType<NumberRepresentationProvider>().plus;
            }
            return plusAudio;
        }
    }

    private static OutputContainer minusAudio;
    public static OutputContainer MinusAudio
    {
        get
        {
            if (minusAudio == null)
            {
                minusAudio = FindObjectOfType<NumberRepresentationProvider>().minus;
            }
            return minusAudio;
        }
    }

    private int representationIdx = 0;
    private int representationIdx2 = 0;
    private System.Random rand = new System.Random();

    public void ChooseRepresentationSet(NumberRepresentation representation)
    {
        Debug.Log("Choosing Representation");
        switch (representation)
        {
            case NumberRepresentation.RANDOM:
                representationIdx = rand.Next(0, representationSets.Length);
                break;
            case NumberRepresentation.HANDS:
                representationIdx = 0;
                break;
            case NumberRepresentation.LINES:
                representationIdx = 1;
                break;
            case NumberRepresentation.DICE:
                representationIdx = 2;
                break;
            case NumberRepresentation.MIXED:
                representationIdx2 = -1;
                break;
        }
    }

    public void ChooseRepresentationSet2(NumberRepresentation representation)
    {
        Debug.Log("Choosing Representation 2");
        switch (representation)
        {
            case NumberRepresentation.RANDOM:
                representationIdx2 = rand.Next(0, representationSets.Length);
                break;
            case NumberRepresentation.HANDS:
                representationIdx2 = 0;
                break;
            case NumberRepresentation.LINES:
                representationIdx2 = 1;
                break;
            case NumberRepresentation.DICE:
                representationIdx2 = 2;
                break;
            case NumberRepresentation.MIXED:
                representationIdx2 = -1;
                break;
        }
    }

    public Sprite GetNumber(int number, bool alternative)
    {
        if (representationIdx < 0 || representationIdx > representationSets.Length)
        {
            return representationSets[rand.Next(0, representationSets.Length)].GetSpriteForNumber(number, alternative);
        }
        return representationSets[representationIdx].GetSpriteForNumber(number, alternative);
    }

    public Sprite GetNumberSecondSet(int number, bool alternative)
    {
        if (representationIdx2 < 0 || representationIdx2 > representationSets.Length)
        {
            return representationSets[rand.Next(0, representationSets.Length)].GetSpriteForNumber(number, alternative);
        }
        return representationSets[representationIdx2].GetSpriteForNumber(number, alternative);
    }
}
