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

    public void OnStartAttackAnimation()
    {
        attackState.StartAttack();
    }

    public void OnFinishAttackAnimation()
    {
        attackState.CheckTarget();
    }

    public void OnHitTargetAttackAnimation()
    {
        attackState.HitTarget();
    }

    public void OnFinishAlertAnimation()
    {
        alertState.ChaseTarget();
    }
}
