using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeExit : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            gameManagerSO.Victory();
        }
    }

}
