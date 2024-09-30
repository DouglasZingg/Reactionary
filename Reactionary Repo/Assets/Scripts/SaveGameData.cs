using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGameData
{
    //Wave Manager
    public int nCurrentWave;

    //Data Manager
    public int nFireballsUsed;
    public int nJumpsUsed;
    public int nDamageDealtToPlayer;
    public int nLightningBoltsUsed;
    public float fTimeTakenToClearWave;
    public float fConeOfColdTimeUsed;

    //DynamicAIWaves
    public float fFireResistanceRatio;
    public float fFireResistanceChanged;
    public float fColdResistanceRatio;
    public float fColdResistanceChanged;
    public float fLightningResistanceRatio;
    public float fLightningResistanceChanged;
    public float fAggressionRatio;
    public float fAggressionChanged;
    public float fHordeAmountRatio;
    public float fHordeAmountChanged;
    public float fPreviousSpawnRate;
    public int nPreviousEnemyCountRemaining;
    public int nPreviousMaxEnemyCountOnMap;

    //Score Manager
    public int nCurrentScore;
    public int nTotalScore;
    public int nTotalKills;
    public int nKillStreak;

    //Player Stat Script
    public float fCurrentHealth;
    public float fCurrentMana;

    public SaveGameData(float fHealth, float fMana)
    {
        if (WaveManager.g_cInstance != null &&
            DataManager.g_cInstance != null &&
            DynamicAIWaves.g_cInstance != null &&
            ScoreManager.g_cInstance != null
            )
        {
            nCurrentWave = WaveManager.g_cInstance.m_nCurrentWave;

            nFireballsUsed = DataManager.g_cInstance.m_nFireballsUsed;
            nLightningBoltsUsed = DataManager.g_cInstance.m_nLightningBoltsUsed;
            nJumpsUsed = DataManager.g_cInstance.m_nJumpsUsed;
            nDamageDealtToPlayer = DataManager.g_cInstance.m_nDamageDealtToPlayer;
            fConeOfColdTimeUsed = DataManager.g_cInstance.m_fConeOfColdTimeUsed;
            fTimeTakenToClearWave = DataManager.g_cInstance.m_fTimeTakenToClearWave;

            fFireResistanceRatio = DynamicAIWaves.g_cInstance.m_fFireResistanceRatio;
            fFireResistanceChanged = DynamicAIWaves.g_cInstance.m_fFireResistanceChanged;
            fAggressionRatio = DynamicAIWaves.g_cInstance.m_fAggressionRatio;
            fAggressionChanged = DynamicAIWaves.g_cInstance.m_fAggressionChanged;
            fHordeAmountRatio = DynamicAIWaves.g_cInstance.m_fHordeAmountRatio;
            fHordeAmountChanged = DynamicAIWaves.g_cInstance.m_fHordeAmountChanged;
            fColdResistanceRatio = DynamicAIWaves.g_cInstance.m_fColdResistanceRatio;
            fColdResistanceChanged = DynamicAIWaves.g_cInstance.m_fColdResistanceChanged;
            fLightningResistanceRatio = DynamicAIWaves.g_cInstance.m_fLightningResistanceRatio;
            fLightningResistanceChanged = DynamicAIWaves.g_cInstance.m_fLightningResistanceChanged;
            fPreviousSpawnRate = DynamicAIWaves.g_cInstance.m_fPreviousSpawnRate;
            nPreviousEnemyCountRemaining = DynamicAIWaves.g_cInstance.m_nPreviousEnemyCountRemaining;
            nPreviousMaxEnemyCountOnMap = DynamicAIWaves.g_cInstance.m_nPreviousMaxEnemyCountOnMap;

            nCurrentScore = ScoreManager.g_cInstance.m_nCurrentScore;
            nTotalScore = ScoreManager.g_cInstance.m_nTotalScore;
            nTotalKills = ScoreManager.g_cInstance.m_nTotalKills;
            nKillStreak = ScoreManager.g_cInstance.m_nKillStreak;

            fCurrentHealth = fHealth;
            fCurrentMana = fMana;

            Debug.Log("Data is saved");
        }
        else
        {
            Debug.Log("Can't save game: Singletons are null");
        }
    }
}

