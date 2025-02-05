using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] GameManagerSO gameManagerSO;


    private void OnEnable()
    {
        gameManagerSO.OnShake += GameManagerSO_OnShake;
    }

    private void OnDisable()
    {
        gameManagerSO.OnShake -= GameManagerSO_OnShake;
    }

    private void GameManagerSO_OnShake(float shakeAmount=0.7f, float shakeDecreaseFactor=0.01f, float shakeDuration=1.5f)
    {
        StartCoroutine(Shake(shakeAmount, shakeDecreaseFactor, shakeDuration));
    }

    private IEnumerator Shake(float shakeAmount=0.7f, float shakeDecreaseFactor=0.01f, float shakeDuration=1.5f)
    {
        Vector3 originalPos = transform.position;
        float timeDecreaseFactor = 1f;

        while (shakeDuration > 0)
        {

            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * timeDecreaseFactor;
            if(shakeAmount > 0f)
            {
                shakeAmount -= shakeDecreaseFactor * Time.deltaTime;
            }
            
            yield return null;
        }

        transform.position = originalPos;
    }
}
