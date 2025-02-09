using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState<EnemyController>
{
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Agent.stoppingDistance = (controller.BodyLength / 2);
        controller.Animator.SetBool("EN01Attacking", true);
    }

    public override void OnUpdateState()
    {
        faceTarget();
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
        if (!controller.GameManagerSO.isAlive)
        {
            controller.Animator.SetBool("EN01Attacking", false);
            controller.ChangeState(controller.TargetDestroyedState);
        }
        else 
        {
            if (Vector3.Distance(transform.position, controller.Target.transform.position) > controller.Agent.stoppingDistance)
            {
                controller.Animator.SetBool("EN01Attacking", false);
                controller.ChangeState(controller.ChaseState);
            }
        }
    }

    public void HitTarget()
    {
        Debug.Log("HitTarget");
        if (Vector3.Distance(transform.position, controller.Target.transform.position) <= controller.Agent.stoppingDistance)
        {
            Debug.Log("Attack succesful");

            controller.GameManagerSO.Damage(GameManagerSO.DamageType.sabre);
        }
    }
}

