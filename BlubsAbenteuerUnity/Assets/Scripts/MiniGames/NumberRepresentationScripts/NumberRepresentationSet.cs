using UnityEngine;

// Scriptable Object representing a set of visual number representations
[CreateAssetMenu(fileName = "new Number Representation Set", menuName = "NumberRepresentationSet")]
public class NumberRepresentationSet : ScriptableObject
{
    [SerializeField] private Sprite[] numberSprites;
    [SerializeField] private Sprite alternative5;
    [SerializeField] private Sprite alternative10;
    [SerializeField] private Sprite alternative20;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite nullSprite;

    public Sprite GetSpriteForNumber(int number, bool alternative)
    {
        if (alternative)
        {
            if (number == 5 && alternative5 != null)
            {
                return alternative5;
            }
            else if (number == 10 && alternative10 != null)
            {
                return alternative10;
            }
            else if (number == 20 && alternative20 != null)
            {
                return alternative20;
            }
        }
        if(number == 0)
        {
            return nullSprite;
        }

        if (number < 0 || number > numberSprites.Length)
        {
            Debug.LogWarning("NumberRepresentationSet has no Sprite for " + number);
            return defaultSprite;
        }
        return numberSprites[number - 1];
    }
}
