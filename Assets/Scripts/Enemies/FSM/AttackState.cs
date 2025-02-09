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
        if (false) //TODO Controlar vida de taget: if (controller.target.GetHealth <= 0)...
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
        if (Vector3.Distance(transform.position, controller.Target.transform.position) <= controller.Agent.stoppingDistance)
        {
            //TODO quitar vida a enemigo
            //controller.TakeDamage(51); //Quitar (es para pruebas) 
        }
    }
}

