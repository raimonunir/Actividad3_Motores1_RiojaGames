using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private float rotationXwhenDark;
    [SerializeField] private float lightIntensityWhenDark;
    [SerializeField] private float lightKelvinsWhenDark;
    [SerializeField][Range(0.1f, 1f)] private float updateFrequency;

    private float rotationInterval;
    private float lightIntensityInterval;
    private float lightKelvinsInterval;
    private Light myLight;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManagerSO == null) { Debug.LogError("You need a gameManager"); }
        myLight = GetComponent<Light>();
        rotationInterval = Mathf.Abs((transform.rotation.eulerAngles.x - rotationXwhenDark)) / (gameManagerSO.TimerToDark / updateFrequency);
        if (transform.rotation.eulerAngles.x > rotationXwhenDark) rotationInterval *= -1f;

        lightIntensityInterval = Mathf.Abs((myLight.intensity - lightIntensityWhenDark)) / (gameManagerSO.TimerToDark / updateFrequency);
        lightKelvinsInterval = Mathf.Abs((myLight.colorTemperature - lightKelvinsWhenDark)) / (gameManagerSO.TimerToDark / updateFrequency);


        StartCoroutine(RotateSun());
    }

    private IEnumerator RotateSun()
    {
        bool hasPassOneRound = false;
        float rotationDarkPositive = rotationXwhenDark+360f;
        float lastRotationX = 360f;

        while (true) {
            yield return new WaitForSeconds(updateFrequency);
            transform.Rotate(Vector3.right, rotationInterval);
            if (!hasPassOneRound)
            {
                hasPassOneRound = lastRotationX < transform.rotation.eulerAngles.x;
            }
            lastRotationX = transform.rotation.eulerAngles.x;
            //Debug.Log("hasPassOneRound="+ hasPassOneRound+" lr =" + lastRotationX + " transform.rotation.eulerAngles.x=" + transform.rotation.eulerAngles.x + " rotationDarkPositive=" + rotationDarkPositive);
            
            myLight.intensity -= lightIntensityInterval;
            myLight.colorTemperature -= lightKelvinsInterval;

            if(hasPassOneRound && transform.rotation.eulerAngles.x <= rotationDarkPositive)
            {
                break;
            }

        }
        // turnoff light, good night!
        gameObject.SetActive(false);
    }
}
