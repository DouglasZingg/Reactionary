using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(SaveGameData gameData)
    {
        if (gameData != null)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string filePath = Application.persistentDataPath + "/SaveGame.fun";
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            binaryFormatter.Serialize(fileStream, gameData);
            fileStream.Close();

            Debug.Log("Game Saved!");
        }
        else
        {
            Debug.Log("SaveGameData was null, not saving");
        }
    }
    
    public static void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/SaveGame.fun";
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            SaveGameData gameData = binaryFormatter.Deserialize(fileStream) as SaveGameData;
            fileStream.Close();

            SetData(gameData);
            WaveManager.g_cInstance.LoadGame(gameData);
            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogError("Save file was not found");
        }
    }

    private static void SetData(SaveGameData gameData)
    {
        if (WaveManager.g_cInstance != null &&
            DataManager.g_cInstance != null &&
            DynamicAIWaves.g_cInstance != null &&
            ScoreManager.g_cInstance != null
            )
        {
            WaveManager.g_cInstance.m_nCurrentWave = gameData.nCurrentWave;

            DataManager.g_cInstance.m_nFireballsUsed = gameData.nFireballsUsed;
            DataManager.g_cInstance.m_nJumpsUsed = gameData.nJumpsUsed;
            DataManager.g_cInstance.m_nDamageDealtToPlayer = gameData.nDamageDealtToPlayer;
            DataManager.g_cInstance.m_fTimeTakenToClearWave = gameData.fTimeTakenToClearWave;
            DataManager.g_cInstance.m_fConeOfColdTimeUsed = gameData.fConeOfColdTimeUsed;
            DataManager.g_cInstance.m_nLightningBoltsUsed = gameData.nLightningBoltsUsed;

            DynamicAIWaves.g_cInstance.m_fFireResistanceRatio = gameData.fFireResistanceRatio;
            DynamicAIWaves.g_cInstance.m_fFireResistanceChanged = gameData.fFireResistanceChanged;
            DynamicAIWaves.g_cInstance.m_fAggressionRatio = gameData.fAggressionRatio;
            DynamicAIWaves.g_cInstance.m_fAggressionChanged = gameData.fFireResistanceChanged;
            DynamicAIWaves.g_cInstance.m_fHordeAmountRatio = gameData.fHordeAmountRatio;
            DynamicAIWaves.g_cInstance.m_fHordeAmountChanged = gameData.fHordeAmountChanged;
            DynamicAIWaves.g_cInstance.m_fColdResistanceRatio = gameData.fColdResistanceRatio;
            DynamicAIWaves.g_cInstance.m_fColdResistanceChanged = gameData.fColdResistanceChanged;
            DynamicAIWaves.g_cInstance.m_fLightningResistanceRatio = gameData.fLightningResistanceRatio;
            DynamicAIWaves.g_cInstance.m_fLightningResistanceChanged = gameData.fLightningResistanceChanged;
            DynamicAIWaves.g_cInstance.m_fPreviousSpawnRate = gameData.fPreviousSpawnRate;
            DynamicAIWaves.g_cInstance.m_nPreviousEnemyCountRemaining = gameData.nPreviousEnemyCountRemaining;
            DynamicAIWaves.g_cInstance.m_nPreviousMaxEnemyCountOnMap = gameData.nPreviousMaxEnemyCountOnMap;

            ScoreManager.g_cInstance.m_nCurrentScore = gameData.nCurrentScore;
            ScoreManager.g_cInstance.m_nTotalScore = gameData.nTotalScore;
            ScoreManager.g_cInstance.m_nTotalKills = gameData.nTotalKills;
            ScoreManager.g_cInstance.m_nKillStreak = gameData.nKillStreak;

            Debug.Log("Game Data Set!");
        }
        else
        {
            Debug.LogWarning("Game Data was not set, SetData was null");
        }
    }
}
