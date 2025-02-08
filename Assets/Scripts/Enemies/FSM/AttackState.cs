using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState<EnemyController>
{
    [SerializeField] private float baseAttackDamage;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        //controller.Agent.isStopped = true;
        controller.Agent.stoppingDistance = controller.AttackDistance;
        controller.Animator.SetBool("EN01Attacking", true);
    }

    public override void OnUpdateState()
    {
        /*controller.Agent.SetDestination(controller.Target.position);

        if (!controller.Agent.pathPending && controller.Agent.remainingDistance <= controller.AttackDistance)
        {
            
        }
        else 
        {
            controller.ChangeState(controller.PatrolState);
        }*/
    }

    public override void OnExitState()
    {
        
    }

    public void faceTarget()
    {
        Vector3 directionToTarget = (controller.Target.position - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
    }

    public void CheckTarget() 
    {
        if (Vector3.Distance(transform.position, controller.Target.transform.position) > controller.AttackDistance)
        {
            controller.Animator.SetBool("EN01Attacking", false);
            controller.ChangeState(controller.ChaseState);
        }
    }
}

