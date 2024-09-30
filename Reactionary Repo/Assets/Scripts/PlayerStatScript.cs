using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStatScript : MonoBehaviour
{
    [SerializeField] private GameObject g_HealthBarObject = null;
    [SerializeField] private GameObject g_ManaBarObject = null;
    [SerializeField] private GameObject g_JumpIconObject = null;
    [SerializeField] private GameObject g_OffensivePowerIconObject = null;
    [SerializeField] private GameObject g_CollisionManagerObject = null;
    [SerializeField] private GameObject g_PlayerObject = null;
    [SerializeField] private GameObject g_DataManagerObject = null;
    [SerializeField] private GameObject g_healthPickup = null;
    [SerializeField] private GameObject g_manaPickup = null;
    [SerializeField] public bool g_bGodMode = false;
    [SerializeField] private float selfDamageAmount = 50.0f;
    [SerializeField] private GameObject SpawnType = null;
    [SerializeField] UnityEngine.UI.Image i_redicleImage;

    private HealthBar g_healthBar = null;
    private ManaBar g_manaBar = null;
    private JumpBar g_jumpBar = null;
    private CollisionManager g_collisionManager = null;
    private DataManager g_dataManager = null;

    public int m_nStatus = 0;

    private void Awake()
    {
        g_healthBar = g_HealthBarObject.GetComponent<HealthBar>();
        g_manaBar = g_ManaBarObject.GetComponent<ManaBar>();
        g_jumpBar = g_JumpIconObject.GetComponent<JumpBar>();
        g_collisionManager = g_CollisionManagerObject.GetComponent<CollisionManager>();
        g_dataManager = g_DataManagerObject.GetComponent<DataManager>();
    }

    //Added for testing purposes to damage player
    private void Update()
    {
        if (Input.GetKeyDown("-"))
        {
            DamagePlayer(selfDamageAmount);
        }
        if (Input.GetKeyDown("0"))
        {
            ToggleGodMode();
            SumCurrentHP(GetPlayerMaxHP());
        }
        if (Input.GetKeyDown("1"))
        {
            WaveManager waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
            waveManager.SkipWaveState();
        }
        if (Input.GetKeyDown("2"))
        {
            SumCurrentMana(GetMaxMana());
        }
        if (Input.GetKeyDown("3"))
        {
            GameObject wand = GameObject.Find("PivotedWand");
            Vector3 spawnPoint = wand.transform.position + wand.transform.forward.normalized * 10.0f;
            spawnPoint.y = 0.0f;
            Quaternion rotation = Quaternion.LookRotation(wand.transform.position - spawnPoint);
            rotation.x = 0;
            rotation.z = 0;

            GameObject clone = Instantiate(SpawnType, spawnPoint, rotation);
            clone.GetComponent<NavMeshAgent>().enabled = true;
            clone.GetComponent<EnemyAIScript>().enabled = true;
        }
        if(Input.GetKeyDown("4"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("ENEMY");
            for(int i = 0; i < enemies.Length; i++)
            {
                EnemyCommonsScript enemyCommons = enemies[i].GetComponent<EnemyCommonsScript>();
                if(enemyCommons != null)
                {
                    switch (m_nStatus)
                    {
                        case 0:
                            enemyCommons.AddTimer(eSTATUS.BURN, 2.1f);
                            break;
                        case 1:
                            enemyCommons.AddTimer(eSTATUS.FREEZE, 2.1f);
                            break;
                        case 2:
                            enemyCommons.AddTimer(eSTATUS.STUN, 2.1f);
                            break;
                        case 3:
                            Debug.Log("Player Cheat 4: Status Effects endList. Press 5 to re-cycle.");
                            break;
                    }
                }
            }
        }
        if(Input.GetKeyDown("5"))
        {
            m_nStatus++;
            m_nStatus %= 4;
        }
        if (Input.GetKeyDown("6"))
        {
            Vector3 v_posOfSpawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 20.0f);
            Instantiate(g_healthPickup, v_posOfSpawn, Quaternion.identity);
        }
        if (Input.GetKeyDown("7"))
        {
            Vector3 v_posOfSpawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 20.0f);
            Instantiate(g_manaPickup, v_posOfSpawn, Quaternion.identity);
        }

        Ray r_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit r_raycastHit;
        if(Physics.Raycast(r_ray, out r_raycastHit, 10000))
        {
            if(r_raycastHit.transform.tag == "ENEMY")
            {
                i_redicleImage.color = Color.white;
            }
        }
    }


    //Added by Petra
    #region Accessors
    //Name: Get[Variable]
    //Ins:
    //Outs: [Variable]

    public float GetPlayerMaxHP()
    {
        return g_healthBar.GetMaxHP();
    }
    public float GetCurrentHP()
    {
        return g_healthBar.GetCurrentHP();
    }
    public float GetHPLerpSpeed()
    {
        return g_healthBar.GetLerpSpeed();
    }
    public float GetMaxMana()
    {
        return g_manaBar.GetMaxMana();
    }
    public float GetCurrentMana()
    {
        return g_manaBar.GetCurrentMana();
    }
    public float GetManaLerpSpeed()
    {
        return g_manaBar.GetLerpSpeed();
    }
    public float GetMovementCooldown()
    {
        return g_jumpBar.GetCooldown();
    }
    public bool GetMovementIsCooldown()
    {
        return g_jumpBar.GetIsCooldown();
    }
    public float GetFireballDamage()
    {
        return g_collisionManager.GetFireballDamage();
    }
    public float GetMagicBoltDamage()
    {
        return g_collisionManager.GetMagicBoltDamage();
    }
    public bool GetGodMode()
    {
        return g_bGodMode;
    }
    #endregion

    #region Mutators
    //Name: Set[Variable]
    //Ins: newVariable
    //Outs:
    //Function: Sets the value of [Variable] to be equal to newVariable
    public void SetMaxHP(float f_newMaxHP)
    {
        g_healthBar.SetMaxHP(f_newMaxHP);
    }
    public void SetCurrentHP(float f_newCurrentHP)
    {
        g_healthBar.SetCurrentHP(f_newCurrentHP);
    }
    public void SetHPLerpSpeed(float f_newLerpSpeed)
    {
        g_healthBar.SetLerpSpeed(f_newLerpSpeed);
    }
    public void SetMaxMana(float f_newMaxMana)
    {
        g_manaBar.SetMaxMana(f_newMaxMana);
    }
    public void SetCurrentMana(float f_newCurrentMana)
    {
        g_manaBar.SetCurrentMana(f_newCurrentMana);
    }
    public void SetManaLerpSpeed(float f_newLerpSpeed)
    {
        g_manaBar.SetLerpSpeed(f_newLerpSpeed);
    }
    public void SetMovementCooldown(float f_newMovementCooldown)
    {
        g_jumpBar.SetCooldown(f_newMovementCooldown);
    }
    public void SetMovementIsCooldown(bool b_movementIsCooldown)
    {
        g_jumpBar.SetIsCooldown(b_movementIsCooldown);
    }
    public void SetMagicBoltDamage(float f_newMagicBoltDamage)
    {
        g_collisionManager.SetMagicBoltDamage(f_newMagicBoltDamage);
    }
    public void SetFireballDamage(float f_newFireballDamage)
    {
        g_collisionManager.SetFireballDamage(f_newFireballDamage);
    }
    public void SetGodMode(bool b_newGodMode)
    {
        g_bGodMode = b_newGodMode;
    }

    //Name: Sum[Variable]
    //Ins: deltaVariable
    //Outs:
    //Function: Variable += deltaVariable.
    public void SumMaxHP(float f_deltaMaxHP)
    {
        g_healthBar.SumMaxHP(f_deltaMaxHP);
    }
    public void SumCurrentHP(float f_deltaCurrentHP)
    {
        g_healthBar.SumCurrentHP(f_deltaCurrentHP);
    }
    public void SumHPLerpSpeed(float f_deltaLerpSpeed)
    {
        g_healthBar.SumLerpSpeed(f_deltaLerpSpeed);
    }
    public void SumMaxMana(float f_deltaMaxMana)
    {
        g_manaBar.SumMaxMana(f_deltaMaxMana);
    }
    public void SumCurrentMana(float f_deltaCurrentMana)
    {
        g_manaBar.SumCurrentMana(f_deltaCurrentMana);
    }
    public void SumManaLerpSpeed(float f_deltaLerpSpeed)
    {
        g_manaBar.SumLerpSpeed(f_deltaLerpSpeed);
    }
    public void SumMovementCooldown(float f_movementDeltaCooldown)
    {
        g_jumpBar.SumCooldown(f_movementDeltaCooldown);
    }
    public void SumMagicBoltDamage(float f_deltaMagicBoltDamage)
    {
        g_collisionManager.SumMagicBoltDamage(f_deltaMagicBoltDamage);
    }
    public void SumFireballDamage(float f_deltaFireballDamage)
    {
        g_collisionManager.SumFireballDamage(f_deltaFireballDamage);
    }

    //Name: Toggle[Variable]
    //Ins:
    //Outs:
    //Function: Inverts the value of [Variable]
    public void ToggleMovementIsCooldown()
    {
        g_jumpBar.ToggleIsCooldown();
    }
    public void ToggleGodMode()
    {
        g_bGodMode = !g_bGodMode;
    }

    //Name: DamagePlayer
    //Ins: float damageAmount
    //Outs:
    //Function: Damages the player. This function Broadcasts to DataManager, otherwise is identical to using SumCurrentHP with a negative deltaHP value.
    public void DamagePlayer(float f_damageAmount)
    {
        if (g_bGodMode)
        {
            Debug.Log("GodMode prevented " + f_damageAmount + " damage.");
        }
        else
        {
            ScoreManager.g_cInstance.PlayerGotHit();
            SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerGetsHurt);
            g_healthBar.SumCurrentHP(f_damageAmount * -1);
            g_dataManager.BroadcastMessage("ChangeDamageDealtToPlayer", f_damageAmount);
            Debug.Log("Broadcast " + f_damageAmount + " player damage to DataManager");
        }
    }

    #endregion
}
