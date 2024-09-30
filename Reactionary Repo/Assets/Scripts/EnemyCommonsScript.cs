using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This script exists to manage common enemy components and behaviours. It should cut down on what would otherwise be intensive direct dependencies between enemy prefab script components.
/// In short, it should replace a lot of serialized fields in several scripts with one script that has lots of serialized fields, lowering the total amount of serialized fields.
/// -Petra
/// </summary>
public class EnemyCommonsScript : MonoBehaviour
{
    [SerializeField] private EnemyStatScript m_cEnemyStats = null;
    [SerializeField] private EnemyAIScript m_cEnemyAI = null;
    [SerializeField] private StatusEffectManagerScript m_cStatusEffects = null;
    [SerializeField] private EnemyVFXScript m_cEnemyVisuals = null;
    [SerializeField] private NavMeshAgent m_navMeshAgent = null;
    [SerializeField] private HitLabel m_cHitLabel = null;

    [SerializeField] private GameObject m_animatedModel = null;
    [SerializeField] private GameObject m_ragdoll = null;

    [SerializeField] private GameObject m_headObject = null;
    private SphereCollider m_headCollider = null;
    [SerializeField] private GameObject m_bodyObject = null;
    private CapsuleCollider m_bodyCollider = null;

    [SerializeField] private ParticleSystem m_bloodSplatter = null;
    [SerializeField] private ParticleSystem m_burnParticle = null;
    [SerializeField] private ParticleSystem m_freezeParticle = null;
    [SerializeField] private ParticleSystem m_stunParticle = null;

    #region OnStartOrEnable
    private void NullCheckAll()
    {
        NullCheckScript.NullCheckElseError(m_cEnemyStats, "m_cEnemyStats was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_cEnemyAI, "m_cEnemyAI was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_cStatusEffects, "m_cStatusEffects was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_cEnemyVisuals, "m_EnemyVisuals was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_navMeshAgent, "m_NavMeshAgent was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_cHitLabel, "m_HitLabel was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_headCollider, "m_headCollider was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_bodyCollider, "m_bodyCollider was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_bloodSplatter, "m_BloodSplatter was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_burnParticle, "m_BurnParticle was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_freezeParticle, "m_FreezeParticle was null in EnemyManagerScript's OnStartOrEnable()");
        NullCheckScript.NullCheckElseError(m_stunParticle, "m_StunParticle was null in EnemyManagerScript's OnStartOrEnable()");
    }

    private void OnStartOrEnable()
    {
        NullCheckScript.NullCheckElseError(m_headObject, "m_headObject was null in EnemyManagerScript's OnStartOrEnable()");
        m_headCollider = m_headObject.GetComponent<SphereCollider>();
        NullCheckScript.NullCheckElseError(m_bodyObject, "m_bodyObject was null in EnemyManagerScript's OnStartOrEnable()");
        m_bodyCollider = m_bodyObject.GetComponent<CapsuleCollider>();
        NullCheckAll();
    }
    #endregion

    #region StatusEffects
    public void AddTimer(eSTATUS type, float duration)
    {
        m_cStatusEffects.AddTimer(type, duration);
    }
    public void ClearTimer(eSTATUS type)
    {
        m_cStatusEffects.ClearTimer(type);
    }
    public void ClearTimers()
    {
        m_cStatusEffects.ClearTimer(eSTATUS.BURN);
        m_cStatusEffects.ClearTimer(eSTATUS.FREEZE);
        m_cStatusEffects.ClearTimer(eSTATUS.STUN);
    }
    public bool GetBurnImmunity()
    {
        return m_cStatusEffects.GetBurnImmunity();
    }
    public bool GetFreezeImmunity()
    {
        return m_cStatusEffects.GetFreezeImmunity();
    }
    public bool GetStunImmunity()
    {
        return m_cStatusEffects.GetStunImmunity();
    }
    #endregion

    #region Stats
    public void SumCurrentHP(float f_deltaCurrentHP)
    {
        m_cEnemyStats.SumCurrentHP(f_deltaCurrentHP);
    }
    public void SetBodyColor(Color color)
    {
        m_cEnemyVisuals.SetBodyColor(color);
    }
    public void SetHeadColor(Color color)
    {
        m_cEnemyVisuals.SetHeadColor(color);
    }
    public float GetGenericResistance()
    {
        return m_cEnemyStats.GetGenericResistance();
    }
    public float GetFireResistance()
    {
        return m_cEnemyStats.GetFireResistance();
    }
    public float GetIceResistance()
    {
        return m_cEnemyStats.GetIceResistance();
    }
    public float GetLightningResistance()
    {
        return m_cEnemyStats.GetLightningResistance();
    }
    public void SetGenericResistance(float amount)
    {
        m_cEnemyStats.SetGenericResistance(amount);
    }
    public void SetFireResistance(float amount)
    {
        m_cEnemyStats.SetFireResistance(amount);
    }
    public void SetIceResistance(float amount)
    {
        m_cEnemyStats.SetIceResistance(amount);
    }
    public void SetLightningResistance(float amount)
    {
        m_cEnemyStats.SetLightningResistance(amount);
    }
    #endregion

    #region Particles
    //Why is only bloodsplatter here and not the status effect particles?
    //The way I see it, the status effect particles should be tightly coupled with the manager script, since they'll only ever be enabled when the status effect timer is active.
    //-Petra
    public void PlayBloodSplatter()
    {
        m_bloodSplatter.Play();
    }
    #endregion

    #region Visuals
    public void SetDissolveScriptsActive(bool active)
    {
        m_cEnemyVisuals.SetDissolveScriptsActive(active);
    }
    #endregion

    #region HitLabel
    public void SetLabel(eHIT inLabel)
    {
        m_cHitLabel.SetLabel(inLabel);
    }
    #endregion

    #region Accessors
    public EnemyStatScript GetEnemyStatScript()
    {
        return m_cEnemyStats;
    }
    public EnemyAIScript GetEnemyAIScript()
    {
        return m_cEnemyAI;
    }    
    public StatusEffectManagerScript GetStatusEffectManager()
    {
        return m_cStatusEffects;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return m_navMeshAgent;
    }
    public ParticleSystem GetBloodSplatter()
    {
        return m_bloodSplatter;
    }
    public ParticleSystem GetBurnParticle()
    {
        return m_burnParticle;
    }
    public ParticleSystem GetFreezeParticle()
    {
        return m_freezeParticle;
    }
    public ParticleSystem GetStunParticle()
    {
        return m_stunParticle;
    }
    public eHIT GetLabel()
    {
        return m_cHitLabel.GetLabel();
    }
    public GameObject GetHeadObject()
    {
        return m_headObject;
    }
    public GameObject GetBodyObject()
    {
        return m_bodyObject;
    }
    public GameObject GetAnimatedModel()
    {
        return m_animatedModel;
    }
    public GameObject GetRagdoll()
    {
        return m_ragdoll;
    }
    #endregion
}
