using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		DynamicAIWaves.cs
 * Date:      		12/09/2020
 * Mod. Date: 		1/18/2021
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to handle creation of dynamic wave contents.
 *                  Determines wave traits and values by Data Manager.
 ************************************************/

 //struct for queuing enemies for the wave manager to spawn. 
public struct LoadedEnemy
{
    public eEnemies eType;
    public eSpawnerIndices eSpawnerIndex;
    public float fGenericResistance;
    public float fFireResistance;
    public float fIceResistance;
    public float fLightningResistance;
    //public float fMovementSpeed;
    //public float fAttackSpeed;
    //public float fFireResistance;
}

//struct being passed to the Wave Manager for spawning enemies
public struct TWaveContents
{
    public float fSpawnRate;
    public int nEnemyCountRemaining;
    public int nMaxEnemyCountOnMap;
    public Queue<LoadedEnemy> tLoadedEnemies;
}

//Script that dynamically controls wave contents.
public class DynamicAIWaves : MonoBehaviour
{
    //Singleton
    public static DynamicAIWaves g_cInstance = null;

    //Unity Inspector fields for default wave
    [SerializeField] private float m_fDefaultSpawnRate;
    [SerializeField] private int m_nDefaultEnemyCountRemaining;
    [SerializeField] private int m_nDefaultMaxEnemyCountOnMap;

    //Member variables
    private Queue<LoadedEnemy> m_tPreviousQueue;
    public float m_fPreviousSpawnRate { get; set; }
    public int m_nPreviousEnemyCountRemaining { get; set; }
    public int m_nPreviousMaxEnemyCountOnMap { get; set; }

    [SerializeField] private float m_fTargetWaveTime;

    /*each approach ratio starts at 0 and will increase each wave by how much data was learned (Added) this wave.
    ex: If the player never uses fireballs, it will remain at 0 and no enemies will be resistant to fire.
    If any ratio is 1, All enemies will spawn with that trait.*/

    public float m_fFireResistanceRatio { get; set; } = 0.0f;
    public float m_fFireResistanceChanged { get; set; } = 0.0f;

    public float m_fColdResistanceRatio { get; set; } = 0.0f;
    public float m_fColdResistanceChanged { get; set; } = 0.0f;

    public float m_fLightningResistanceRatio { get; set; } = 0.0f;
    public float m_fLightningResistanceChanged { get; set; } = 0.0f;

    public float m_fAggressionRatio { get; set; } = 0.0f;
    public float m_fAggressionChanged { get; set; } = 0.0f;

    public float m_fHordeAmountRatio { get; set; } = 0.0f;
    public float m_fHordeAmountChanged { get; set; }= 0.0f;

    //Determines which immunities are availble based on Resistance ratios reaching 1
    private eEnemies GetEnemyType()
    {
        List<eEnemies> ImmunityTypes = new List<eEnemies>();
        ImmunityTypes.Add(eEnemies.HeadBanger);

        if (m_fFireResistanceRatio == 1.0f)
        {
            ImmunityTypes.Add(eEnemies.FireHeadBanger);
        }
        if (m_fColdResistanceRatio == 1.0f)
        {
            ImmunityTypes.Add(eEnemies.IceHeadBanger);
        }
        if (m_fLightningResistanceRatio == 1.0f)
        {
            ImmunityTypes.Add(eEnemies.LightningHeadBanger);
        }

        int index = Random.Range(0, ImmunityTypes.Count);
        return ImmunityTypes[index];
    }

    //sets the first wave so the rest can learn off a baseline
    public TWaveContents SetInitialWave()
    {
        //set up a default wave as a baseline
        TWaveContents tDefaultWave = new TWaveContents
        {
            fSpawnRate = m_fDefaultSpawnRate,
            nEnemyCountRemaining = m_nDefaultEnemyCountRemaining,
            nMaxEnemyCountOnMap = m_nDefaultMaxEnemyCountOnMap,
            tLoadedEnemies = new Queue<LoadedEnemy>()
        };

        //load up a default queue
        for (int i = 0; i < tDefaultWave.nEnemyCountRemaining; i++)
        {
            LoadedEnemy tEnemy = new LoadedEnemy
            {
                eType = GetEnemyType(),
                eSpawnerIndex = eSpawnerIndices.Random,
                fGenericResistance = 1.0f,
                fFireResistance = 1.0f,
                fIceResistance = 1.0f,
                fLightningResistance = 1.0f
            };

            tDefaultWave.tLoadedEnemies.Enqueue(tEnemy);
        }

        //set the previous wave as this default
        m_fPreviousSpawnRate = m_fDefaultSpawnRate;
        m_nPreviousEnemyCountRemaining = m_nDefaultEnemyCountRemaining;
        m_nPreviousMaxEnemyCountOnMap = m_nDefaultMaxEnemyCountOnMap;
        m_tPreviousQueue = tDefaultWave.tLoadedEnemies;

        return tDefaultWave;
    }

    /*****************************************************************
    * CalculateNewWave()      Determines a new wave approach by calling the Data Manager
    *                         and getting the results of the previous wave.
    *                         
    * Ins:			          none
    *      		              
    * Outs:		              New wave contents for the Wave Manager to spawn.
    *
    * Returns:		          TWaveContents
    *
    * Mod. Date:		      12/11/2020
    * Mod. Initials:	      KK
    *****************************************************************/
    public TWaveContents CalculateNewWave()
    {
        TWaveContents tNewWave = new TWaveContents();

        //get the most recent data from the Data Manager and consider this data to adjust approach.
        AdjustApproach();

        //Determine the amount of enemies that we should make
        //Aggression * Horde * previous enemy count
        tNewWave.nEnemyCountRemaining = Mathf.Clamp(m_nPreviousEnemyCountRemaining + 
            (int)(m_fHordeAmountRatio * m_fAggressionRatio * m_nPreviousEnemyCountRemaining), 10, 50);

        //Determine how many enemies should be allowed to be on the map at one time
        //Aggression * Horde * previous max count
        tNewWave.nMaxEnemyCountOnMap = m_nPreviousMaxEnemyCountOnMap +
            (int)(m_fHordeAmountRatio * m_nPreviousMaxEnemyCountOnMap);

        //Determine how fast enemies should spawn
        //Horde * previous max count
        tNewWave.fSpawnRate = m_fPreviousSpawnRate -
            (m_fHordeAmountRatio * m_fPreviousSpawnRate);

        Debug.Log("Prior Wave time: " + DataManager.g_cInstance.m_fTimeTakenToClearWave + " Next Wave: Enemies: " + tNewWave.nEnemyCountRemaining +
        " Max on Map: " + tNewWave.nMaxEnemyCountOnMap +
        " Spawn Rate: " + tNewWave.fSpawnRate);

        /* Potential Ideas
        
        Determine the types of enemies that work well against the player.
        Determine ideal spawn points player doesn't go to very often.

        */

        //LOAD THE QUEUE
        tNewWave.tLoadedEnemies = new Queue<LoadedEnemy>();

        for (int i = 0; i < tNewWave.nEnemyCountRemaining; i++)
        {
            LoadedEnemy tEnemy = new LoadedEnemy
            {
                //Only have HeadBangers at the moment
                //Determine if any immunity should be applied
                eType = GetEnemyType(),
                //Random spawn point at the moment
                eSpawnerIndex = eSpawnerIndices.Random,
                fGenericResistance = 1.0f + m_fAggressionRatio,
                fFireResistance = 1.0f + m_fFireResistanceRatio,
                fIceResistance = 1.0f + m_fColdResistanceRatio,
                fLightningResistance = 1.0f + m_fLightningResistanceRatio
            };

            tNewWave.tLoadedEnemies.Enqueue(tEnemy);
        }

        Debug.Log("New Wave Generated");
        //save this wave as the previous wave
        m_fPreviousSpawnRate = tNewWave.fSpawnRate;
        m_nPreviousEnemyCountRemaining = tNewWave.nEnemyCountRemaining;
        m_nPreviousMaxEnemyCountOnMap = tNewWave.nMaxEnemyCountOnMap;
        m_tPreviousQueue = tNewWave.tLoadedEnemies;

        return tNewWave;
    }

    /*****************************************************************
    * AdjustApproach()        Adjusts the approach ratios of the AI manager by new data received.
    *                         
    * Ins:			          tNewData
    *      		              
    * Outs:		              none
    *
    * Returns:		          none
    *
    * Mod. Date:		      12/11/2020
    * Mod. Initials:	      KK
    *****************************************************************/
    private void AdjustApproach()
    {
        //Fire Resistance changed = Fireballs used
        m_fFireResistanceChanged = (m_fFireResistanceChanged + (0.01f * DataManager.g_cInstance.m_nFireballsUsed));
        m_fFireResistanceRatio = Mathf.Clamp(m_fFireResistanceRatio + m_fFireResistanceChanged, 0.0f, 1.0f);

        //Cold Resistance = Cold time Used
        m_fColdResistanceChanged = (m_fColdResistanceChanged + (0.01f * DataManager.g_cInstance.m_fConeOfColdTimeUsed));
        m_fColdResistanceRatio = Mathf.Clamp(m_fColdResistanceRatio + m_fColdResistanceChanged, 0.0f, 1.0f);

        //Lightning Resistance = Lighting Bolts used
        m_fLightningResistanceChanged = (m_fLightningResistanceChanged + (0.01f * DataManager.g_cInstance.m_nLightningBoltsUsed));
        m_fLightningResistanceRatio = Mathf.Clamp(m_fLightningResistanceRatio + m_fLightningResistanceChanged, 0.0f, 1.0f);

        //Aggression (movement speed, attack speed) = Damage done to Player
        //determine the amount the ratio should increase by: If ratio reaches 1, level up enemies and reset.
        m_fAggressionChanged = (m_fAggressionChanged + (1.0f / (DataManager.g_cInstance.m_nDamageDealtToPlayer + 1.0f)));
        m_fAggressionRatio = Mathf.Clamp(m_fAggressionRatio + m_fAggressionChanged, 0.0f, 1.0f);

        //Horde Amount (Number of enemies in a wave, Maximum number on map at once, spawn rate) = wave number / previous wave count
        m_fHordeAmountChanged = (m_fHordeAmountChanged + (WaveManager.g_cInstance.m_nCurrentWave / (float)m_nPreviousEnemyCountRemaining));
        m_fHordeAmountRatio = Mathf.Clamp(m_fHordeAmountRatio + m_fHordeAmountChanged, 0.0f, 1.0f);
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
}
