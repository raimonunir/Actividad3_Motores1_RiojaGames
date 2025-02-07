using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState<EnemyController>
{
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float baseAttackDamage;

    private float timer;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        timer = timeBetweenAttack;
        controller.Agent.stoppingDistance = controller.AttackDistance;
    }

    public override void OnUpdateState()
    {
        controller.Agent.SetDestination(controller.Target.position);

        if (!controller.Agent.pathPending && controller.Agent.remainingDistance <= controller.Agent.stoppingDistance)
        {
            timer += Time.deltaTime;

            if (timer >= timeBetweenAttack)
            {
                Debug.Log("Ataque");
                timer = 0;
            }
        }
        else 
        {
            controller.ChangeState(controller.PatrolState);
        }
    }

    public override void OnExitState()
    {

    }
}