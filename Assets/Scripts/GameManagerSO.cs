using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
    public enum InteractuableObjectType {doorSwitch, nothing};
    public enum DamageType {spike, fire, boulder, poison, sabre}

    [Header("HP & Lives")]
    [SerializeField][Range(10f, 100f)] private float maxHP;
    [SerializeField][Range(10f, 100f)] private float initialHP;
    [Tooltip("Act like seriously injured under this HP value")]
    [SerializeField][Range(10f, 50f)] private float seriouslyInjured;
    [SerializeField][Range(1, 5)] private int initialLives;
    [SerializeField] private Vector3 initialRespawnPosition;
    [SerializeField] private Vector3 currentRespawnPosition;
    [Header("Damage")]
    [SerializeField][Range(1f, 100f)] private float minSpikeDamage;
    [SerializeField][Range(1f, 100f)] private float maxSpikeDamage;
    [SerializeField][Range(1f, 100f)] private float minSabreDamage;
    [SerializeField][Range(1f, 100f)] private float maxSabreDamage;
    [SerializeField][Range(1f, 100f)] private float fireDamage;
    [SerializeField][Range(1f, 100f)] private float boulderDamage;
    [SerializeField][Range(1f, 100f)] private float poisonDamage;
    [Header("Other settings")]
    [SerializeField][Range(3f, 6f)] private float secondsToQuitAppAffterWin;
    [SerializeField][Range(5f, 120f)][Tooltip("Seconds to sunset")] private float timerToGetDark;

    [Header("Scoring Values")]
    [SerializeField] private int pointsPerCollectible = 20;
    [SerializeField] private int pointsPerSwitch = 10;
    [SerializeField] private int pointsPerTrap = 15;
    [SerializeField] private int pointsPerEnemyDestruction = 30;
    [SerializeField] private int pointsPerEnemyDamage = 10;

    [Header("Scoring Options")]
    [SerializeField] private bool enableGeneralScoring = true;
    [SerializeField] private bool enableCollectibleScoring = true;
    [SerializeField] private bool enableSwitchScoring = true;
    [SerializeField] private bool enableTrapScoring = true;
    [SerializeField] private bool enableEnemyDestructionScoring = true;
    [SerializeField] private bool enableEnemyDamageScoring = true;

    [Header("Idol Settings")]
    [SerializeField] private GameObject collectiblePrefab; // Prefab of the collectible
    public GameObject CollectiblePrefab => collectiblePrefab;



    // events
    public event Action<int> OnSwitchActivated;
    public event Action<int,int> OnDamageEnemy;
    public event Action<InteractuableObjectType> OnInteractuableObjectDetected;
    public event Action OnVictory;
    public event Action OnDeath;
    public event Action OnPlayerOnSpikes;
    public event Action <float> OnUpdateHP;
    public event Action <float, float, float> OnShake;
    // Event to update the score UI (triggered when the general score changes)
    public event Action<int> OnScoreUpdated;
    public event Action<int,int> OnCollectibleScoring;
    public event Action OnInjured;
    public event Action OnSeriouslyInjured;
    public event Action OnResetLevel;
    public event Action<Transform> OnSetPlayerPosition;


    private int collectedCollectibles = 0; // Contador de coleccionables recogidos
    private int totalCollectibles = 0;
    private bool isAlive = true;
    private bool isSeriouslyInjured = false;
    private float currentHp;
    private int currentLives;

    // General score and individual scores
    private int generalScore = 0;
    private int collectibleScore = 0;
    private int switchScore = 0;
    private int trapScore = 0;
    private int enemyDestructionScore = 0;
    private int enemyDamageScore = 0;



    public bool IsAlive {  get => isAlive;  }
    public float TimerToDark { get => timerToGetDark; }
    // Public properties to access the scores
    public int GeneralScore => CollectibleScore + SwitchScore + TrapScore + EnemyDamageScore + EnemyDestructionScore;
    public int CollectibleScore { get => collectibleScore; }
    public int SwitchScore { get => switchScore; }
    public int TrapScore { get => trapScore; }
    public int EnemyDestructionScore { get => enemyDestructionScore; }
    public int EnemyDamageScore { get => enemyDamageScore; }
    public float MaxHP { get => maxHP; }
    public bool IsSeriouslyInjured { get => isSeriouslyInjured; }



    public bool EnableGeneralScoring => enableGeneralScoring;
    public bool EnableCollectibleScoring => enableCollectibleScoring;
    public bool EnableSwitchScoring => enableSwitchScoring;
    public bool EnableTrapScoring => enableTrapScoring;
    public bool EnableEnemyDamageScoring => enableEnemyDamageScoring;
    public bool EnableEnemyDestructionScoring => enableEnemyDestructionScoring;

    private void OnValidate()
    {
        if (maxHP < initialHP)
        {
            maxHP = initialHP;
            Debug.LogWarning($" initialHP={initialHP} shouldn't be greater than maxH={MaxHP}");
        }

        if (minSpikeDamage > maxSpikeDamage)
        {
            maxSpikeDamage = minSpikeDamage;
        }
    }

    public void SetPlayerPosition(Transform transform)
    {
        OnSetPlayerPosition?.Invoke(transform);
    }

    public void SetTotalCollectibles(int total)
    {
        totalCollectibles = total;
        collectedCollectibles = 0; // Reinicia el contador de recogidos
        Debug.Log($" SetTotalCollectibles() called! Total: {total}");
        generalScore = 0;
        collectibleScore = 0;
        switchScore = 0;
        trapScore = 0;
        enemyDamageScore = 0;
        enemyDestructionScore = 0;
        // Llamar al evento para actualizar la UI al iniciar el juego
        OnCollectibleScoring?.Invoke(collectedCollectibles, totalCollectibles);
    }

    // Switch has been activated
    public void SwitchActivated(int idSwitch)
    {
        OnSwitchActivated?.Invoke(idSwitch);
        int points = pointsPerSwitch;
        if (enableGeneralScoring)
        {
            generalScore += points;
            OnScoreUpdated?.Invoke(generalScore);
        }
        if (enableSwitchScoring)
        {
            switchScore += points;
        }
        Debug.Log($"SwitchActivated called! Score: {generalScore}, Added: {points}, Switch ID: {idSwitch}");
    }

    public void DamageEnemy(int enemyId, int damage)
    {
        OnDamageEnemy?.Invoke(enemyId, damage);
    }

    public void InfoUI(InteractuableObjectType interactuableObject)
    {
        OnInteractuableObjectDetected?.Invoke(interactuableObject);
    }

    public void Victory()
    {
        OnVictory?.Invoke();
    }

    public void Death()
    {
        if (isAlive)
        {
            OnDeath?.Invoke();
            isAlive = false;
            // update HP
            currentHp = 0;
            OnUpdateHP?.Invoke(currentHp);
        }

        currentLives--;

        if (currentLives == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        throw new NotImplementedException();
    }

    public void Shake(float shakeAmount = 0.7f, float shakeDecreaseFactor = 0.01f, float shakeDuration = 1.5f)
    {
        OnShake?.Invoke(shakeAmount, shakeDecreaseFactor, shakeDuration);  
    }

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
        Start();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Start()
    {
        isAlive = true;
        isSeriouslyInjured = false;
        currentLives = initialLives;

        // hp
        currentHp = initialHP;
        // update hp UI
        OnUpdateHP?.Invoke(currentHp);
    }

    public void SetAlive()
    {
        isAlive = true;
        currentHp = initialHP;
        generalScore = 0; // Asegurar que la puntuaci�n inicia correctamente
        Debug.Log("GameManagerSO initialized, Score: " + generalScore);
    }

    public void PlayerOnSpikes()
    {
        OnPlayerOnSpikes?.Invoke();
    }

    public void Damage(DamageType damageType)
    {
        if (!isAlive) { return; }
        // XXX exit here if player is death

        // notify player
        OnInjured?.Invoke();

        if (damageType == DamageType.spike)
        {
            currentHp -= UnityEngine.Random.Range(minSpikeDamage, maxSpikeDamage);
        }
        else if(damageType == DamageType.boulder)
        {
            currentHp -= boulderDamage;
        }else if(damageType == DamageType.fire)
        {
            currentHp -= fireDamage;
        }else if(damageType == DamageType.poison) 
        { 
            currentHp -= poisonDamage; 
        }
        else if (damageType == DamageType.sabre)
        {
            currentHp -= UnityEngine.Random.Range(minSabreDamage, maxSabreDamage);
        }

        Debug.Log($"currentHp={currentHp}");

        // limit currentHp to 0
        if (currentHp < 0) currentHp = 0f;


        // update current live
        OnUpdateHP?.Invoke(currentHp);

        // is player seriously injured?
        if (!isSeriouslyInjured && currentHp < seriouslyInjured)
        {
            isSeriouslyInjured = true;
            OnSeriouslyInjured?.Invoke();
        }

        // Penalize for trap damage (-pointsPerTrap)
        int penalty = pointsPerTrap;
        if (enableGeneralScoring)
        {
            generalScore -= penalty;
            OnScoreUpdated?.Invoke(generalScore);
        }
        if (enableTrapScoring)
        {
            trapScore -= penalty;
        }

        // gameover
        if (currentHp <= 0f) { Death(); }
    }

    #region Scoring Methods
    // Called when the player receives damage from an enemy (-pointsPerEnemyDamage)
    public void DamageFromEnemy()
    {
        int penalty = pointsPerEnemyDamage;
        if (enableGeneralScoring)
        {
            generalScore -= penalty;
            OnScoreUpdated?.Invoke(generalScore);
        }
        if (enableEnemyDamageScoring)
        {
            enemyDamageScore -= penalty;
        }
    }

    // Called when an enemy is destroyed (+pointsPerEnemyDestruction)
    public void EnemyDestroyed()
    {
        int points = pointsPerEnemyDestruction;
        if (enableGeneralScoring)
        {
            generalScore += points;
            OnScoreUpdated?.Invoke(generalScore);
        }
        if (enableEnemyDestructionScoring)
        {
            enemyDestructionScore += points;
        }
    }

    // Called when a gem or collectible is collected (+pointsPerCollectible)
    public void GemCollected()
    {
        int points = pointsPerCollectible;
        if (enableGeneralScoring)
        {
            generalScore += points;
            OnScoreUpdated?.Invoke(generalScore);
            
        }
        if (enableCollectibleScoring)
        {
            collectibleScore += points;
        }

        // Incrementar el contador de coleccionables recogidos
        collectedCollectibles++;

        // Invocar el evento pasando el n�mero de recogidos y el total
        OnCollectibleScoring?.Invoke(collectedCollectibles, totalCollectibles);

        Debug.Log($"Collected: {collectedCollectibles} / {totalCollectibles}");
    }
    #endregion

    public void Exit()
    {
        Debug.Log("Byebye!");
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }
}