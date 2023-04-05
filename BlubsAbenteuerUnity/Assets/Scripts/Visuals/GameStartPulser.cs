using UnityEngine;
using UnityEngine.UI;

// visual animation script for game start buttons in story mode
public class GameStartPulser : MonoBehaviour
{
    [SerializeField] float minAlpha;
    [SerializeField] float maxAlpha;
    [SerializeField] [Range(0, 1)] float pulseSpeed;

    bool increasing = true;
    Image image;
    Color baseColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        baseColor = image.color;
        image.color = new Color(baseColor.r, baseColor.g, baseColor.b, Random.Range(minAlpha, maxAlpha));
        if (image.color.a > (maxAlpha - minAlpha) / 2)
        {
            increasing = false;
        }
        else
        {
            increasing = true;
        }
    }

    private void Update()
    {
        Color newColor;
        if (increasing)
        {
            newColor = new Color(baseColor.r, baseColor.g, baseColor.b, image.color.a + Time.deltaTime * pulseSpeed);
            if (newColor.a >= maxAlpha)
            {
                increasing = false;
            }
        }
        else
        {
            newColor = new Color(baseColor.r, baseColor.g, baseColor.b, image.color.a - Time.deltaTime * pulseSpeed);
            if (newColor.a <= minAlpha)
            {
                increasing = true;
            }
        }
        image.color = newColor;
    }
}
