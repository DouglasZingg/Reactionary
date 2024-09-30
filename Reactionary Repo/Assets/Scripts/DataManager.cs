using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		DataManager.cs
 * Date:      		12/05/2020
 * Mod. Date: 		1/18/2021
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to collect data from each wave.
 *                  Gathers player data, enemy data, and world data
 ************************************************/

public class DataManager : MonoBehaviour
{
    public static DataManager g_cInstance = null;

    //Variable fields to be tracked PER WAVE
    //Power variables
    public int m_nFireballsUsed { get; set; }
    public int m_nJumpsUsed { get; set; }
    public int m_nLightningBoltsUsed { get; set; }
    public float m_fConeOfColdTimeUsed { get; set; }

    //Damage variables
    public int m_nDamageDealtToPlayer { get; set; }

    //Wave variables
    public float m_fTimeTakenToClearWave { get; set; }

    /***************
    * Mutators
    ***************/
    public void ChangeFireballsUsed(int nChangeAmount)
    {
        //Added this line for testing purposes
        Debug.Log("Received " + nChangeAmount + " Fireballs from PowerManagerScript");
        m_nFireballsUsed += nChangeAmount;
    }

    public void ChangeLightningBoltsUsed(int nChangeAmount)
    {
        //Added this line for testing purposes
        Debug.Log("Received " + nChangeAmount + " LightningBolts from PowerManagerScript");
        m_nLightningBoltsUsed += nChangeAmount;
    }

    public void ChangeColdTimeUsed(float fChangeAmount)
    {
        //Added this line for testing purposes
        Debug.Log("Received " + fChangeAmount + " Cold Time from PowerManagerScript");
        m_fConeOfColdTimeUsed += fChangeAmount;
    }

    public void ChangeJumpsUsed(int nChangeAmount)
    {
        Debug.Log("Received " + nChangeAmount + " Jumps from FPSMovementScript");
        m_nJumpsUsed += nChangeAmount;
    }

    public void ChangeDamageDealtToPlayer(int nChangeAmount)
    {
        Debug.Log("Received " + nChangeAmount + " player damage from PlayerStatScript");
        m_nDamageDealtToPlayer += nChangeAmount;
    }

    //resets data manager for next wave 
    public void ZeroOut()
    {
        m_nDamageDealtToPlayer = 0;
        m_nFireballsUsed = 0;
        m_nJumpsUsed = 0;
        m_fTimeTakenToClearWave = 0;
    }

    private void Start()
    {
        ZeroOut();
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
