using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "GameManagerSO")]
public class GameManagerSO : ScriptableObject
{

    public enum InteractuableObjectType {doorSwitch, nothing};
    public enum DamageType {spike, fire, boulder, poison, sabre}


    [SerializeField][Range(3f, 6f)] private float secondsToQuitAppAffterWin;
    [Header("HP & Damage")]
    [SerializeField][Range(10f, 100f)] private float maxHP;
    [SerializeField][Range(10f, 100f)] private float initialHP;
    [SerializeField][Range(1f, 100f)] private float spikeDamage;
    [SerializeField][Range(1f, 100f)] private float fireDamage;
    [SerializeField][Range(1f, 100f)] private float boulderDamage;
    [SerializeField][Range(1f, 100f)] private float poisonDamage;
    [SerializeField][Range(1f, 100f)] private float minSabreDamage;
    [SerializeField][Range(1f, 100f)] private float maxSabreDamage;
    [SerializeField][Range(5f, 120f)][Tooltip("Seconds to sunset")] private float m_timerToGetDark;


    // events
    public event Action<int> OnSwitchActivated;
    public event Action<int> OnDamageEnemy;
    public event Action<InteractuableObjectType> OnInteractuableObjectDetected;
    public event Action OnVictory;
    public event Action OnDeath;
    public event Action <float> OnUpdateHP;
    public event Action <float, float, float> OnShake;

    private bool m_isAlive = true;
    private float currentHp;

    public bool isAlive {  get => m_isAlive;  }
    public float timerToDark { get => m_timerToGetDark; }

    // Switch has been activated
    public void SwitchActivated(int idSwitch)
    {
        OnSwitchActivated?.Invoke(idSwitch);
    }

    public void DamageEnemy(int damage)
    {
        OnDamageEnemy?.Invoke(damage);
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
        else if (damageType == DamageType.sabre)
        {
            currentHp -= UnityEngine.Random.Range(minSabreDamage, maxSabreDamage);
        }

        Debug.Log($"currentHp={currentHp}");


        // update current live
        OnUpdateHP?.Invoke(currentHp);
        
        // gameover
        if (currentHp <= 0f) { Death(); }
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
