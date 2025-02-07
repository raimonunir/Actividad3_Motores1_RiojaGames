using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState<EnemyController>
{
    [SerializeField] private float chaseVelocity;
    [SerializeField] private float timeBeforeBackToPatrol;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Agent.speed = chaseVelocity;
        controller.Agent.stoppingDistance = controller.AttackDistance;
        controller.Agent.acceleration = 1000000; //Para que siempre se detenga a la misma distancia ¿No esta funcionando?
    }

    public override void OnUpdateState()
    {
        if (!controller.Agent.pathPending && controller.Agent.CalculatePath(controller.Target.position, new NavMeshPath()))
        {
            controller.Agent.SetDestination(controller.Target.position);

            if (!controller.Agent.pathPending && controller.Agent.remainingDistance < controller.Agent.stoppingDistance)
            {
                controller.ChangeState(controller.AttackState);
            }
        }
        else 
        {
            StartCoroutine(StopAndReturn());
        }
    }

    private IEnumerator StopAndReturn()
    {
        yield return new WaitForSeconds(timeBeforeBackToPatrol);
        controller.ChangeState(controller.PatrolState);
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }
}