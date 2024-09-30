using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ProBuilder2.Common;

public class UIManager : MonoBehaviour
{
    //Member variables that require serialization
    [SerializeField] private GameObject g_pauseMenu = null;
    [SerializeField] private GameObject g_resultMenu = null;
    [SerializeField] private GameObject g_healthUI = null;
    [SerializeField] private GameObject g_deathUI = null;
    [SerializeField] private Text t_gameStats = null;
    [SerializeField] private Text t_AIStats = null;
    [SerializeField] private Text t_waveCompleteNumber = null;
    [SerializeField] private Text t_waveStartingNumber = null;
    [SerializeField] private Text m_cUIResultsText = null;
    [SerializeField] private Text m_cheatCodeGodModeText = null;
    [SerializeField] private Text m_cSaveGameFeedback = null;
    [SerializeField] private Text m_cDowntimeTimerText = null;
    [SerializeField] private Text t_deathScoreText = null;
    [SerializeField] private Image[] i_statusImages = null;
    [SerializeField] private Image[] i_enemyTypeImages = null;
    [SerializeField] private GameObject g_firePowerhUI = null;
    [SerializeField] private GameObject g_icePowerUI = null;
    [SerializeField] private GameObject g_lightningPowerUI = null;
    [SerializeField] private PlayerStatScript m_playerStat = null;
    [SerializeField] private HealthBar m_healthBar = null;
    [SerializeField] private AudioSource a_gameMusic = null;

    //Other member variables
    private GameObject g_playerHands = null;
    public bool b_isPaused = false;
    private bool b_resultActive = false;
    private bool m_bPlayerIsDead = false;
    private FPSCameraScript m_CameraScript = null;
    public static UIManager g_cInstance = null;

    //Sets up the Singleton
    private void Awake()
    {
        if (g_cInstance == null)
        {
            g_cInstance = this;
        }
        else if (g_cInstance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1;

        g_playerHands = GameObject.Find("PlayerHands");
        if (NullCheckScript.NullCheckElseError(g_playerHands, "Player hands was null on UI Manager Start"))
        {
            m_CameraScript = g_playerHands.GetComponent<FPSCameraScript>();
            if(NullCheckScript.NullCheckElseError(m_CameraScript, "Player hands' Camera Script was null on UI Manager Start"))
            {
                m_CameraScript.enabled = true;
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!m_bPlayerIsDead)
        {
            UpdateUI();

            //if in downtime, update UI timer
            if (WaveManager.g_cInstance.m_eWaveState == eWaveState.Downtime)
            {
                UpdateDowntimeUI(WaveManager.g_cInstance.GetDowntimeTimer());
            }

            if (Input.GetKeyDown(KeyCode.Escape) && NullCheckScript.NullCheckElseError(g_playerHands, "UI Manager could not find PlayerHands in Update()"))
            {
                GameObject g_pausedOptionsMenu = GameObject.Find("Paused Options Menu");
                if (NullCheckScript.NullCheckElseError(g_pausedOptionsMenu, "Paused Options Menu could not be found or is null"))
                {
                    if (!g_pausedOptionsMenu.transform.GetChild(0).gameObject.activeSelf)
                    {
                        b_isPaused = !b_isPaused;
                    }
                }
            }

            if (b_isPaused)
                ActivateMenu();

            if (m_playerStat.g_bGodMode == true)
                m_cheatCodeGodModeText.color = Color.red;
            else
                m_cheatCodeGodModeText.color = Color.black;

            if (m_playerStat.m_nStatus == 0)
                i_statusImages[0].color = Color.white;
            else
                i_statusImages[0].color = Color.black;

            if (m_playerStat.m_nStatus == 1)
                i_statusImages[1].color = Color.white;
            else
                i_statusImages[1].color = Color.black;

            if (m_playerStat.m_nStatus == 2)
                i_statusImages[2].color = Color.white;
            else
                i_statusImages[2].color = Color.black;

            if (m_playerStat.m_nStatus == 3)
                i_statusImages[3].color = Color.white;
            else
                i_statusImages[3].color = Color.black;

            if (m_playerStat.m_nStatus == 0)
                i_enemyTypeImages[3].color = Color.white;
            else
                i_enemyTypeImages[3].color = Color.black;

            if (m_playerStat.m_nStatus == 1)
                i_enemyTypeImages[0].color = Color.white;
            else
                i_enemyTypeImages[0].color = Color.black;

            if (m_playerStat.m_nStatus == 2)
                i_enemyTypeImages[1].color = Color.white;
            else
                i_enemyTypeImages[1].color = Color.black;

            if (m_playerStat.m_nStatus == 3)
                i_enemyTypeImages[2].color = Color.white;
            else
                i_enemyTypeImages[2].color = Color.black;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                b_resultActive = !b_resultActive;
                if (NullCheckScript.NullCheckElseError(g_resultMenu, "Results menu is null in UI Manager"))
                {
                    g_resultMenu.SetActive(b_resultActive);
                }
            }

            if (NullCheckScript.NullCheckElseError(m_healthBar, "Health bar is null in UI Manager"))
            {
                if (m_healthBar.health <= 0)
                {
                    if (m_bPlayerIsDead == false)
                    {
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerDied);
                        m_bPlayerIsDead = true;
                    }

                    Time.timeScale = 0;

                    if (NullCheckScript.NullCheckElseError(m_CameraScript, "UI Manager could not find FPSCameraScript on Player Death"))
                    {
                        m_CameraScript.enabled = false;
                    }

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    if (NullCheckScript.NullCheckElseError(g_healthUI, "Health UI was null in UI Manager on Player death"))
                    {
                        g_healthUI.SetActive(false);
                    }
                    if (NullCheckScript.NullCheckElseError(g_deathUI, "Death UI was null in UI Manager on Player death"))
                    {
                        g_deathUI.SetActive(true);
                    }

                    t_deathScoreText.text = "SCORE: " + ScoreManager.g_cInstance.m_nCurrentScore;

                    if (NullCheckScript.NullCheckElseError(a_gameMusic, "AudioSource gameMusic was null in UI Manager on Player death"))
                    {
                        a_gameMusic.Stop();
                    }
                }
            }
        }
        
    }
    public void SetMenuIsActive(bool active)
    {
        g_pauseMenu.SetActive(active);
        Cursor.visible = active;

        if (active)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            m_CameraScript.enabled = false;

        }
        else
        {
            Time.timeScale = 1;
            b_isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_CameraScript.enabled = true;
        }

        GameObject g_gameUI = GameObject.Find("HealthManaSkillsUI");
        if (NullCheckScript.NullCheckElseError(g_gameUI, "UI Manager's SetMenuIsActive() could not find HealthManaSkillsUI"))
        {
            g_gameUI.transform.GetChild(6).gameObject.SetActive(!active);
        }
    }

    void ActivateMenu()
    {
        SetMenuIsActive(true);
    }

    public void DeactivateMenu()
    {
        SetMenuIsActive(false);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Level Scene");
    }

    public void BlogButtion()
    {
        string URL = "https://capstonecold.wordpress.com/";
        Application.OpenURL(URL);
    }

    public void ExitButton()
    {
        //Commenting this out because it might be breaking our build -Petra
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SaveButton()
    {
        SaveSystem.SaveGame(WaveManager.g_cInstance.GetCurrentSaveData());
        SetSaveGameFeedbackIsActive(true, "Game Saved!");
    }

    public void LoadButton()
    {
        SaveSystem.LoadGame();
        SetSaveGameFeedbackIsActive(true, "Game Loaded!");
    }

    public void PausedExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MenuSwitch(string sz_menuNameOne, string sz_menuNameTwo, bool b_activeState)
    {
        GameObject g_menuOne = GameObject.Find(sz_menuNameOne);
        GameObject g_menuTwo = GameObject.Find(sz_menuNameTwo);

        if (NullCheckScript.NullCheckElseError(g_menuOne, "UI Manager's MenuSwitch() could not find menu " + sz_menuNameOne + " or that menu was null") && 
            NullCheckScript.NullCheckElseError(g_menuTwo, "UI Manager's MenuSwitch() could not find menu " + sz_menuNameTwo + " or that menu was null")) 
        { 
            for (int i = 0; i < g_menuOne.transform.childCount; i++)
            {
                g_menuOne.transform.GetChild(i).gameObject.SetActive(b_activeState);
            }

            for (int i = 0; i < g_menuTwo.transform.childCount; i++)
            {
                g_menuTwo.transform.GetChild(i).gameObject.SetActive(!b_activeState);
            }
        }
    }

    public void OptionButton()
    {
        MenuSwitch("Options Menu", "Main Menu", true);
    }

    public void OptionExitButton()
    {
        MenuSwitch("Main Menu", "Options Menu", true);
    }
    public void PausedResumeButton()
    {
        MenuSwitch("HeathManaSkillsUI", "Pause Menu", true);
    }

    public void PausedOptionsButton()
    {
        MenuSwitch("Paused Options Menu", "Pause Menu", true);
    }

    public void PausedOptionsExitButton()
    {
        MenuSwitch("Pause Menu", "Paused Options Menu", true);
    }

    public void HowToPlayExitButton()
    {
        MenuSwitch("Main Menu", "How To Play", true);
    }

    public void HowToPlayButton()
    {
        MenuSwitch("How To Play", "Main Menu", true);
    }

    public void CreditsButton()
    {
        MenuSwitch("Credits", "Main Menu", true);
    }
    public void CreditsExitButton()
    {
        MenuSwitch("Main Menu", "Credits", true);
    }

    public void ResultsNextLevel()
    {
        MenuSwitch("HeathManaSkillsUI", "Results", true);
    }

    private void UpdateUI()
    {
        if(NullCheckScript.NullCheckElseError(ScoreManager.g_cInstance, "UIManager's UpdateUI() could not find ScoreManager.g_cInstance()") && 
            NullCheckScript.NullCheckElseError(WaveManager.g_cInstance, "UIManager's UpdateUI() could not find WaveManager.g_cInstance()"))
        {
            t_gameStats.text = "Wave Num: " + WaveManager.g_cInstance.m_nCurrentWave + "\n"
            + "Num of Enemies: " + WaveManager.g_cInstance.m_tWaveContents.nEnemyCountRemaining + "\n"
            + "Score: " + ScoreManager.g_cInstance.m_nCurrentScore + "\n"
            + "Kill Streak: " + ScoreManager.g_cInstance.m_nKillStreak + "\n"
            + "Multi-Kill Multiplier: " + ScoreManager.g_cInstance.m_nMultiKillMultiplier;

            m_cUIResultsText.text = "Total Kills: " + ScoreManager.g_cInstance.m_nTotalKills + "\n"
            + "Wave Number: " + WaveManager.g_cInstance.m_nCurrentWave + "\n"
            + "Challenge Points: 0 \n"
            + "Total Score : " + ScoreManager.g_cInstance.m_nTotalScore + "\n"
            + "Kill Streak : " + ScoreManager.g_cInstance.m_nKillStreak + "\n";
        }
    }

    public void UpdateAIStatsUI()
    {
        if (NullCheckScript.NullCheckElseError(DynamicAIWaves.g_cInstance, "UIManager's UpdateAIStatsUI() could not find DynamicAIWaves.g_cInstance()"))
        {
            t_AIStats.text = "Horde:" + DynamicAIWaves.g_cInstance.m_fHordeAmountRatio + "\n" +
            "Aggression:" + DynamicAIWaves.g_cInstance.m_fAggressionRatio + "\n" +
            "Fire Res:" + DynamicAIWaves.g_cInstance.m_fFireResistanceRatio + "\n" +
            "Cold Res:" + DynamicAIWaves.g_cInstance.m_fColdResistanceRatio + "\n" +
            "Lightning Res:" + DynamicAIWaves.g_cInstance.m_fLightningResistanceRatio;
        }
    }

    private void UpdateDowntimeUI(int time)
    {
        if (NullCheckScript.NullCheckElseError(m_cDowntimeTimerText, "UIManager's UpdateDowntimeUI() could not find m_cDowntimeTimerText"))
        {
            m_cDowntimeTimerText.text = time + "";
        }
    }

    public void ReloadLevelScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void SetWaveCompleteIsActive(bool active)
    {
        GameObject g_waveComplete = GameObject.Find("WaveComplete");
        if(NullCheckScript.NullCheckElseError(g_waveComplete, "UIManager's SetWaveCompleteIsActive() could not find WaveComplete or WaveComplete is null"))
        {
            for (int i = 0; i < g_waveComplete.transform.childCount; i++)
            {
                g_waveComplete.transform.GetChild(i).gameObject.SetActive(active);
            }
        }
        if(active)
        {
            t_waveCompleteNumber.text = WaveManager.g_cInstance.m_nCurrentWave + "";
        }
    }

    public void SetWaveStartingIsActive(bool active)
    {
        GameObject g_waveStarting = GameObject.Find("WaveStarting");
        if (NullCheckScript.NullCheckElseError(g_waveStarting, "UIManager's SetWaveCompleteIsActive() could not find WaveStarting or WaveStarting is null"))
        {
            for (int i = 0; i < g_waveStarting.transform.childCount; i++)
            {
                g_waveStarting.transform.GetChild(i).gameObject.SetActive(active);
            }
        }
        if (active)
        {
            t_waveStartingNumber.text = WaveManager.g_cInstance.m_nCurrentWave + "";
        }
    }

    public void SetSaveGameFeedbackIsActive(bool active, string message)
    {
        GameObject g_saveGameFeedback = GameObject.Find("SaveGameFeedback");
        if (NullCheckScript.NullCheckElseError(g_saveGameFeedback, "UIManager's SetSaveGameFeedbackIsActive() could not find SaveGameFeedback or SaveGameFeedback is null"))
        {
            for (int i = 0; i < g_saveGameFeedback.transform.childCount; i++)
            {
                g_saveGameFeedback.transform.GetChild(i).gameObject.SetActive(active);
            }
        }
        if (active)
        {
            m_cSaveGameFeedback.text = message;
            Invoke("TurnOffSaveGameFeedback", 5.0f);
        }
    }
    private void TurnOffSaveGameFeedback()
    {
        SetSaveGameFeedbackIsActive(false, "Game Saved!");
    }

    public void SetDowntimeTimerIsActive(bool active)
    {
        GameObject g_downtimeTimer = GameObject.Find("DowntimeTimer");
        if (NullCheckScript.NullCheckElseError(g_downtimeTimer, "UIManager's SetDowntimeTimerIsActive() could not find DowntimeTimer or DowntimeTimer is null"))
        {
            for (int i = 0; i < g_downtimeTimer.transform.childCount; i++)
            {
                g_downtimeTimer.transform.GetChild(i).gameObject.SetActive(active);
            }
        }
    }

    public void SwitchPowers(int whichPower)
    {
        if(whichPower == 0)
        {
            g_firePowerhUI.SetActive(true);
            g_icePowerUI.SetActive(false);
            g_lightningPowerUI.SetActive(false);
        }
        if (whichPower == 1)
        {
            g_firePowerhUI.SetActive(false);
            g_icePowerUI.SetActive(false);
            g_lightningPowerUI.SetActive(true);
        }
        if (whichPower == 2)
        {
            g_firePowerhUI.SetActive(false);
            g_icePowerUI.SetActive(true);
            g_lightningPowerUI.SetActive(false);
        }
    }
}
