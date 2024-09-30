using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject m_Fireball = null;
    [SerializeField] private GameObject m_LightningBolt = null;
    [SerializeField] private GameObject m_ColdCone = null;

    [SerializeField] private GameObject m_ProjectileSpawnPosition = null;
    [SerializeField] private GameObject m_Camera = null;
    [SerializeField] private GameObject m_playerStatManager = null;
    [SerializeField] private GameObject m_dataManagerObject = null;
    [SerializeField] private GameObject m_ManaObject = null;

    private ManaBar m_manaScript = null;
    private DataManager m_dataManager = null;
    private PlayerStatScript m_playerStatScript = null;
    [SerializeField] UIManager m_uiManagerScript = null;
    [SerializeField] JumpBar g_fireBar = null;

    [SerializeField] private float m_fFireballAttackSpeed = 1.0f;
    [SerializeField] private float m_fFireballVelocity = 30.0f;
    [SerializeField] private float m_fFireballManaDrain = 50.0f;

    [SerializeField] private float m_fLightningBoltAttackSpeed = 1.0f;
    [SerializeField] private float m_fLightningBoltVelocity = 30.0f;
    [SerializeField] private float m_fLightningBoltManaDrain = 50.0f;

    [SerializeField] private float m_fColdConeManaDrain = 0.05f;

    [SerializeField] private float m_fMagicBoltAttackSpeed = 1.0f;
    [SerializeField] private float m_fMagicBoltVelocity = 0;
    private float m_fWandLastTimeFired = 0.0f;

    private float m_fLastTimeFired = 0.0f;
    private bool m_bGodMode = false;
    int i_selectedPower = 0;

    void Start()
    {
        m_playerStatScript = m_playerStatManager.GetComponent<PlayerStatScript>();
        m_dataManager = m_dataManagerObject.GetComponent<DataManager>();
        m_manaScript = m_ManaObject.GetComponent<ManaBar>();
        m_playerStatScript = m_playerStatManager.GetComponent<PlayerStatScript>();
    }

    // Update is called once per frame
    void Update()
    {
        m_bGodMode = m_playerStatScript.GetGodMode();
        GameObject UIManager = GameObject.Find("Menu Manager");
        UIManager UIScript = UIManager.GetComponent<UIManager>();

        if (!UIScript.b_isPaused)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                Debug.Log(i_selectedPower);
                if (i_selectedPower >= 2)
                    i_selectedPower = 0;
                else
                    i_selectedPower++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                Debug.Log(i_selectedPower);
                if (i_selectedPower <= 0)
                    i_selectedPower = 2;
                else
                    i_selectedPower--;
            }

            m_uiManagerScript.SwitchPowers(i_selectedPower);

            if (Input.GetKeyDown("e"))
            {
                if (i_selectedPower == 0)
                {
                    ShootPower(m_Fireball);
                }
                if (i_selectedPower == 1)
                {
                    ShootPower(m_LightningBolt);
                }
                if (i_selectedPower == 2)
                {
                    ActivateColdcone();
                    if (m_ColdCone.activeSelf)
                    {
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerFiresConeOfCold);
                    }
                    else
                    {
                        SFXManager.g_cInstance.StopSoundEffect(SFXManager.eSounds.PlayerFiresConeOfCold);
                    }
                }
            }
            else
            {
                if(Input.GetMouseButton(0))
                {
                    ShootPower(null, true);
                }
            }
            if (m_ColdCone.activeSelf)
            {
                ColdConeManaDrain();
            }
        }
    }

    private void ColdConeManaDrain()
    {
        if (m_playerStatScript.GetCurrentMana() >= m_fColdConeManaDrain)
        {
            m_playerStatScript.SumCurrentMana(m_fColdConeManaDrain * -1);
            DataManager.g_cInstance.BroadcastMessage("ChangeColdTimeUsed", Time.deltaTime);
        }
        if (!m_bGodMode && m_playerStatScript.GetCurrentMana() < m_fColdConeManaDrain)
        {
            SFXManager.g_cInstance.StopSoundEffect(SFXManager.eSounds.PlayerFiresConeOfCold);
            m_ColdCone.SetActive(false);
        }
    }

    //ActivateColdcone()
    //Ins:
    //Outs:
    //Function: Sets Cone of Cold (ColdCone) active.
    public void ActivateColdcone()
    {
        m_ColdCone.SetActive(!m_ColdCone.activeSelf);
    }

    //FirePower
    //Ins:
    //Outs:
    //Function: Instantiates a fireball (case 0) or lightning bolt (case 1) and fires it with a velocity towards where the player is aiming.
    private void FirePower(GameObject powerProjectile, int type)
    {
        GameObject clone = null;
        if(type > 0)
        {
            clone = Instantiate(powerProjectile, m_ProjectileSpawnPosition.transform.position, powerProjectile.transform.rotation);
            clone.gameObject.layer = 10;
            clone.gameObject.SetActive(true);
            clone.transform.forward = m_Camera.transform.forward;
            clone.transform.up = m_Camera.transform.up;
            m_fLastTimeFired = Time.time;
        }

        switch (type)
        {
            case 0:
                {
                    SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerFiresMagicBolt);
                    clone = ObjectPool.g_cInstance.GetMagicBolt();
                    clone.transform.position = m_ProjectileSpawnPosition.transform.position;
                    clone.gameObject.layer = 10;
                    clone.gameObject.SetActive(true);
                    clone.transform.forward = m_Camera.transform.forward;
                    clone.transform.up = m_Camera.transform.up;
                    clone.GetComponent<Rigidbody>().velocity = m_Camera.transform.forward * m_fMagicBoltVelocity;
                    m_fWandLastTimeFired = Time.time;
                    break;
                }
            case 1:
                {
                    m_dataManager.BroadcastMessage("ChangeFireballsUsed", 1);
                    Debug.Log("Broadcast " + 1 + " Fireball to DataManager");
                    SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerFiresFireball);
                    clone.GetComponent<Rigidbody>().velocity = m_Camera.transform.forward * m_fFireballVelocity;
                    break;
                }
            case 2:
                {
                    m_dataManager.BroadcastMessage("ChangeLightningBoltsUsed", 1);
                    Debug.Log("Broadcast " + 1 + " LightningBolt to DataManager");
                    clone.GetComponent<Rigidbody>().velocity = m_Camera.transform.forward * m_fLightningBoltVelocity;
                    break;
                }
        }        
    }

    //ShootPower
    //Checks if the power is ready to fire. If it is, calls FirePower()
    //Ins: 
    //Outs:
    public void ShootPower(GameObject powerProjectile, bool isMagicBolt = false)
    {
        //Instantiate a projectile and set the projectile's velocity towards the forward vector of the player transform
        if (!isMagicBolt && m_bGodMode)
        {
            Debug.Log("GodMode prevented Mana loss from using " + powerProjectile.name);
        }
        if(isMagicBolt)
        {
            if(Time.time > m_fWandLastTimeFired + m_fMagicBoltAttackSpeed)
            {
                FirePower(null, 0);
            }
        }
        else
        {
            switch (powerProjectile.tag)
            {
                case "FIREBALL":
                    {
                        if (m_bGodMode)
                        {

                        }
                        else if (m_manaScript.f_mana >= m_fFireballManaDrain && Time.time > m_fLastTimeFired + m_fFireballAttackSpeed)
                        {
                            m_playerStatScript.SumCurrentMana(-1 * m_fFireballManaDrain);
                        }
                        else
                        {
                            break;
                        }
                        FirePower(powerProjectile, 1);
                        break;
                    }
                case "LIGHTNINGBOLT":
                    {
                        if (m_bGodMode)
                        {

                        }
                        else if (m_manaScript.f_mana >= m_fLightningBoltManaDrain && Time.time > m_fLastTimeFired + m_fLightningBoltAttackSpeed)
                        {
                            m_playerStatScript.SumCurrentMana(-1 * m_fLightningBoltManaDrain);
                        }
                        else
                        {
                            break;
                        }
                        FirePower(powerProjectile, 2);
                        break;
                    }
            }
        }
        
    }
}
