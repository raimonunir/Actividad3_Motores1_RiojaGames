using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState<EnemyController>
{
    private const float PATROL_VELOCITY = 3.5f;
    private const float WAITING_TIME = 2.5f;

    [SerializeField] private Transform patrolRoute;

    private List<Vector3> waypoints = new List<Vector3>();
    private Vector3 currentWaypoint;
    private int indexCurrentWaypoint = 0;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        foreach (Transform point in patrolRoute)
        {
            waypoints.Add(point.position);
        }

        currentWaypoint = waypoints[indexCurrentWaypoint];

        controller.Agent.speed = PATROL_VELOCITY;
        controller.Agent.stoppingDistance = 0;

        StartCoroutine(PatrolAndWait());
    }

    public override void OnUpdateState()
    {
        Collider[] collsDetectados = Physics.OverlapSphere(transform.position, controller.ViewRange, controller.TargetMask);
        if (collsDetectados.Length > 0)
        {
            Vector3 directionATarget = (collsDetectados[0].transform.position - transform.position).normalized;

            if (!Physics.Raycast(transform.position, directionATarget, controller.ViewRange, controller.ObstacleMask))
            {
                if (Vector3.Angle(transform.forward, directionATarget) <= controller.ViewAngle / 2)
                {
                    controller.Target = collsDetectados[0].transform;
                    controller.Animator.SetBool("EN01Walking", false);
                    controller.ChangeState(controller.AlertState);
                }
            }
        }
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }

    private IEnumerator PatrolAndWait()
    {
        while (true)
        {
            //going destination
            controller.Agent.SetDestination(currentWaypoint);
            controller.Animator.SetBool("EN01Walking", true);
            yield return new WaitUntil(() => !controller.Agent.pathPending && controller.Agent.remainingDistance <= 0.2f);

            //waiting
            controller.Animator.SetBool("EN01Walking", false);
            yield return new WaitForSeconds(WAITING_TIME);
            CalcularNuevoDestino();
        }
    }

    private void CalcularNuevoDestino()
    {
        indexCurrentWaypoint++;
        if (indexCurrentWaypoint >= waypoints.Count) indexCurrentWaypoint = 0;
        currentWaypoint = waypoints[indexCurrentWaypoint];
    }
}