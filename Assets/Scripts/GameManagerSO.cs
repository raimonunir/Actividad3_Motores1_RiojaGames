using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "GameManagerSO")]
public class GameManagerSO : ScriptableObject
{

    public enum InteractuableObjectType {doorSwitch, nothing};
    public enum DamageType {spike, fire, boulder, poison}

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
    [SerializeField][Range(1f, 100f)] private float fireDamage;
    [SerializeField][Range(1f, 100f)] private float boulderDamage;
    [SerializeField][Range(1f, 100f)] private float poisonDamage;
    [Header("Other settings")]
    [SerializeField][Range(3f, 6f)] private float secondsToQuitAppAffterWin;
    [SerializeField][Range(5f, 120f)][Tooltip("Seconds to sunset")] private float timerToGetDark;


    // events
    public event Action<int> OnSwitchActivated;
    public event Action<InteractuableObjectType> OnInteractuableObjectDetected;
    public event Action OnVictory;
    public event Action OnDeath;
    public event Action <float> OnUpdateHP;
    public event Action <float, float, float> OnShake;
    public event Action OnPlayerOnSpikes;
    public event Action OnSeriouslyInjured;
    public event Action OnInjured;
    public event Action OnResetLevel;

    private bool isAlive = true;
    private bool isSeriouslyInjured = false;
    private float currentHp;
    private int currentLives;

    public bool IsAlive {  get => isAlive;  }
    public float TimerToDark { get => timerToGetDark; }
    public float MaxHP { get =>  maxHP; }
    public bool IsSeriouslyInjured { get => isSeriouslyInjured; }


    // validate inspector inputs
    private void OnValidate()
    {
        if (maxHP < initialHP)
        {
            maxHP = initialHP;
            Debug.LogWarning($" initialHP={initialHP} shouldn't be greater than maxH={MaxHP}");
        }

        if(minSpikeDamage > maxSpikeDamage)
        {
            maxSpikeDamage = minSpikeDamage;
        }
    }

    public void PlayerOnSpikes()
    {
        OnPlayerOnSpikes?.Invoke();
    }

    // Switch has been activated
    public void SwitchActivated(int idSwitch)
    {
        OnSwitchActivated?.Invoke(idSwitch);
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

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
        Start();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void Shake(float shakeAmount = 0.7f, float shakeDecreaseFactor = 0.01f, float shakeDuration = 1.5f)
    {
        OnShake?.Invoke(shakeAmount, shakeDecreaseFactor, shakeDuration);  
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

    public void Damage(DamageType damageType)
    {
        if (!isAlive) { return; }
        // XXX exit here if player is death

        // notify player
        OnInjured?.Invoke();

        // calculate hp
        if (damageType == DamageType.spike)
        {
            currentHp -= UnityEngine.Random.Range(minSpikeDamage, maxSpikeDamage);
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

        // limit currentHp to 0
        if(currentHp < 0) currentHp = 0f;

        // update current hp
        OnUpdateHP?.Invoke(currentHp);

        // is player seriously injured?
        if( !isSeriouslyInjured && currentHp < seriouslyInjured)
        {
            isSeriouslyInjured = true;
            OnSeriouslyInjured?.Invoke();
        }

        // death?
        if (currentHp <= 0f) Death(); 

    }


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
