using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTtrigger : MonoBehaviour
{
    private TNTtrap tNTtrap;

    private void Start()
    {
        tNTtrap = GetComponentInParent<TNTtrap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            tNTtrap.ActivateWick();
            Destroy(gameObject);
        }
    }
}
