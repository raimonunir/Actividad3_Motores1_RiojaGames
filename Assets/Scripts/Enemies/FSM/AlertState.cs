using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : EnemyState<EnemyController>
{
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Agent.isStopped = true;
        controller.Animator.SetBool("EN01Alert", true);
    }

    public override void OnUpdateState()
    {
    }

    public override void OnExitState()
    {
    }

    public void ChaseTarget() {
        controller.Agent.isStopped = false;
        controller.Animator.SetBool("EN01Alert", false);
        controller.ChangeState(controller.ChaseState);
    }
}
