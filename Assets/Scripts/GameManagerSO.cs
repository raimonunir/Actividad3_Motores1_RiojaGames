using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "GameManagerSO")]
public class GameManagerSO : ScriptableObject
{

    public enum InteractuableObjectType {doorSwitch, nothing};
    public enum DamageType {spike, fire, boulder, poison}


    [SerializeField][Range(3f, 6f)] private float secondsToQuitAppAffterWin;
    [Header("HP & Damage")]
    [SerializeField][Range(10f, 100f)] private float maxHP;
    [SerializeField][Range(10f, 100f)] private float initialHP;
    [SerializeField][Range(1f, 100f)] private float spikeDamage;
    [SerializeField][Range(1f, 100f)] private float fireDamage;
    [SerializeField][Range(1f, 100f)] private float boulderDamage;
    [SerializeField][Range(1f, 100f)] private float poisonDamage;
    [SerializeField][Range(5f, 120f)][Tooltip("Seconds to sunset")] private float m_timerToGetDark;

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

    // events
    public event Action<int> OnSwitchActivated;
    public event Action<InteractuableObjectType> OnInteractuableObjectDetected;
    public event Action OnVictory;
    public event Action OnDeath;
    public event Action <float> OnUpdateHP;
    public event Action <float, float, float> OnShake;
    // Event to update the score UI (triggered when the general score changes)
    public event Action<int> OnScoreUpdated;

    private bool m_isAlive = true;
    private float currentHp;

    // General score and individual scores
    private int generalScore = 0;
    private int collectibleScore = 0;
    private int switchScore = 0;
    private int trapScore = 0;
    private int enemyDestructionScore = 0;
    private int enemyDamageScore = 0;



    public bool isAlive {  get => m_isAlive;  }
    public float timerToDark { get => m_timerToGetDark; }
    // Public properties to access the scores
    public int GeneralScore { get => generalScore; }
    public int CollectibleScore { get => collectibleScore; }
    public int SwitchScore { get => switchScore; }
    public int TrapScore { get => trapScore; }
    public int EnemyDestructionScore { get => enemyDestructionScore; }
    public int EnemyDamageScore { get => enemyDamageScore; }



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
        if (m_isAlive) {
            OnDeath?.Invoke();
            m_isAlive = false;
        }
    }

    public void Shake(float shakeAmount = 0.7f, float shakeDecreaseFactor = 0.01f, float shakeDuration = 1.5f)
    {
        OnShake?.Invoke(shakeAmount, shakeDecreaseFactor, shakeDuration);  
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetAlive()
    {
        m_isAlive = true;
        currentHp = initialHP;
        generalScore = 0; // Asegurar que la puntuación inicia correctamente
        Debug.Log("GameManagerSO initialized, Score: " + generalScore);
    }

    public void Damage(DamageType damageType)
    {
        if (damageType == DamageType.spike)
        {
            currentHp -= spikeDamage;
        }else if(damageType == DamageType.boulder)
        {
            currentHp -= boulderDamage;
        }else if(damageType == DamageType.fire)
        {
            currentHp -= fireDamage;
        }else if(damageType == DamageType.poison) 
        { 
            currentHp -= poisonDamage; 
        }
        Debug.Log($"currentHp={currentHp}");


        // update current live
        OnUpdateHP?.Invoke(currentHp);

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
