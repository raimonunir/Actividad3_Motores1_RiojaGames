using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorHandler : MonoBehaviour
{
    AlertState alertState;
    AttackState attackState;

    private void Start()
    {
        alertState = transform.parent.GetComponent<AlertState>();
        attackState = transform.parent.GetComponent<AttackState>();
    }

    private void OnFinishAttackAnimation()
    {
        attackState.CheckTarget();
    }

    private void OnFinishAlertAnimation()
    {
        alertState.ChaseTarget();
    }
}
