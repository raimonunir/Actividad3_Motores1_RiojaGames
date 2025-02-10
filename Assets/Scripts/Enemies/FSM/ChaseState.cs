using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState<EnemyController>
{
    private const float CHASE_SPEED = 12;
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Animator.SetBool("EN01Running", true);
        controller.Agent.speed = CHASE_SPEED;
        controller.Agent.stoppingDistance = controller.BodyLength / 2;
    }

    public override void OnUpdateState()
    {
        //if (!controller.Agent.pathPending && controller.Agent.CalculatePath(controller.Target.position, new NavMeshPath()))
        if (!controller.Agent.pathPending)
        {
            controller.Agent.SetDestination(controller.Target.position);

            if (!controller.Agent.pathPending && controller.Agent.remainingDistance < controller.Agent.stoppingDistance)
            {
                controller.Animator.SetBool("EN01Running", false);
                controller.ChangeState(controller.AttackState);
            }
        }
        else 
        {
            controller.Animator.SetBool("EN01Running", false);
            StartCoroutine(StopAndReturn());
        }
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }

    private IEnumerator StopAndReturn()
    {
        yield return new WaitForSeconds(1);
        controller.Animator.SetBool("EN01Running", false);
        controller.ChangeState(controller.PatrolState);
    }
}