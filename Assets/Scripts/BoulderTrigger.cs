using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrigger : MonoBehaviour
{
    [SerializeField] private GameManagerSO m_gameManager;
    [SerializeField] private Boulder boulder;
    [Header("CameraShakeEffect")]
    [SerializeField] [Range(0f, 1f)] private float shakeAmount;
    [SerializeField] [Range(0f, 0.1f)] private float shakeDecreaseFactor;
    [SerializeField] [Range(0f, 5f)] private float shakeDuration;

    private bool hasBeenActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenActivated)
        {
            m_gameManager.Shake(shakeAmount, shakeDecreaseFactor, shakeDuration);
            boulder.Activate();
            hasBeenActivated = true;
        }
    }
}
