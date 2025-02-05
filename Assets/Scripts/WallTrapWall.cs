using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrapWall : MonoBehaviour
{
    private bool m_isTouchingPlayer = false;

    public bool isTouchingPlayer {  get => m_isTouchingPlayer; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_isTouchingPlayer = false;
        }
    }

}
