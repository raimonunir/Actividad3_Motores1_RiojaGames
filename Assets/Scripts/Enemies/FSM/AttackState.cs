using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState<EnemyController>
{
    private float targetWidth;
    private bool isAttacking;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Agent.stoppingDistance = (controller.BodyLength / 2);
        isAttacking = true;
        controller.Animator.SetBool("EN01Attacking", true);
        targetWidth = controller.Target.GetComponent<Renderer>().bounds.size.x;
    }

    public override void OnUpdateState()
    {
        faceTarget();
    }

    public override void OnExitState() {}

    public void faceTarget()
    {
        Vector3 directionToTarget = (controller.Target.position - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
    }

    public void CheckTarget() 
    {
        if (controller.TargetIsDead)
        {
            isAttacking = false;
            controller.Animator.SetBool("EN01Attacking", false);
            controller.ChangeState(controller.TargetDestroyedState);
        }
        else 
        {
            if (Vector3.Distance(transform.position, controller.Target.transform.position) > controller.Agent.stoppingDistance + (targetWidth / 2))
            {
                isAttacking = false;
                controller.Animator.SetBool("EN01Attacking", false);
                controller.ChangeState(controller.ChaseState);
            }
            else
            {
                isAttacking = true;
            }
        }
    }

    public void HitTarget()
    {
        if (Vector3.Distance(transform.position, controller.Target.transform.position) <= (controller.Agent.stoppingDistance + (targetWidth/2) + 0.5))
        {
            controller.GameManagerSO.Damage(GameManagerSO.DamageType.sabre);
        }
    }

    internal void StartAttack()
    {
        if (isAttacking) controller.FuenteSonido.PlayOneShot(controller.tigerBite);
    }
}