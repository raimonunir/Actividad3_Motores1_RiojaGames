using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestroyedState : EnemyState<EnemyController>
{
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        controller.Animator.SetBool("EN01Roaring", true);
        controller.Target = null;

        StartCoroutine(returnToPatrol());
    }

    public override void OnUpdateState() {}

    public override void OnExitState() {}

    private IEnumerator returnToPatrol()
    {
        yield return new WaitForSeconds(1.5f);
        controller.Animator.SetBool("EN01Roaring", false);
        controller.ChangeState(controller.PatrolState);
    }
}