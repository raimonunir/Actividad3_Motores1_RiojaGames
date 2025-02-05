using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColor : MonoBehaviour
{
    private Renderer myRenderer;
    private bool isColorA = true;
    [SerializeField][ColorUsageAttribute(true, true)] private Color colorA;
    [SerializeField][ColorUsageAttribute(true, true)] private Color colorB;
    [SerializeField] [Range(0.5f, 5f)] private float secondsBlinkFrecuency;

    // Start is called before the first frame update
    void Start()
    {
        // get renderer component and start coroutine for blinking
        myRenderer = GetComponent<Renderer>();
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        // couroutine in unity will stop automatically if the gameObject is disabled
        while (true)
        {
            if (isColorA)
            {
                myRenderer.material.SetColor("_EmissionColor", colorB);
                isColorA = false;
            }
            else
            {
                myRenderer.material.SetColor("_EmissionColor", colorA);
                isColorA = true;
            }
            yield return new WaitForSeconds(secondsBlinkFrecuency);
        }
    }
}
