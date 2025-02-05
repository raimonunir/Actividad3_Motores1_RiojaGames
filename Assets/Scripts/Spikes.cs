using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Spikes : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;

    private void Start()
    {
        if (gameManagerSO == null) { Debug.LogError("NO gameManagerReference, click to get gameObjec", gameObject); }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            gameManagerSO.Damage(GameManagerSO.DamageType.spike);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManagerSO.Damage(GameManagerSO.DamageType.spike);
        }
    }
}
