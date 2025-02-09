using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab_Totem : MonoBehaviour
{

    public GameObject PointsObj;
    public int remainingCollectables;

    private void OnTriggerEnter(Collider other)
    
    {
         if(other. tag == "Player")
        {
            PointsObj.GetComponent<CollectablePoints>().collectablePoints += 1;
            Destroy(gameObject);
        }
    }
}
