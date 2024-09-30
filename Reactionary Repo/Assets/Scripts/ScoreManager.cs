using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		ScoreManager.cs
 * Date:      		1/6/2021
 * Mod. Date: 		1/20/2021
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to handle score transactions and tracking
 ************************************************/

public class ScoreManager : MonoBehaviour
{
    //member variables
    public static ScoreManager g_cInstance;

    public int m_nCurrentScore { get; set; }
    public int m_nTotalScore { get; set; }
    public int m_nTotalKills { get; set; }
    public int m_nKillStreak { get; set; }
    public int m_nMultiKillMultiplier { get; set; } = 1;

    [SerializeField] private int m_nDefaultHeadBanagerValue = 10;
    [SerializeField] private int m_nDefaultLossValue = 5;
    [SerializeField] private float m_fMultiKillResetTime = 1.5f;
    private float m_fMultiKillTimer = 0;

    //ticks down the window of time to get a multikill. If 0, reset to 1
    private void Update()
    {
        if (m_fMultiKillTimer > 0)
        {
            m_fMultiKillTimer -= Time.deltaTime;
        }
        else
        {
            m_nMultiKillMultiplier = 1;
        }
    }

    //Currently supports HeadBanger Kills
    public void EnemyDied()
    {
        //Increase total score, current score, and total kills. 
        m_nTotalKills++;
        if (m_nKillStreak > 1)
        {
            m_nCurrentScore = Clamp(m_nCurrentScore + (m_nDefaultHeadBanagerValue * m_nMultiKillMultiplier * m_nKillStreak));
            m_nTotalScore = Clamp(m_nTotalScore + (m_nDefaultHeadBanagerValue * m_nMultiKillMultiplier * m_nKillStreak));
        }
        else
        {
            m_nCurrentScore = Clamp(m_nCurrentScore + (m_nDefaultHeadBanagerValue * m_nMultiKillMultiplier));
            m_nTotalScore = Clamp(m_nTotalScore + (m_nDefaultHeadBanagerValue * m_nMultiKillMultiplier));
        }

        m_nKillStreak++;

        //Check for multiKill and add to the multiplier
        if (m_fMultiKillTimer > 0)
        {
            //If killed in time, double the multiplier
            m_nMultiKillMultiplier *= 2;
        }
        //reset the timer for the chance at a higher multikill
        m_fMultiKillTimer = m_fMultiKillResetTime;

        Debug.Log("Score increased by " + 10);
    }

    public void PlayerGotHit()
    {
        //Decrease total score, current score, and reset kill streak
        m_nCurrentScore = Clamp(m_nCurrentScore - m_nDefaultLossValue);
        m_nTotalScore = Clamp(m_nTotalScore - m_nDefaultLossValue);
        m_nKillStreak = 0;
    }

    private int Clamp(int nValue)
    {
        if (nValue < 0)
        {
            return 0;
        }

        uint temp = (uint)nValue;

        if (nValue > int.MaxValue)
        {
            return int.MaxValue;
        }

        return nValue;
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
