using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;

    [SerializeField] private GameObject panelStartUI;
    [SerializeField][Range(0.5f, 1.5f)] private float minimumSecondsShowingStartMenu;
    [SerializeField] private GameObject panelEndGameUI;
    [SerializeField] private TextMeshProUGUI statsScoreText; // Texto donde se mostrarán las estadísticas;
    [SerializeField] private GameObject panelMiniMap;
    [SerializeField] private GameObject panelDeath;
    [SerializeField] private TextMeshProUGUI textRestartIn;
    [SerializeField] private Image HealthBar;
    [SerializeField] private Text HealthBarValueText;
    [SerializeField] private GameObject panelInjured;
    [SerializeField][Range(1f, 5f)] private float secondsShowingPanelInjured;
    [SerializeField] private GameObject panelSeriouslyInjured;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelDebug;
    [SerializeField] private GameObject HeartLiveOne;
    [SerializeField] private GameObject HeartLiveTwo;
    [SerializeField] private GameObject HeartLiveThree;




    [Header("Score UI")]
    [SerializeField] private GameObject panelScore;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Collectable UI")]
    [SerializeField] private GameObject CollectableUI;
    [SerializeField] private TextMeshProUGUI collectibleText;

    [Header("Camera Target UI")]
    [SerializeField] private GameObject panelCameraTarget;
    [SerializeField] private Image imagePointCameraTarget;
    [SerializeField] private TextMeshProUGUI textTargetInfo;

    [Header("Debug Positions")]
    [SerializeField] private Transform debugPositionInitial;
    [SerializeField] private Transform debugPositionMid1;
    [SerializeField] private Transform debugPositionMid2;
    [SerializeField] private Transform debugPositionFinal;

    private Animator panelStartUIanimator;
    private bool firstClick = false;
    private bool isDesativaeTargetInfoRunning = false;
    private bool targetUIwasShowingSomething = false;
    private float secondsToQuitAppAffterWin = 3f;


    // Start is called before the first frame update
    void Start()
    {
        InitialSet();
    }

    private void Update()
    {
        // hide/show miniMap
        if (Input.GetKeyDown(KeyCode.H))
        {
            panelMiniMap.SetActive(!panelMiniMap.activeSelf);
            CollectableUI.SetActive(!CollectableUI.activeSelf);
        }

        // hide start panel UI on first click
        if (!firstClick && Input.GetKeyDown(KeyCode.Mouse0)) {
            firstClick = true;
            StartCoroutine(FadeOutStartPanelUI());
        }

        // test console
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.C))
        {
            panelDebug.SetActive(!panelDebug.activeSelf);
        }

        if (panelDebug.activeSelf) {
            if (Input.GetKeyDown(KeyCode.K))
            {
                gameManagerSO.Damage(GameManagerSO.DamageType.spike);
            }            
            
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                gameManagerSO.SetPlayerPosition(debugPositionInitial);
            }            
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                gameManagerSO.SetPlayerPosition(debugPositionMid1);
            }            
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                gameManagerSO.SetPlayerPosition(debugPositionMid2);
            }            
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                gameManagerSO.SetPlayerPosition(debugPositionFinal);
            }
            
        }
    }


    private void OnEnable()
    {
        gameManagerSO.OnInteractuableObjectDetected += GameManagerSO_OnInteractuableObjectDetected;
        gameManagerSO.OnVictory += GameManagerSO_OnVictory;
        gameManagerSO.OnDeath += GameManagerSO_OnDeath;
        gameManagerSO.OnScoreUpdated += GameManagerSO_OnScoreUpdated;
        gameManagerSO.OnCollectibleScoring += GameManagerSO_OnCollectibleScoring;
        gameManagerSO.OnUpdateHP += GameManagerSO_OnUpdateHP;
        gameManagerSO.OnSeriouslyInjured += GameManagerSO_OnSeriouslyInjured;
        gameManagerSO.OnInjured += GameManagerSO_OnInjured;
        gameManagerSO.OnGameOver += GameManagerSO_OnGameOver;
        gameManagerSO.OnResetLevel += GameManagerSO_OnResetLevel;
        gameManagerSO.OnUpdateLives += GameManagerSO_OnUpdateLives;
    }

    private void OnDisable()
    {
        gameManagerSO.OnInteractuableObjectDetected -= GameManagerSO_OnInteractuableObjectDetected;
        gameManagerSO.OnVictory -= GameManagerSO_OnVictory;
        gameManagerSO.OnDeath -= GameManagerSO_OnDeath;
        gameManagerSO.OnScoreUpdated -= GameManagerSO_OnScoreUpdated;
        gameManagerSO.OnCollectibleScoring -= GameManagerSO_OnCollectibleScoring;
        gameManagerSO.OnUpdateHP -= GameManagerSO_OnUpdateHP;
        gameManagerSO.OnSeriouslyInjured -= GameManagerSO_OnSeriouslyInjured;
        gameManagerSO.OnInjured -= GameManagerSO_OnInjured;
        gameManagerSO.OnGameOver -= GameManagerSO_OnGameOver;
        gameManagerSO.OnResetLevel -= GameManagerSO_OnResetLevel;
        gameManagerSO.OnUpdateLives -= GameManagerSO_OnUpdateLives;
    }

    private void GameManagerSO_OnUpdateLives(int obj)
    {
        if (obj == 0)
        {
            HeartLiveOne.SetActive(false);
            HeartLiveTwo.SetActive(false);
            HeartLiveThree.SetActive(false);
        }
        else if (obj == 1)
        {
            HeartLiveOne.SetActive(true);
            HeartLiveTwo.SetActive(false);
            HeartLiveThree.SetActive(false);
        }
        else if (obj == 2)
        {
            HeartLiveOne.SetActive(true);
            HeartLiveTwo.SetActive(true);
            HeartLiveThree.SetActive(false);
        }
        else if (obj == 3) {
            HeartLiveOne.SetActive(true);
            HeartLiveTwo.SetActive(true);
            HeartLiveThree.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Number of lives don't match");
        }
    }

    private void InitialSet()
    {
        // setting UI panels and animators
        panelStartUI.SetActive(true);
        panelCameraTarget.SetActive(false);
        panelEndGameUI.SetActive(false);
        panelInjured.SetActive(false);
        panelSeriouslyInjured.SetActive(false);
        panelDeath.SetActive(false);
        panelScore.SetActive(false);
        CollectableUI.SetActive(false);
        panelDebug.SetActive(false);
        panelGameOver.SetActive(false);
        panelStartUIanimator = panelStartUI.GetComponent<Animator>();
        imagePointCameraTarget.enabled = false;
        textTargetInfo.enabled = false;
        textTargetInfo.text = "";
        scoreText.text = "0";
        //collectibleText.text = "";
    }

    private void GameManagerSO_OnResetLevel()
    {
        InitialSet();
    }

    private void GameManagerSO_OnGameOver()
    {
        panelGameOver.SetActive(true);
        StartCoroutine(Wait());
        gameManagerSO.LoadMainMenu();
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
    }

    private void GameManagerSO_OnInjured()
    {
        if ( !gameManagerSO.IsSeriouslyInjured)
        {
            StartCoroutine(showInjuredPanelForSeconds());
        }   
    }

    private IEnumerator showInjuredPanelForSeconds()
    {
        panelInjured.SetActive(true);
        yield return new WaitForSeconds(secondsShowingPanelInjured);
        panelInjured.SetActive(false);
    }
    private void GameManagerSO_OnSeriouslyInjured()
    {
        panelSeriouslyInjured.SetActive(true);
    }

    private void GameManagerSO_OnUpdateHP(float hp)
    {
        HealthBar.fillAmount = hp / gameManagerSO.MaxHP;
        HealthBarValueText.text = "" + hp.ToString("f0");
    }


    private void GameManagerSO_OnDeath()
    {
        panelMiniMap.SetActive(false);
        panelScore.SetActive(false);
        CollectableUI.SetActive(false);
        panelCameraTarget.SetActive(false);
        panelDeath.SetActive(true);
        Debug.Log("CanvasONDEAAAATH");
        StartCoroutine(RestartCountdown());
    }

    private IEnumerator RestartCountdown()
    {
        Debug.Log("CoroutineONDEAAAATH");
        int seconds = 3;
        while (seconds > 0)
        {
            Debug.Log($"Restarting the game in {seconds} seconds");
            textRestartIn.text = "Restarting the game in "+seconds+" seconds";
            yield return new WaitForSeconds(1f);
            seconds--;
        }

        gameManagerSO.ResetLevel();
    }


    private void GameManagerSO_OnVictory()
    {
        // hide minimap, show UI end game
        panelMiniMap.SetActive(false);
        panelCameraTarget.SetActive(false);
        panelScore.SetActive(false);
        CollectableUI.SetActive(false);
        panelEndGameUI.SetActive(true);
        StartCoroutine(ExitAppAfterSeconds(secondsToQuitAppAffterWin));
        Time.timeScale = 0f;
        ShowStats();
    }

    private void ShowStats()
    {
        if (statsScoreText != null)
        {
            string statsText = "FINAL SCORE\n\n";

            if (gameManagerSO.EnableGeneralScoring)
                statsText += $"General Score: {gameManagerSO.GeneralScore}\n";

            if (gameManagerSO.EnableCollectibleScoring)
                statsText += $"Collectible Score: {gameManagerSO.CollectibleScore}\n";

            if (gameManagerSO.EnableSwitchScoring)
                statsText += $"Switch Score: {gameManagerSO.SwitchScore}\n";

            if (gameManagerSO.EnableTrapScoring)
                statsText += $"Trap Penalty: {gameManagerSO.TrapScore}\n";

            if (gameManagerSO.EnableEnemyDamageScoring)
                statsText += $"Enemy Damage Penalty: {gameManagerSO.EnemyDamageScore}\n";

            if (gameManagerSO.EnableEnemyDestructionScoring)
                statsText += $"Enemies Eliminated Score: {gameManagerSO.EnemyDestructionScore}\n";

            statsScoreText.text = statsText;
        }
    }

    private void GameManagerSO_OnInteractuableObjectDetected(GameManagerSO.InteractuableObjectType obj)
    {
        // activate point
        imagePointCameraTarget.enabled = true;

        // get info text
        if (obj == GameManagerSO.InteractuableObjectType.doorSwitch) {
            textTargetInfo.enabled = true;
            textTargetInfo.text = "Door Switch (Press 'E' to activate)";
            targetUIwasShowingSomething = true;
        } else if (obj == GameManagerSO.InteractuableObjectType.respawnPoint) {
            textTargetInfo.enabled = true;
            textTargetInfo.text = "Respawn Point (Press 'E' to activate)";
            targetUIwasShowingSomething = true;
        }
        else if(obj == GameManagerSO.InteractuableObjectType.nothing)
        {
            imagePointCameraTarget.enabled = false;

            if (!isDesativaeTargetInfoRunning && targetUIwasShowingSomething) 
            {
                StartCoroutine(DesactivateTargetInfo());
            }

            targetUIwasShowingSomething = false;
        }
        
    }
    private void GameManagerSO_OnScoreUpdated(int newScore)
    {
        Debug.Log($"Updating Score UI: {newScore}");
        if (scoreText != null)
            scoreText.text = "" + newScore;
    }

    private void GameManagerSO_OnCollectibleScoring(int collected, int total)
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"{collected} / {total}";
        }
    }


    private IEnumerator DesactivateTargetInfo()
    {
        isDesativaeTargetInfoRunning = true;

        yield return new WaitForSeconds(1f);
        imagePointCameraTarget.enabled = false;

        textTargetInfo.enabled = false;

        isDesativaeTargetInfoRunning = false;
    }



    private IEnumerator FadeOutStartPanelUI()
    {
        float secondsOffsetAfterAnimationEnds = 1f;

        // fade out start menu
        yield return new WaitForSeconds(minimumSecondsShowingStartMenu);
        panelStartUIanimator.SetTrigger("FadeOut");

        // cursor locked
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        // wait for fadeout
        yield return new WaitForSeconds(panelStartUIanimator.GetCurrentAnimatorStateInfo(0).length + secondsOffsetAfterAnimationEnds);

        // disable start menu
        panelStartUI.SetActive(false);

        // show minimap
        panelMiniMap.SetActive(true);

        // show ScoreUI
        panelScore.SetActive(true);

        //show CollectableUI
        CollectableUI.SetActive(true);

        // show camera target cross
        panelCameraTarget.SetActive(true);
    }


    /****
     * This function is called from an Event Trigger from PanelStart
     ****/
    public void StartGame()
    {
        // fade out and disable UI start menu after x seconds
        StartCoroutine(FadeOutStartPanelUI());
    }

    private IEnumerator ExitAppAfterSeconds(float seconds)
    {
        //Debug.LogError("NO USES ESTA OPCIÓN");
        yield return new WaitForSecondsRealtime(seconds); //XXX Realtime is not affected by Time.timeScale
        Application.Quit();
    }

}
