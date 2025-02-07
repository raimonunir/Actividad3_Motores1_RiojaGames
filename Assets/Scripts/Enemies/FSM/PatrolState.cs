using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState<EnemyController>
{
    [SerializeField] private Transform patrolRoute;
    [SerializeField] private float patrolVelocity;
    [SerializeField] private float waitingTime;

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

        controller.Agent.speed = patrolVelocity;
        controller.Agent.stoppingDistance = 0;
        controller.Agent.acceleration = 8;

        StartCoroutine(PatrullarYEsperar());
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
                    Debug.Log("Player detectado.");
                    controller.Target = collsDetectados[0].transform;
                    controller.ChangeState(controller.ChaseState);
                }
            }
        }
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }

    private IEnumerator PatrullarYEsperar()
    {
        while (true)
        {
            controller.Agent.SetDestination(currentWaypoint);
            yield return new WaitUntil(() => !controller.Agent.pathPending && controller.Agent.remainingDistance <= 0.2f);
            yield return new WaitForSeconds(waitingTime);
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