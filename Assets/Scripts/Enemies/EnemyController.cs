using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int enemyId;
    [SerializeField] private float healthPoints;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange;
    [SerializeField] private Slider healthBarSlider;

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
    private float currentHealthPoints;
    private bool isDead;

    //Clips de audio
    [SerializeField] public AudioClip tigerDamage;
    [SerializeField] public AudioClip tigerDeath;
    [SerializeField] public AudioClip tigerRunning;
    [SerializeField] public AudioClip tigerBite;
    [SerializeField] public AudioClip tigerRoar;

    private AudioSource fuenteSonido;

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
    public float HealthPoints { get => healthPoints;}
    #endregion

    public SkinnedMeshRenderer enemyRender;

    public BoxCollider enemyCollider;

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
        healthBarSlider.value = currentHealthPoints / healthPoints;

        ChangeState(patrolState);

        fuenteSonido = GetComponent<AudioSource>();
        enemyRender = GetComponentInChildren<SkinnedMeshRenderer>();
        enemyCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (currentState != null && !isDead) currentState.OnUpdateState();
    }

    public void ChangeState(EnemyState<EnemyController> newState)
    {
        if (!isDead)
        {
            Debug.Log("EnemyID: " + enemyId + ". ChangeState: " + newState.ToString());
            if (currentState != null) currentState.OnExitState();

            currentState = newState;
            currentState.OnEnterState(this);
        }
    }

    public void TakeDamage(int enemyId, float damage)
    {
        if (enemyId == this.enemyId) {
            currentHealthPoints -= damage;

            healthBarSlider.value = currentHealthPoints / healthPoints;

            //llamamos a una corrutina para tintar al enemigo
            StartCoroutine(DamageBlink(0.15f));

            fuenteSonido.PlayOneShot(tigerDamage);

            animator.SetTrigger("EN01GetHurt");

            if (currentHealthPoints <= 0) Die();
        }
    }

    private void Die()
    {
        fuenteSonido.PlayOneShot(tigerDeath);
        animator.SetBool("EN01Dying", true);

        isDead = true;
        agent.isStopped = true;
        this.enabled = false;
        enemyCollider.enabled = false;
    }

    private IEnumerator DamageBlink(float tiempoBlink)
    {

   
        //Como en el enemigo hay varios materiales vamos a iterar entre todos ellos
        foreach (Material mat in enemyRender.materials)
        {
            mat.color = Color.red;
        }

        //Hacemos un yield return que aguardará durante el tiempo indicado en tiempoBlink
        yield return new WaitForSeconds(tiempoBlink);

        //y volvermos a poner los colores originales
        //Como en el enemigo hay varios materiales vamos a iterar entre todos ellos
        foreach (Material mat in enemyRender.materials)
        {
            mat.color = Color.white;
        }
    }
}