using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    private Light mlight;

    [SerializeField][Range(0f, 1f)] private float minLightIntensity;
    [SerializeField][Range(0.5f, 3f)] private float maxLightIntensity;
    [SerializeField][Range(0.01f, 0.05f)] private float minFlickerIntensity;
    [SerializeField][Range(0.05f, 0.1f)] private float maxFlickerIntensity;


    // Start is called before the first frame update
    void Start()
    {
        mlight = GetComponent<Light>();
        StartCoroutine(changeIntensity());
    }

    private IEnumerator changeIntensity()
    {
        yield return new WaitForSeconds(Random.Range(minFlickerIntensity, maxFlickerIntensity));
        mlight.intensity = Random.Range(minLightIntensity, maxLightIntensity);
        StartCoroutine(changeIntensity());
    }


}
