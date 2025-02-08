using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestroyedState : EnemyState<EnemyController>
{
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Agent.isStopped = true;
        controller.Animator.SetBool("EN01Roaring", true);
    }

    public override void OnUpdateState()
    {
    }

    public override void OnExitState()
    {
    }
}
