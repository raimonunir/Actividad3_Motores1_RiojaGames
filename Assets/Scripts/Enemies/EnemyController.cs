using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//TODO RageState: Te ve pero no puede alcanzarte
//TODO AlertState: Te acaba de perder de vista
//TODO DeathState: Ce murio
public class EnemyController : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange ;
    [SerializeField] private float attackDistance;
    [SerializeField] private float maxVelocity;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private EnemyState<EnemyController> currentState;
    private PatrolState patrolState;
    private AlertState alertState;
    private ChaseState chaseState;
    private AttackState attackState;

    public NavMeshAgent Agent { get => agent; }
    public Transform Target { get => target; set => target = value; }
    public Animator Animator { get => animator; }
    public LayerMask TargetMask { get => targetMask; }
    public LayerMask ObstacleMask { get => obstacleMask; }
    public PatrolState PatrolState { get => patrolState; }
    public AlertState AlertState { get => alertState; }
    public ChaseState ChaseState { get => chaseState; }
    public AttackState AttackState { get => attackState; }
    public float ViewAngle { get => viewAngle; }
    public float ViewRange { get => viewRange; }
    public float AttackDistance { get => attackDistance; }
    public float MaxVelocity { get => maxVelocity; }

    


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        patrolState = GetComponent<PatrolState>();
        alertState = GetComponent<AlertState>();
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
