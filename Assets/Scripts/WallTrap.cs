using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WallTrap : MonoBehaviour
{
    
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private GameObject wallLeft;
    [SerializeField] private GameObject wallRight;
    [SerializeField][Range(1000f, 100000f)] private float wallTrapForce;
    [SerializeField] private AudioSource audioSourceWallLeft;
    [SerializeField] private AudioSource audioSourceWallRight;

    private WallTrapWall wallTrapWallLeft;
    private WallTrapWall wallTrapWallRight;
    private Rigidbody wallLeftRigidBody;
    private Rigidbody wallRightRigidBody;
    private bool wallTrapActivated = false;
    private float initialZpositionWallLeft;
    private float initialZpositionWallRight;
    private Vector3 lastPositionWallLeft;
    private Vector3 lastPositionWallRight;

    private void Start()
    {
        wallTrapWallLeft = wallLeft.GetComponent<WallTrapWall>();
        wallTrapWallRight = wallRight.GetComponent<WallTrapWall>();
        wallLeftRigidBody = wallLeft.GetComponent<Rigidbody>();
        wallRightRigidBody = wallRight.GetComponent<Rigidbody>();
        initialZpositionWallLeft = wallLeftRigidBody.position.z;
        initialZpositionWallRight = wallRightRigidBody.position.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            wallTrapActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            wallTrapActivated = false;
        }
    }


    private void FixedUpdate()
    {
        if (!gameManagerSO.isAlive) {
            return;
        }

        //XXX if player is death this won't be executed XXX

        if (wallTrapActivated)
        {
            wallRightRigidBody.AddForce(wallRight.transform.forward * wallTrapForce, ForceMode.Force);
            wallLeftRigidBody.AddForce(wallLeft.transform.forward * -wallTrapForce, ForceMode.Force);
            if (wallTrapWallLeft.isTouchingPlayer && wallTrapWallRight.isTouchingPlayer)
            {
                gameManagerSO.Death();
            }
        }
        else
        {
            if(wallRightRigidBody.position.z > initialZpositionWallRight)
            {
                wallRightRigidBody.AddForce(wallRight.transform.forward * -wallTrapForce, ForceMode.Force);
            }            
            if(wallLeftRigidBody.position.z < initialZpositionWallLeft)
            {
                wallLeftRigidBody.AddForce(wallLeft.transform.forward * wallTrapForce, ForceMode.Force);
            }
        }

        // sound wall left
        if (Vector3.Distance(lastPositionWallLeft, wallLeftRigidBody.position) > 0.01f && wallLeftRigidBody.GetAccumulatedForce().magnitude > 0.1f)
        {
            if (!audioSourceWallLeft.isPlaying)
            {
                audioSourceWallLeft.Play();
            }
        }
        else
        {
            audioSourceWallLeft.Stop();
        }

        // sound wall right
        if(Vector3.Distance(lastPositionWallRight, wallRightRigidBody.position) > 0.01f && wallRightRigidBody.GetAccumulatedForce().magnitude > 0.1f)
        {
            if (!audioSourceWallRight.isPlaying)
            {
                audioSourceWallRight.Play();
            }
        }
        else
        {
            audioSourceWallRight.Stop();
        }

        //XXX rigidbody.velocity is not working, I need to save lasts positions
        lastPositionWallLeft = wallLeftRigidBody.position;
        lastPositionWallRight = wallRightRigidBody.position;
    }

}
