using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.HableCurve;

public class PatrolState : EnemyState<EnemyController>
{
    private const float PATROL_VELOCITY = 3.5f;
    private const float WAITING_TIME = 2.5f;

    [SerializeField] private Transform patrolRoute;
    [SerializeField] private Boolean showDetectionZone;

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
        if (showDetectionZone) ShowDetectionZone();

        Collider[] collsDetectados = Physics.OverlapSphere(transform.position, controller.ViewRange, controller.TargetMask);

        if (collsDetectados.Length > 0)
        {
            Vector3 directionATarget = (collsDetectados[0].transform.position - transform.position).normalized;

            if (!Physics.Raycast(transform.position, directionATarget, controller.ViewRange, controller.ObstacleMask))
            {
                if (Vector3.Angle(transform.forward, directionATarget) <= controller.ViewAngle / 2)
                {
                    controller.Target = collsDetectados[0].transform;

                    if(controller.Agent.CalculatePath(controller.Target.position, new NavMeshPath()))
                    {
                        controller.Animator.SetBool("EN01Walking", false);
                        controller.ChangeState(controller.AlertState);
                    }
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

    private void ShowDetectionZone()
    {
        Vector3 center = transform.position;
        const int segments = 36;

        //draw overlap sphere
        for (int i = 0; i < 36; i++)
        {
            float angle1 = Mathf.Deg2Rad * (i * 360f / segments);
            float angle2 = Mathf.Deg2Rad * ((i + 1) * 360f / segments);

            Vector3 point1 = center + new Vector3(Mathf.Sin(angle1), 0, Mathf.Cos(angle1)) * controller.ViewRange;
            Vector3 point2 = center + new Vector3(Mathf.Sin(angle2), 0, Mathf.Cos(angle2)) * controller.ViewRange;

            Debug.DrawLine(point1, point2, Color.red);
        }

        //draw vision cone
        float angleStep = controller.ViewAngle / segments;
        Vector3 startDirection = transform.forward;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i * angleStep) - (controller.ViewAngle / 2);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * startDirection;
            Vector3 endPoint = center + direction * controller.ViewRange;

            Debug.DrawLine(center, endPoint, Color.red);
        }
    }
}