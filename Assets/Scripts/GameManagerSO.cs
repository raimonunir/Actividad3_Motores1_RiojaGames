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
    [SerializeField][Range(10f, 100f)] private float m_maxHP;
    [SerializeField][Range(10f, 100f)] private float initialHP;
    [SerializeField][Range(1f, 100f)] private float minSpikeDamage;
    [SerializeField][Range(1f, 100f)] private float maxSpikeDamage;
    [SerializeField][Range(1f, 100f)] private float fireDamage;
    [SerializeField][Range(1f, 100f)] private float boulderDamage;
    [SerializeField][Range(1f, 100f)] private float poisonDamage;
    [SerializeField][Range(5f, 120f)][Tooltip("Seconds to sunset")] private float m_timerToGetDark;


    // events
    public event Action<int> OnSwitchActivated;
    public event Action<InteractuableObjectType> OnInteractuableObjectDetected;
    public event Action OnVictory;
    public event Action OnDeath;
    public event Action <float> OnUpdateHP;
    public event Action <float, float, float> OnShake;
    public event Action OnPlayerOnSpikes;

    private bool m_isAlive = true;
    private float currentHp;

    public bool isAlive {  get => m_isAlive;  }
    public float timerToDark { get => m_timerToGetDark; }
    public float maxHP { get =>  m_maxHP; }

    // validate inspector inputs
    private void OnValidate()
    {
        if (m_maxHP < initialHP)
        {
            m_maxHP = initialHP;
            Debug.LogWarning($" initialHP={initialHP} shouldn't be greater than maxH={maxHP}");
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
        if (m_isAlive) {
            OnDeath?.Invoke();
            m_isAlive = false;
            // update HP
            currentHp = 0;
            OnUpdateHP?.Invoke(currentHp);
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

    public void OnStart()
    {
        m_isAlive = true;

        // hp
        currentHp = initialHP;
        // update hp UI
        OnUpdateHP?.Invoke(currentHp);
    }

    public void Damage(DamageType damageType)
    {
        if (!m_isAlive) { return; }
        // XXX exit here if player is death

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
        
        // gameover
        if (currentHp <= 0f) { 
            currentHp = 0f;
            Death(); 
        }

        // update current hp
        OnUpdateHP?.Invoke(currentHp);
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
