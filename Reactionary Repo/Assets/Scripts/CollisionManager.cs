using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		CollisionManager.cs
 * Date:      		12/05/2020
 * Mod. Date: 		12/09/2020
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to handle reactions from all collisions in game
 ************************************************/

//These will be the labels attached to objects and entities to determine collision resolution
public enum eHIT
{
    UNAFFECTED, TERRAIN, ENEMY, HEADSHOT, PLAYER, _endList
}

//These will be all objects that could hit something for reaction.
public enum ePROJECTILES
{
    MAGICBOLT, FIREBALL, COLDCONE, LIGHTNINGBOLT, ENEMY, _endList
}

public static class HitEncountered
{
    public delegate void HitResponse(ePROJECTILES projectile, GameObject collider, GameObject objectColliding);
    public static event HitResponse hitEncounteredEvent;

    public static void SendHitResponse(ePROJECTILES projectile, GameObject collider, GameObject objectColliding)
    {
        hitEncounteredEvent(projectile, collider, objectColliding);
    }
}

public class CollisionManager : MonoBehaviour
{
    //members
    [SerializeField] float m_fMagicBoltDamageToEnemies = 4;
    [SerializeField] float m_fFireballDamageToEnemies = 6;
    [SerializeField] float m_fFireballCollisionBurnDuration = 2.0f;
    [SerializeField] float m_fColdConeDamageToEnemies = 2;
    [SerializeField] float m_fColdConeFreezeDuration = 0.02f;
    [SerializeField] float m_fLightningBoltDamageToEnemies = 3;
    [SerializeField] float m_fLightningBoltStunDuration = 2.0f;
    [SerializeField] float m_fZombieDamageToPlayer = 10;

    [SerializeField] GameObject m_playerStatsObject = null;
    private PlayerStatScript m_playerStatScript = null;

    Color m_orange;

    //2D array (table) of possible reactions.
    int[,] nReactionsTable = new int[(int)eHIT._endList, (int)ePROJECTILES._endList];


    //used to populate the table of reactions for indexing
    void FillResultTable()
    {
        int nEntryIndex = 0;
        for (int i = 0; i < (int)eHIT._endList; i++)
        {
            for (int j = 0; j < (int)ePROJECTILES._endList; j++)
            {
                nReactionsTable[i, j] = nEntryIndex;
                nEntryIndex++;
            }
        }
    }

    private void OnEnable()
    {
        HitEncountered.hitEncounteredEvent += GetHitReaction;
    }

    private void Start()
    {
        FillResultTable();
        m_playerStatScript = m_playerStatsObject.GetComponent<PlayerStatScript>();
        m_orange.r = 1.0f;
        m_orange.g = 0.9f;
        m_orange.b = 0.0f;
        m_orange.a = 1.0f;
    }

    //Name: CheckComponent
    //Ins: Component inComponent
    //Outs:
    //Function: Checks if inComponent is null. returns a bool. If false, throws a debug error.
    //-Petra
    public static bool CheckComponent(Component inComponent)
    {
        if(inComponent != null)
        {
            return true;
        }
        else
        {
            Debug.LogError("Component was null.");
            return false;
        }
    }

    //Name: DamageEnemy
    //Ins: EnemyStatScript enemyStats, Color color, int damageAmount, bool doBloodSplatter
    //Outs:
    //Function: Damages an enemy and sets their color. If doBloodSplatter is true, trigger a blood splatter particle effect.
    //-Petra
    private void DamageEnemy(EnemyCommonsScript EnemyCommons, float damageAmount, ePROJECTILES damageType, Color color,  bool doBloodSplatter = false, float statusTimer = 2.0f, bool setHeadColor = false)
    {
        float damageResistance = 1.0f;
        switch (damageType)
        {
            case ePROJECTILES.MAGICBOLT:
                {
                    damageResistance = EnemyCommons.GetGenericResistance();
                    break;
                }
            case ePROJECTILES.FIREBALL:
                {
                    if(!EnemyCommons.GetBurnImmunity())
                    {
                        EnemyCommons.AddTimer(eSTATUS.BURN, statusTimer);
                    }
                    damageResistance = EnemyCommons.GetFireResistance();
                    break;
                }
            case ePROJECTILES.COLDCONE:
                {
                    if(!EnemyCommons.GetFreezeImmunity())
                    {
                        EnemyCommons.AddTimer(eSTATUS.FREEZE, statusTimer);
                    }
                    damageResistance = EnemyCommons.GetIceResistance();
                    break;
                }
            case ePROJECTILES.LIGHTNINGBOLT:
                {
                    if(!EnemyCommons.GetStunImmunity())
                    {
                        EnemyCommons.AddTimer(eSTATUS.STUN, statusTimer);
                    }
                    damageResistance = EnemyCommons.GetLightningResistance();
                    break;
                }
            default:
                {
                    Debug.LogError("Collision Manager's DamageEnemy() damageType was an unexpected value");
                    break;
                }
        }
        float resistedDamage = damageAmount / damageResistance;
        EnemyCommons.SumCurrentHP(resistedDamage * -1);
        EnemyCommons.SetBodyColor(color);
        if(doBloodSplatter)
        {
            EnemyCommons.PlayBloodSplatter();
        }
        if(setHeadColor)
        {
            EnemyCommons.SetHeadColor(color);
        }
        return;
    }

    /*****************************************************************
    * GetHitReaction()        Takes in a projectile label and object hit
    *                         to determine what kind of reaction should occur
    *
    * Ins:			          eProjectile
    *      		              gameObjectHit
    *
    * Outs:		              Varies
    *
    * Returns:		          void
    *
    * Mod. Date:		      12/09/2020
    * Mod. Initials:	      KK
    *****************************************************************/
    private void GetHitReaction(ePROJECTILES eProjectile, GameObject gameObjectHit, GameObject gameObjectHitting)
    {
        //get the label of the object hit
        HitLabel cObjectHitLabel;
        if (gameObjectHit.TryGetComponent(out cObjectHitLabel))
        {
            eHIT eObjectLabel = cObjectHitLabel.GetLabel();

            //find the index of this reaction in the table
            switch (nReactionsTable[(int)eObjectLabel, (int)eProjectile])
            {
                #region REACTIONS
                //MAGICBOLT HITS UNAFFECTED
                case 0:
                    {
                        //Debug.Log("Hit Unaffected with Magicbolt");
                    }
                    break;

                //FIREBALL HITS UNAFFECTED
                case 1:
                    {
                        //Debug.Log("Hit Unaffected with Fireball");
                    }
                    break;
                //COLDCONE HITS UNAFFECTED
                case 2:
                    {
                        //Debug.Log("Hit Unaffected with Cone of Cold");
                    }
                    break;

                //LIGHTNINGBOLT HITS UNAFFECTED
                case 3:
                    {
                        //Debug.Log("Hit Unaffected with LightningBolt");
                    }
                    break;

                //ENEMY HITS UNAFFECTED
                case 4:
                    {
                        //Debug.Log("Enemy attacked Unaffected");
                    }
                    break;

                //MAGICBOLT HITS TERRAIN
                case 5:
                    {
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.TerrainHitByMagicBolt);
                        //Debug.Log("Hit Terrain with Magicbolt");
                    }
                    break;

                //FIREBALL HITS TERRAIN
                case 6:
                    {
                        //Debug.Log("Hit Terrain with Fireball");
                    }
                    break;

                //COLDCONE HITS TERRAIN
                case 7:
                    {
                        //Debug.Log("Hit Terrain with Cone of Cold");
                    }
                    break;

                //LIGHTNINGBOLT HITS TERRAIN
                case 8:
                    {
                        //Debug.Log("Hit Terrain with Lightning Bolt");
                    }
                    break;

                //ENEMY HITS TERRAIN
                case 9:
                    {
                        //Debug.Log("Enemy attacked Terrain");
                        break;
                    }

                //MAGICBOLT HITS ENEMY
                case 10:
                    {
                        //Debug.Log("Hit Enemy with Magicbolt");
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.EnemyHitByMagicBolt);
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.MAGICBOLT;
                            DamageEnemy(EnemyCommons, m_fMagicBoltDamageToEnemies, ePROJECTILES.MAGICBOLT, Color.red, true);
                        }
                    }
                    break;

                //FIREBALL HITS ENEMY
                case 11:
                    {
                        //Debug.Log("Hit Enemy with Fireball");
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.EnemyHitByFireball);
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.FIREBALL;
                            script.m_lastAppliedForce = Vector3.Normalize(gameObjectHit.transform.position - gameObjectHitting.transform.position);
                            DamageEnemy(EnemyCommons, m_fFireballDamageToEnemies, ePROJECTILES.FIREBALL, m_orange, false, m_fFireballCollisionBurnDuration);
                        }

                    }
                    break;

                //COLDCONE HITS ENEMY
                case 12:
                    {
                        //Debug.Log("Hit Enemy with Cone of Cold");
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.COLDCONE;
                            script.m_lastAppliedForce = Vector3.Normalize(gameObjectHit.transform.position - gameObjectHitting.transform.position);
                            DamageEnemy(EnemyCommons, m_fColdConeDamageToEnemies * 0.5f, ePROJECTILES.COLDCONE, Color.blue, false, m_fColdConeFreezeDuration);
                        }                        
                    }
                    break;

                //LIGHTNINGBOLT HITS ENEMY
                case 13:
                    {
                        //Debug.Log("Hit Enemy with Lightning bolt");
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.EnemyHitByMagicBolt);
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.LIGHTNINGBOLT;
                            DamageEnemy(EnemyCommons, m_fLightningBoltDamageToEnemies, ePROJECTILES.LIGHTNINGBOLT, Color.yellow, true, m_fLightningBoltStunDuration);
                        }                        
                    }
                    break;

                //ENEMY HITS ENEMY
                case 14:
                    {
                        //Debug.Log("Enemy attacked Enemy");
                    }
                    break;

                //MAGICBOLT HEADSHOTS ENEMY
                case 15:
                    {
                        //Debug.Log("Headshot Enemy with Magicbolt");
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.Headshot);
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.MAGICBOLT;
                            DamageEnemy(EnemyCommons, m_fMagicBoltDamageToEnemies * 2, ePROJECTILES.MAGICBOLT, Color.red, true);
                        }
                    }
                    break;

                //FIREBALL HEADSHOTS ENEMY
                case 16:
                    {
                        //Debug.Log("Headshot Enemy with Fireball");
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.FIREBALL;
                            script.m_lastAppliedForce = Vector3.Normalize(gameObjectHit.transform.position - gameObjectHitting.transform.position);
                            DamageEnemy(EnemyCommons, m_fFireballDamageToEnemies, ePROJECTILES.FIREBALL, m_orange, false, m_fFireballCollisionBurnDuration * 1.5f);
                        }
                    }
                    break;

                //COLDCONE HEADSHOTS ENEMY
                case 17:
                    {
                        //Debug.Log("Headshot Enemy with Cone of Cold");
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                           EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                           script.m_LastHitBy = ePROJECTILES.COLDCONE;
                           script.m_lastAppliedForce = Vector3.Normalize(gameObjectHit.transform.position - gameObjectHitting.transform.position);
                           DamageEnemy(EnemyCommons, m_fColdConeDamageToEnemies * 0.5f, ePROJECTILES.COLDCONE, Color.blue, false, m_fColdConeFreezeDuration);
                        }
                    }
                    break;

                //LIGHTNINGBOLT HEADSHOTS ENEMY
                case 18:
                    {
                        //Debug.Log("Headshot Enemy with Lightning bolt");
                        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.Headshot);
                        EnemyCommonsScript EnemyCommons = gameObjectHit.GetComponentInParent<EnemyCommonsScript>();
                        if (CheckComponent(EnemyCommons))
                        {
                            EnemyAIScript script = EnemyCommons.GetEnemyAIScript();
                            script.m_LastHitBy = ePROJECTILES.LIGHTNINGBOLT;
                            DamageEnemy(EnemyCommons, m_fLightningBoltDamageToEnemies * 2, ePROJECTILES.LIGHTNINGBOLT, Color.yellow, true, m_fLightningBoltStunDuration * 1.5f);
                        }
                    }
                    break;

                //ENEMY HEADSHOTS ENEMY
                case 19:
                    {
                        //Debug.Log("Headshot Enemy collider with Enemy Attack");
                    }
                    break;

                //MAGICBOLT HITS PLAYER
                case 20:
                    {
                        //Debug.Log("Hit Player with Magicbolt");
                    }
                    break;
                //FIREBALL HITS PLAYER
                case 21:
                    {
                        //Debug.Log("Hit Player with Fireball");
                    }
                    break;
                //COLDCONE HITS PLAYER
                case 22:
                    {
                        //Debug.Log("Hit Player with Cone of Cold");
                    }
                    break;

                //LIGHTNINGBOLT HITS PLAYER
                case 23:
                    {
                        //Debug.Log("Lightning Bolt hit Player");                          
                    }
                    break;

                //ENEMY HITS PLAYER
                case 24:
                    {
                        //Debug.Log("Enemy attacked Player");
                        if (m_playerStatScript != null)
                        {
                            m_playerStatScript.DamagePlayer(m_fZombieDamageToPlayer);
                        }
                    }
                    break;

                default:
                    break;
                    #endregion
            }
        }
        else
        {
            Debug.LogError(gameObjectHit.name + " has no Hit Label");
        }
    }

    private void OnDisable()
    {
        HitEncountered.hitEncounteredEvent -= GetHitReaction;
    }

    //Added by Petra
    #region Accessors
    //Name: Get[Variable]
    //Ins:
    //Outs: [Variable]
    public float GetMagicBoltDamage()
    {
        return m_fMagicBoltDamageToEnemies;
    }
    public float GetFireballDamage()
    {
        return m_fFireballDamageToEnemies;
    }
    #endregion

    #region Mutators
    //Name: Set[Variable]
    //Ins: newVariable
    //Outs:
    //Function: Sets the value of [Variable] to be equal to newVariable
    public void SetMagicBoltDamage(float f_newMagicBoltDamage)
    {
        m_fMagicBoltDamageToEnemies = f_newMagicBoltDamage;
    }
    public void SetFireballDamage(float f_newFireballDamage)
    {
        m_fFireballDamageToEnemies = f_newFireballDamage;
    }

    //Name: Sum[Variable]
    //Ins: deltaVariable
    //Outs:
    //Function: Variable += deltaVariable.
    public void SumMagicBoltDamage(float f_deltaMagicBoltDamage)
    {
        m_fMagicBoltDamageToEnemies += f_deltaMagicBoltDamage;
    }
    public void SumFireballDamage(float f_deltaFireballDamage)
    {
        m_fFireballDamageToEnemies += f_deltaFireballDamage;
    }
    #endregion
}
