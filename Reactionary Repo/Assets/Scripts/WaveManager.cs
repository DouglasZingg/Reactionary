using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/***********************************************
 * Filename:  		WaveManager.cs
 * Date:      		12/07/2020
 * Mod. Date: 		12/09/2020
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to handle transitions between wave
 *                  states and spawning the dynamically created waves.
 ************************************************/
public enum eSpawnerIndices
{
    Random
}

public enum eEnemies
{
    HeadBanger, FireHeadBanger, IceHeadBanger, LightningHeadBanger, FireArms, BatBlood
}

public enum eWaveState
{
    Downtime, WaveIntro, WaveStart, WaveRunning, WaveOutro
}

//-------------------
//Wave Manager Code
//Must assign Enemy prefabs and Spawn Location GameObjects to the Unity Inspector
//Will work without Data Manager for first wave.
//-------------------
public class WaveManager : MonoBehaviour
{
    //Singleton
    public static WaveManager g_cInstance = null;

    //Contents containers
    public TWaveContents m_tWaveContents;
    private PlayerStatScript m_cPlayerStats;
    private SaveGameData m_cSavedGameData;
    [SerializeField] private bool m_bAutoSaveEnabled;

    //Wavestate transition variables
    [SerializeField] private float m_fDowntimeDuration;
    private float m_fDowntimeRemaining;
    [SerializeField] private float m_fIntroDuration;
    private float m_fIntroRemaining;
    [SerializeField] private float m_fOutroDuration;
    private float m_fOutroRemaining;
    [SerializeField] private float m_fStartDuration;
    private float m_fStartRemaining;

    //wave traits
    public int m_nCurrentWave { get; set; }
    public float m_fCurrentWaveTime { get; set; }
    public eWaveState m_eWaveState { get; set; }
    public int m_nEnemyCountOnMap { get; set; }

    //Spawn points
    [SerializeField] private Transform[] m_EnemySpawnPoints = new Transform[5];

    //Enemies available
    [SerializeField] private GameObject[] m_EnemyTypes = new GameObject[1];

    //which wave ends the game
    [SerializeField] private int m_nFinalWave;

    //tracks time between unit spawns
    private float m_fSpawnRateTimer;

    public SaveGameData GetCurrentSaveData()
    {
        return m_cSavedGameData;
    }

    public void EnemyDied()
    {
        //remove the enemy
        m_tWaveContents.nEnemyCountRemaining--;
        m_nEnemyCountOnMap--;

        //Tell ScoreManager
        ScoreManager.g_cInstance.EnemyDied();

        Debug.Log("Enemies remaining: " + m_tWaveContents.nEnemyCountRemaining);
        Debug.Log("Enemies on map: " + m_nEnemyCountOnMap);
    }

    //initialize default variables on game start
    private void Start()
    {
        m_cPlayerStats = GameObject.Find("PlayerHands").GetComponentInChildren<PlayerStatScript>();
        NullCheckScript.NullCheckElseError(m_cPlayerStats, "Wave Manager could not find PlayerStatScript");

        m_nCurrentWave = 1;
        m_eWaveState = eWaveState.Downtime;
        m_fCurrentWaveTime = 0.0f;

        m_fDowntimeRemaining = m_fDowntimeDuration;
        m_fIntroRemaining = m_fIntroDuration;
        m_fOutroRemaining = m_fOutroDuration;
        m_fStartRemaining = m_fStartDuration;

        //Load up a default wave for starting the game
        m_tWaveContents = DynamicAIWaves.g_cInstance.SetInitialWave();

        m_fSpawnRateTimer = m_tWaveContents.fSpawnRate;
        m_nEnemyCountOnMap = 0;

        m_cSavedGameData = new SaveGameData(m_cPlayerStats.GetPlayerMaxHP(), m_cPlayerStats.GetMaxMana());

        UIManager.g_cInstance.SetDowntimeTimerIsActive(true);
    }

    public void LoadGame(SaveGameData gameData)
    {
        m_cSavedGameData = gameData;

        m_cPlayerStats.SetCurrentHP(gameData.fCurrentHealth);
        m_cPlayerStats.SetCurrentMana(gameData.fCurrentMana);

        m_eWaveState = eWaveState.Downtime;
        m_fDowntimeRemaining = m_fDowntimeDuration;
        UIManager.g_cInstance.SetDowntimeTimerIsActive(true);
        m_fCurrentWaveTime = 0.0f;

        ObjectPool.g_cInstance.ResetAll();

        if (m_nCurrentWave == 1)
        {
            m_tWaveContents = DynamicAIWaves.g_cInstance.SetInitialWave();
        }
        else
        {
            m_tWaveContents = DynamicAIWaves.g_cInstance.CalculateNewWave();
        }
        UIManager.g_cInstance.UpdateAIStatsUI();

        m_fSpawnRateTimer = m_tWaveContents.fSpawnRate;
        m_nEnemyCountOnMap = 0;

        DataManager.g_cInstance.ZeroOut();

        UIManager.g_cInstance.SetWaveCompleteIsActive(false);
        UIManager.g_cInstance.SetWaveStartingIsActive(false);
    }

    //Update called every frame
    private void Update()
    {
        //determine the wavestate
        switch (m_eWaveState)
        {
            //if in downtime, count down to wave intro
            case eWaveState.Downtime:
                {
                    m_fDowntimeRemaining -= Time.deltaTime;

                    //If downtime is over, transition to intro
                    if (m_fDowntimeRemaining <= 0)
                    {
                        Debug.Log("Downtime is over, going to intro");
                        m_fDowntimeRemaining = m_fDowntimeDuration;
                        m_eWaveState = eWaveState.WaveIntro;

                        UIManager.g_cInstance.SetDowntimeTimerIsActive(false);
                    }
                }
                break;
            
            //if in intro, count down to wave start
            case eWaveState.WaveIntro:
                {
                    m_fIntroRemaining -= Time.deltaTime;

                    //If intro is over, transition to start
                    if (m_fIntroRemaining <= 0)
                    {
                        UIManager.g_cInstance.SetWaveStartingIsActive(true);
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.WaveStarted);

                        Debug.Log("Intro is over, going to start");
                        m_fIntroRemaining = m_fIntroDuration;
                        m_eWaveState = eWaveState.WaveStart;
                    }
                }
                break;

            //if in wave start, count down to wave running
            case eWaveState.WaveStart:
                {
                    m_fStartRemaining -= Time.deltaTime;

                    //If start is over, transition to running
                    if (m_fStartRemaining <= 0)
                    {
                        UIManager.g_cInstance.SetWaveStartingIsActive(false);
                        Debug.Log("Start is over, going to running");
                        m_fStartRemaining = m_fStartDuration;
                        m_eWaveState = eWaveState.WaveRunning;
                    }
                }
                break;

            //if the wave is running, keep spawning enemies
            case eWaveState.WaveRunning:
                {
                    //Update how long the wave has been running
                    m_fCurrentWaveTime += Time.deltaTime;

                    //if there are no more enemies to spawn or kill
                    if (m_tWaveContents.nEnemyCountRemaining <= 0)
                    {
                        //if this was the final wave, end the game
                        if (m_nCurrentWave >= m_nFinalWave)
                        {
                            EndGame();
                        }
                        //else, end this wave, calculate next wave, and start outro
                        else
                        {
                            //display the wave complete screen
                            UIManager.g_cInstance.SetWaveCompleteIsActive(true);
                            SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.WaveCompleted);

                            //increment the wave number
                            m_nCurrentWave++;

                            //reset current wave timer
                            DataManager.g_cInstance.m_fTimeTakenToClearWave = m_fCurrentWaveTime;
                            m_fCurrentWaveTime = 0.0f;

                            if (m_bAutoSaveEnabled)
                            {
                                //save game after each wave
                                m_cSavedGameData = new SaveGameData(m_cPlayerStats.GetCurrentHP(), m_cPlayerStats.GetCurrentMana());
                                SaveSystem.SaveGame(m_cSavedGameData);
                                UIManager.g_cInstance.SetSaveGameFeedbackIsActive(true, "Game AutoSaved!");
                            }

                            //get new wave
                            m_tWaveContents = DynamicAIWaves.g_cInstance.CalculateNewWave();

                            //Display new AI stats
                            UIManager.g_cInstance.UpdateAIStatsUI();

                            //reset Data Manager
                            DataManager.g_cInstance.ZeroOut();

                            Debug.Log("Running is over, going to outro");
                            m_eWaveState = eWaveState.WaveOutro;
                        }

                    }
                    //else, there are enemies remaining in the wave
                    else
                    {
                        //if not ready to spawn, continue counting down.
                        if (m_fSpawnRateTimer > 0)
                        {
                            m_fSpawnRateTimer -= Time.deltaTime;
                        }

                        //else if ready to spawn and the maximum enemy count on the map has not been exceeded, spawn one
                        else if (m_fSpawnRateTimer <= 0 
                            && m_nEnemyCountOnMap < m_tWaveContents.nMaxEnemyCountOnMap
                            && m_nEnemyCountOnMap < m_tWaveContents.nEnemyCountRemaining)
                        {
                            m_nEnemyCountOnMap++;

                            //Spawn the next enemy in line
                            SpawnNextEnemy();

                            //reset spawn timer
                            m_fSpawnRateTimer = m_tWaveContents.fSpawnRate;
                        }

                    }
                }
                break;

            //if the wave ends, allow time for saving and result screen, then transition to downtime
            case eWaveState.WaveOutro:
                {
                    m_fOutroRemaining -= Time.deltaTime;

                    //If outro is over, transition to downtime
                    if (m_fOutroRemaining <= 0)
                    {
                        Debug.Log("Outro is over, going to downtime");
                        m_fOutroRemaining = m_fOutroDuration;
                        m_eWaveState = eWaveState.Downtime;

                        //turn off the UI Wave complete screen
                        UIManager.g_cInstance.SetWaveCompleteIsActive(false);
                        UIManager.g_cInstance.SetDowntimeTimerIsActive(true);
                    }
                }
                break;

            default:
                Debug.Log("No Wave State");
                break;
        }
    }

    //Dequeues the enemies queue in the Wave Contents to spawn the next enemy in line.
    void SpawnNextEnemy()
    {
        LoadedEnemy tNextEnemy = m_tWaveContents.tLoadedEnemies.Dequeue();

        //spawn an enemy at a random location
        if (tNextEnemy.eSpawnerIndex == eSpawnerIndices.Random)
        {
            int nSpawnIndex = Random.Range(0, m_EnemySpawnPoints.Length);
            GameObject newHeadBangerType = null;

            //Determine which kind of enemy to grab from the object pool
            //set position to spawers position and set the enemy as active
            switch (tNextEnemy.eType)
            {
                case eEnemies.HeadBanger:
                    {
                        newHeadBangerType = ObjectPool.g_cInstance.GetHeadBanger();
                    }
                    break;
                case eEnemies.FireHeadBanger:
                    {
                        newHeadBangerType = ObjectPool.g_cInstance.GetFireHeadBanger();
                    }
                    break;
                case eEnemies.IceHeadBanger:
                    {
                        newHeadBangerType = ObjectPool.g_cInstance.GetIceHeadBanger();
                    }
                    break;
                case eEnemies.LightningHeadBanger:
                    {
                        newHeadBangerType = ObjectPool.g_cInstance.GetLightningHeadBanger();
                    }
                    break;
                case eEnemies.FireArms:
                    break;
                case eEnemies.BatBlood:
                    break;
                default:
                    break;   
            }

            if (newHeadBangerType != null)
            {
                //Set the Position and Rotation to the appropriate location
                newHeadBangerType.transform.position = m_EnemySpawnPoints[nSpawnIndex].position;
                newHeadBangerType.transform.rotation = m_EnemySpawnPoints[nSpawnIndex].rotation;

                //Activate enemy
                newHeadBangerType.SetActive(true);

                //Adjust traits
                newHeadBangerType.GetComponent<EnemyAIScript>().enabled = true;
                newHeadBangerType.GetComponent<NavMeshAgent>().enabled = true;

                EnemyStatScript enemyStatScript = newHeadBangerType.GetComponent<EnemyStatScript>();
                enemyStatScript.SetGenericResistance(tNextEnemy.fGenericResistance);
                enemyStatScript.SetFireResistance(tNextEnemy.fFireResistance);
                enemyStatScript.SetIceResistance(tNextEnemy.fIceResistance);
                enemyStatScript.SetLightningResistance(tNextEnemy.fLightningResistance);
            }
            else
            {
                Debug.Log("Object Pool HeadBangerType request failed, gameobject was null");
            }
        }
    }

    //called after final wave is completed
    private void EndGame()
    {
        Debug.LogError("The final wave was cleared");
    }

    public int GetDowntimeTimer()
    {
        return (int)m_fDowntimeRemaining;
    }

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

    #region SkipWaveState
    public void SkipWaveState()
    {
        //determine the wavestate
        switch (m_eWaveState)
        {
            //if in downtime, skip to wave intro
            case eWaveState.Downtime:
                {
                    Debug.Log("Skipping downtime, going to intro");
                    m_fDowntimeRemaining = m_fDowntimeDuration;
                    m_eWaveState = eWaveState.WaveIntro;
                    UIManager.g_cInstance.SetDowntimeTimerIsActive(false);
                }
                break;

            //if in intro, skip to wave start
            case eWaveState.WaveIntro:
                {
                    Debug.Log("Skipping intro, going to start");
                    UIManager.g_cInstance.SetWaveStartingIsActive(true);
                    SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.WaveStarted);
                    m_fIntroRemaining = m_fIntroDuration;
                    m_eWaveState = eWaveState.WaveStart;
                }
                break;

            //if in wave start, skip to wave running
            case eWaveState.WaveStart:
                {
                    UIManager.g_cInstance.SetWaveStartingIsActive(false);
                    Debug.Log("Skipping start, going to running");
                    m_fStartRemaining = m_fStartDuration;
                    m_eWaveState = eWaveState.WaveRunning;
                }
                break;

            //if the wave is running, keep spawning enemies
            case eWaveState.WaveRunning:
                {
                    //if this was the final wave, end the game
                    if (m_nCurrentWave >= m_nFinalWave)
                    {
                        EndGame();
                    }
                    //else, end this wave, calculate next wave, and start outro
                    else
                    {
                        //display the wave complete screen
                        UIManager.g_cInstance.SetWaveCompleteIsActive(true);
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.WaveCompleted);

                        //increment the wave number
                        m_nCurrentWave++;

                        //reset current wave timer
                        DataManager.g_cInstance.m_fTimeTakenToClearWave = m_fCurrentWaveTime;
                        m_fCurrentWaveTime = 0.0f;

                        if (m_bAutoSaveEnabled)
                        {
                            //save game after each wave
                            m_cSavedGameData = new SaveGameData(m_cPlayerStats.GetCurrentHP(), m_cPlayerStats.GetCurrentMana());
                            SaveSystem.SaveGame(m_cSavedGameData);
                            UIManager.g_cInstance.SetSaveGameFeedbackIsActive(true, "Game AutoSaved!");
                        }

                        //get new wave
                        m_tWaveContents = DynamicAIWaves.g_cInstance.CalculateNewWave();

                        //Display new AI stats
                        UIManager.g_cInstance.UpdateAIStatsUI();

                        //reset Data Manager
                        DataManager.g_cInstance.ZeroOut();

                        Debug.Log("Skipping running, going to outro");
                        ObjectPool.g_cInstance.ResetAll();
                        m_eWaveState = eWaveState.WaveOutro;
                    }
                }
                break;

            //if the wave ends, allow time for saving and result screen, then transition to downtime
            case eWaveState.WaveOutro:
                {
                    Debug.Log("Skipping Outro, going to downtime");
                    m_fOutroRemaining = m_fOutroDuration;
                    m_eWaveState = eWaveState.Downtime;

                    //turn off the UI Wave complete screen
                    UIManager.g_cInstance.SetWaveCompleteIsActive(false);
                    UIManager.g_cInstance.SetDowntimeTimerIsActive(true);
                }
                break;

            default:
                Debug.Log("No Wave State");
                break;
        }
    }
    #endregion
}