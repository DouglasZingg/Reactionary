using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		ObjectPool.cs
 * Date:      		12/18/2020
 * Mod. Date: 		12/21/2020
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to create objects for reuse throughout the game
 ************************************************/

public class ObjectPool : MonoBehaviour
{
    //member variables
    public static ObjectPool g_cInstance;

    //MagicBolts
    [SerializeField] private GameObject m_cMagicBoltObject;
    [SerializeField] private int m_nNumMagicBolts;
    public List<GameObject> m_cPooledMagicBolts;

    //HeadBangers
    [SerializeField] private GameObject m_cHeadBangerObject;
    [SerializeField] private int m_nNumHeadBangers;
    public List<GameObject> m_cPooledHeadBangers;

    [SerializeField] private GameObject m_cFireHeadBangerObject;
    [SerializeField] private int m_nNumFireHeadBangers;
    public List<GameObject> m_cPooledFireHeadBangers;

    [SerializeField] private GameObject m_cIceHeadBangerObject;
    [SerializeField] private int m_nNumIceHeadBangers;
    public List<GameObject> m_cPooledIceHeadBangers;

    [SerializeField] private GameObject m_cLightningHeadBangerObject;
    [SerializeField] private int m_nNumLightningHeadBangers;
    public List<GameObject> m_cPooledLightningHeadBangers;


    //Creates the objects, deactivates them, and stores them into their respective lists.
    private void Start()
    {
        //Instantiate the MagicBolts and Store them
        m_cPooledMagicBolts = new List<GameObject>();
        for (int i = 0; i < m_nNumMagicBolts; i++)
        {
            GameObject cMagicBolt = Instantiate(m_cMagicBoltObject);
            cMagicBolt.SetActive(false);
            m_cPooledMagicBolts.Add(cMagicBolt);
        }

        //Instantiate the HeadBangers and Store them
        m_cPooledHeadBangers = new List<GameObject>();
        for (int i = 0; i < m_nNumHeadBangers; i++)
        {
            GameObject cHeadBanger = Instantiate(m_cHeadBangerObject);
            cHeadBanger.SetActive(false);
            m_cPooledHeadBangers.Add(cHeadBanger);
        }

        m_cPooledFireHeadBangers = new List<GameObject>();
        for (int i = 0; i < m_nNumFireHeadBangers; i++)
        {
            GameObject cFireHeadBanger = Instantiate(m_cFireHeadBangerObject);
            cFireHeadBanger.SetActive(false);
            m_cPooledFireHeadBangers.Add(cFireHeadBanger);
        }

        m_cPooledIceHeadBangers = new List<GameObject>();
        for (int i = 0; i < m_nNumIceHeadBangers; i++)
        {
            GameObject cIceHeadBanger = Instantiate(m_cIceHeadBangerObject);
            cIceHeadBanger.SetActive(false);
            m_cPooledIceHeadBangers.Add(cIceHeadBanger);
        }

        m_cPooledLightningHeadBangers = new List<GameObject>();
        for (int i = 0; i < m_nNumLightningHeadBangers; i++)
        {
            GameObject cLightningHeadBanger = Instantiate(m_cLightningHeadBangerObject);
            cLightningHeadBanger.SetActive(false);
            m_cPooledLightningHeadBangers.Add(cLightningHeadBanger);
        }
    }

    //Gets first available MagicBolt for use in game
    public GameObject GetMagicBolt()
    {
        //for each MagicBolt in the pool, find the first available
        for (int i = 0; i < m_cPooledMagicBolts.Count; i++)
        {
            //if this MagicBolt is not active in the scene, return it.
            if (!m_cPooledMagicBolts[i].activeInHierarchy)
            {
                return m_cPooledMagicBolts[i];
            }
        }
        //If there was not a MagicBolt availible, return null
        return null;
    }

    //Gets first available HeadBanger for use in game
    public GameObject GetHeadBanger()
    {
        //for each HeadBanger in the pool, find the first available
        for (int i = 0; i < m_cPooledHeadBangers.Count; i++)
        {
            //if this HeadBanger is not active in the scene, return it.
            if (!m_cPooledHeadBangers[i].activeInHierarchy)
            {
                return m_cPooledHeadBangers[i];
            }
        }
        //If there was not a HeadBanger availible, return null
        return null;
    }

    //Gets first available FireHeadBanger for use in game
    public GameObject GetFireHeadBanger()
    {
        //for each HeadBanger in the pool, find the first available
        for (int i = 0; i < m_cPooledFireHeadBangers.Count; i++)
        {
            //if this HeadBanger is not active in the scene, return it.
            if (!m_cPooledFireHeadBangers[i].activeInHierarchy)
            {
                return m_cPooledFireHeadBangers[i];
            }
        }
        //If there was not a HeadBanger availible, return null
        return null;
    }

    //Gets first available IceHeadBanger for use in game
    public GameObject GetIceHeadBanger()
    {
        //for each HeadBanger in the pool, find the first available
        for (int i = 0; i < m_cPooledIceHeadBangers.Count; i++)
        {
            //if this HeadBanger is not active in the scene, return it.
            if (!m_cPooledIceHeadBangers[i].activeInHierarchy)
            {
                return m_cPooledIceHeadBangers[i];
            }
        }
        //If there was not a HeadBanger availible, return null
        return null;
    }

    //Gets first available LightningHeadBanger for use in game
    public GameObject GetLightningHeadBanger()
    {
        //for each HeadBanger in the pool, find the first available
        for (int i = 0; i < m_cPooledLightningHeadBangers.Count; i++)
        {
            //if this HeadBanger is not active in the scene, return it.
            if (!m_cPooledLightningHeadBangers[i].activeInHierarchy)
            {
                return m_cPooledLightningHeadBangers[i];
            }
        }
        //If there was not a HeadBanger availible, return null
        return null;
    }

    public void ResetAll()
    {
        for (int i = 0; i < m_cPooledMagicBolts.Count; i++)
        {
            m_cPooledMagicBolts[i].SetActive(false);
        }
        for (int i = 0; i < m_cPooledHeadBangers.Count; i++)
        {
            m_cPooledHeadBangers[i].SetActive(false);
        }
        for (int i = 0; i < m_cPooledFireHeadBangers.Count; i++)
        {
            m_cPooledFireHeadBangers[i].SetActive(false);
        }
        for (int i = 0; i < m_cPooledIceHeadBangers.Count; i++)
        {
            m_cPooledIceHeadBangers[i].SetActive(false);
        }
        for (int i = 0; i < m_cPooledLightningHeadBangers.Count; i++)
        {
            m_cPooledLightningHeadBangers[i].SetActive(false);
        }
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
