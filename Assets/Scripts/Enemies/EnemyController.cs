using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange ;
    [SerializeField] private float attackDistance;

    private NavMeshAgent agent;
    private Transform target;
    private EnemyState<EnemyController> currentState;
    private PatrolState patrolState;
    private ChaseState chaseState;
    private AttackState attackState;

    public NavMeshAgent Agent { get => agent; }
    public Transform Target { get => target; set => target = value; }
    public LayerMask TargetMask { get => targetMask; }
    public LayerMask ObstacleMask { get => obstacleMask; }
    public float ViewAngle { get => viewAngle; }
    public float ViewRange { get => viewRange; }
    public float AttackDistance { get => attackDistance; }
    public PatrolState PatrolState { get => patrolState; }
    public ChaseState ChaseState { get => chaseState; }
    public AttackState AttackState { get => attackState; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolState = GetComponent<PatrolState>();
        chaseState = GetComponent<ChaseState>();
        attackState = GetComponent<AttackState>();

        ChangeState(patrolState);
    }

    private void Update()
    {
        if (currentState != null) currentState.OnUpdateState();
    }

    public void ChangeState(EnemyState<EnemyController> newState)
    {
        if (currentState != null) currentState.OnExitState();

        currentState = newState;
        currentState.OnEnterState(this);
    }
}
