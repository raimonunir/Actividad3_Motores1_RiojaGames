using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private int idDoor;
    [SerializeField] private float speed;
    [SerializeField][Range(0.5f, 5f)] private float timeOpen;
    [SerializeField] private BoxCollider doorCollider;


    private bool isClosed = true;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        gameManagerSO.OnSwitchActivated += OpenDoorEventDetected;
    }

    private void OnDisable()
    {
        gameManagerSO.OnSwitchActivated -= OpenDoorEventDetected;
    }

    private void OpenDoorEventDetected(int idSwitch)
    {
        if(isClosed && idDoor == idSwitch)
        {
            StartCoroutine(OpenCloseDoorMechanic());
        }
    }

    private IEnumerator OpenCloseDoorMechanic()
    {
        float maxOpenYPosition = transform.position.y + 0.1f - doorCollider.bounds.size.y + 0.5f;
        float closePosition = transform.position.y;

        isClosed = false;

        // wait 1 second
        yield return new WaitForSeconds(1f);

        // play sound
        audioSource.Play();

        // open the door
        while (true)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.down);
            yield return null;
            if (transform.position.y <= maxOpenYPosition)
                break;
        }
        // stop sound
        audioSource.Pause();

        // waits for x seconds
        yield return new WaitForSeconds(timeOpen);

        // play sound
        audioSource.UnPause();

        // close the door
        while (true)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
            yield return null;
            if (transform.position.y >= closePosition)
                break;
        }
        isClosed = true;

        // stop audio
        audioSource.Stop();
    } 
}
