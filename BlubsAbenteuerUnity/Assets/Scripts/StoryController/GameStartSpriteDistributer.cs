using UnityEngine;

// container class for game start button sprites
public class GameStartSpriteDistributer : MonoBehaviour
{
    public static GameStartSpriteDistributer instance;

    [SerializeField] Sprite[] navSprites;
    [SerializeField] Sprite[] labSprites;
    [SerializeField] Sprite[] engineSprites;

    public Sprite GetSprite(Room room)
    {
        switch (room)
        {
            case Room.HUB:
                return null;
            case Room.NAV:
                return navSprites[Random.Range(0, navSprites.Length)];
            case Room.LAB:
                return labSprites[Random.Range(0, labSprites.Length)];
            case Room.ENGINE:
                return engineSprites[Random.Range(0, engineSprites.Length)];
        }
        return null;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
