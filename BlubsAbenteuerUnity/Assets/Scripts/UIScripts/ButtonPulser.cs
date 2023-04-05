using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPulser : MonoBehaviour
{
    private float currentTime;

    private bool pulse;
    public bool Pulse
    {
        get { return pulse; }
        set
        {
            if (!value)
            {
                transform.localScale = Vector3.one;
                currentTime = 0;
            }
            pulse = value;
        }
    }

    [SerializeField] [Range(0.5f, 5f)] private float maxScale;
    [SerializeField] [Range(0.1f, 10f)] private float pulseTime;

    // Start is called before the first frame update
    void Start()
    {
        pulse = false;
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (pulse)
        {
            currentTime += Time.deltaTime;
            currentTime %= pulseTime;
            float newScale = 1 + (currentTime > pulseTime / 2 ? (maxScale - 1) * ((pulseTime - currentTime) / (pulseTime / 2)) : (maxScale - 1) * (currentTime / (pulseTime / 2)));
            transform.localScale = new Vector3(newScale, newScale);
        }
    }
}
