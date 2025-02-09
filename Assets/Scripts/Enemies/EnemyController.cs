using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int enemyId;
    [SerializeField] private int healthPoints;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private EnemyState<EnemyController> currentState;
    private PatrolState patrolState;
    private AlertState alertState;
    private ChaseState chaseState;
    private AttackState attackState;
    private TargetDestroyedState targetDestroyedState;

    private float bodyLength = 10;
    private int currentHealthPoints;

    #region setters & getters
    public NavMeshAgent Agent { get => agent; }
    public Transform Target { get => target; set => target = value; }
    public GameManagerSO GameManagerSO { get => gameManagerSO; }
    public Animator Animator { get => animator; }
    public LayerMask TargetMask { get => targetMask; }
    public LayerMask ObstacleMask { get => obstacleMask; }
    public PatrolState PatrolState { get => patrolState; }
    public AlertState AlertState { get => alertState; }
    public ChaseState ChaseState { get => chaseState; }
    public AttackState AttackState { get => attackState; }
    public TargetDestroyedState TargetDestroyedState { get => targetDestroyedState; }
    public int EnemyId { get => enemyId; }
    public float ViewAngle { get => viewAngle; }
    public float ViewRange { get => viewRange; }
    public float BodyLength { get => bodyLength; }
    #endregion

    private void OnEnable()
    {
        gameManagerSO.OnDamageEnemy += TakeDamage;
    }

    private void OnDisable()
    {
        gameManagerSO.OnDamageEnemy -= TakeDamage;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        patrolState = GetComponent<PatrolState>();
        alertState = GetComponent<AlertState>();
        chaseState = GetComponent<ChaseState>();
        attackState = GetComponent<AttackState>();
        targetDestroyedState = GetComponent<TargetDestroyedState>();

        currentHealthPoints = healthPoints;

        ChangeState(patrolState);
    }

    private void Update()
    {
        if (currentState != null) currentState.OnUpdateState();
    }

    public void ChangeState(EnemyState<EnemyController> newState)
    {
        Debug.Log("EnemyID: " + enemyId + ". ChangeState: " + newState.ToString());
        if (currentState != null) currentState.OnExitState();

        currentState = newState;
        currentState.OnEnterState(this);
    }

    public void TakeDamage(int enemyId, int damage)
    {
        if (enemyId == this.enemyId) {
            currentHealthPoints -= damage;

            animator.SetTrigger("EN01GetHurt");

            if (currentHealthPoints <= 0) Die();
        }
    }

    private void Die()
    {
        animator.SetBool("EN01Dying", true);
        this.enabled = false;
    }
}