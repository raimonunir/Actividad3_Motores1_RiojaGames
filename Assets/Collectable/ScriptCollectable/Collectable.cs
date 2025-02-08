using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectablePoints : MonoBehaviour
{
    public int collectablePoints;
    public TextMeshProUGUI textCollectable;

    private void Update()
    {
        textCollectable.text = "" + collectablePoints.ToString();
    }

}
