using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum eSTATUS
{
    BURN, FREEZE, STUN, _endList
}

public class StatusEffectManagerScript : MonoBehaviour
{
    [SerializeField] private EnemyCommonsScript m_cEnemyCommons = null;
    private EnemyStatScript m_cEnemyStats = null;
    private NavMeshAgent m_navMeshAgent = null;
    private ParticleSystem m_burnParticle = null;
    private ParticleSystem m_freezeParticle = null;
    private ParticleSystem m_stunParticle = null;

    [SerializeField] private float m_fBurnDamage = 1.0f;
    [SerializeField] private float m_fBurnFrequency = 1.0f;
    [SerializeField] private float m_fFreezeSlowAmount = 1.3f;
    [SerializeField] private float m_fFreezeFrequency = 1.5f;

    [SerializeField] private bool m_bBurnImmune = false;
    [SerializeField] private bool m_bFreezeImmune = false;
    [SerializeField] private bool m_bStunImmune = false;
    private float m_fBurnStartTime = 0.0f;
    private float m_fFreezeStartTime = 0.0f;
    private float m_fStunStartTime = 0.0f;
    private float m_fBurnTimer = 0.0f;
    private float m_fFreezeTimer = 0.0f;
    private float m_fStunTimer = 0.0f;
    private float m_fEnemyBaseSpeed = 0.0f;
    [SerializeField] UnityEngine.UI.Image i_fireImage;
    [SerializeField] UnityEngine.UI.Image i_iceImage;
    [SerializeField] UnityEngine.UI.Image i_stunImage;
    [SerializeField] UnityEngine.UI.Image i_fireBackImage;
    [SerializeField] UnityEngine.UI.Image i_iceBackImage;
    [SerializeField] UnityEngine.UI.Image i_stunBackImage;

    // Start is called before the first frame update

    public void ClearTimers()
    {
        ClearTimer(eSTATUS.BURN);
        ClearTimer(eSTATUS.FREEZE);
        ClearTimer(eSTATUS.STUN);
    }
    public void ClearTimer(eSTATUS timer)
    {
        switch (timer)
        {
            case eSTATUS.BURN:
                {
                    m_fBurnTimer = 0.0f;
                    m_burnParticle.Stop();
                    Debug.Log("Cleared burn status");
                    break;
                }
            case eSTATUS.FREEZE:
                {
                    if(m_navMeshAgent)
                    {
                        m_navMeshAgent.speed = m_fEnemyBaseSpeed;
                    }
                    m_fFreezeTimer = 0.0f;
                    m_freezeParticle.Stop();
                    Debug.Log("Cleared freeze status");
                    break;
                }
            case eSTATUS.STUN:
                {
                    if(m_navMeshAgent.isActiveAndEnabled)
                    {
                        m_navMeshAgent.isStopped = false;
                    }
                    m_fStunTimer = 0.0f;
                    m_stunParticle.Stop();
                    Debug.Log("Cleared stun status");
                    break;
                }
            default:
                {
                    m_stunParticle.Pause();
                    Debug.LogWarning("Called StatusEffectManager.ClearTimer on invalid status condition.");
                    break;
                }
        }
    }

    public void AddTimer(eSTATUS type, float duration)
    {
        switch (type)
        {
            case eSTATUS.BURN:
                {
                    m_burnParticle.Play();
                    if(m_fBurnTimer == 0.0f)
                    {
                        m_fBurnStartTime = Time.time;
                        //Debug.Log("Applied burn");
                    }
                    m_fBurnTimer += duration;
                    break;
                }
            case eSTATUS.FREEZE:
                {
                    m_freezeParticle.Play();
                    if (m_fFreezeTimer == 0.0f)
                    {
                        m_fFreezeStartTime = Time.time;
                        //Debug.Log("Applied freeze");
                    }
                    m_fFreezeTimer += duration;
                    break;
                }
            case eSTATUS.STUN:
                {
                    m_stunParticle.Play();
                    if (m_navMeshAgent.isActiveAndEnabled)
                    {
                        m_navMeshAgent.isStopped = true;
                    }
                    if (m_fStunTimer == 0.0f)
                    {
                        m_fStunStartTime = Time.time;
                        //Debug.Log("Applied stun");
                    }
                    m_fStunTimer += duration;
                    break;
                }
            default:
                {
                    Debug.LogWarning("Called StatusEffectManager.AddTimer on invalid status condition.");
                    break;
                }
        }
    }
    private void OnStartOrEnable()
    {
        m_cEnemyStats = m_cEnemyCommons.GetEnemyStatScript();
        m_navMeshAgent = m_cEnemyCommons.GetNavMeshAgent();
        m_burnParticle = m_cEnemyCommons.GetBurnParticle();
        m_freezeParticle = m_cEnemyCommons.GetFreezeParticle();
        m_stunParticle = m_cEnemyCommons.GetStunParticle();
        if (m_navMeshAgent)
        {
            m_fEnemyBaseSpeed = m_navMeshAgent.speed;
        }
        else
        {
            Debug.LogError("StatusEffectManager could not find NavMeshAgent on " + gameObject.name);
        }
    }

    private void Start()
    {
        OnStartOrEnable();

        if (NullCheckScript.NullCheckElseWarning(i_fireImage, "i_fireImage in EnemyStateScript EnemyHealthBar() was null")) { }
        if (NullCheckScript.NullCheckElseWarning(i_iceImage, "i_iceImage in EnemyStateScript EnemyHealthBar() was null")) { }
        if (NullCheckScript.NullCheckElseWarning(i_stunImage, "i_stunImage in EnemyStateScript EnemyHealthBar() was null")) { }
        if (NullCheckScript.NullCheckElseWarning(i_fireBackImage, "i_fireBackImage in EnemyStateScript EnemyHealthBar() was null")) { }
        if (NullCheckScript.NullCheckElseWarning(i_iceBackImage, "i_iceBackImage in EnemyStateScript EnemyHealthBar() was null")) { }
        if (NullCheckScript.NullCheckElseWarning(i_stunBackImage, "i_stunBackImage in EnemyStateScript EnemyHealthBar() was null")) { }
    }

    private void OnEnable()
    {
        OnStartOrEnable();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_cEnemyStats != null)
        {
            if (m_cEnemyStats.GetCurrentHP() > 0)
            {
                if (m_fBurnTimer > 0.0f)
                {
                    if ((Time.time - m_fBurnStartTime) % m_fBurnFrequency <= 0.005f)
                    {
                        //Do burn damage
                        m_cEnemyStats.SumCurrentHP(-1 * m_fBurnDamage);
                        //Debug.Log("Status effect applied " + m_fBurnDamage + " burn damage to " + gameObject.name);
                        i_fireImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                        i_fireBackImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    if (Time.time >= m_fBurnTimer + m_fBurnStartTime)
                    {
                        ClearTimer(eSTATUS.BURN);
                        i_fireImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
                        i_fireBackImage.GetComponent<UnityEngine.UI.Image>().enabled = false;

                    }
                }
                if (m_fFreezeTimer > 0.0f)
                {
                    if ((Time.time - m_fFreezeStartTime) % m_fFreezeFrequency <= 0.005f)
                    {
                        if(m_navMeshAgent.isActiveAndEnabled)
                        {
                            m_navMeshAgent.speed -= m_fFreezeSlowAmount;
                        }
                        //Debug.Log("Status effect applied " + m_fFreezeSlowAmount + " freeze slowing to " + gameObject.name);
                        i_iceImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                        i_iceBackImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    if (Time.time >= m_fFreezeTimer + m_fFreezeStartTime)
                    {
                        ClearTimer(eSTATUS.FREEZE);
                        i_iceImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
                        i_iceBackImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                }
                if (m_fStunTimer > 0.0f)
                {
                    i_stunImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    i_stunBackImage.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    if (Time.time >= m_fStunTimer + m_fStunStartTime)
                    {
                        ClearTimer(eSTATUS.STUN);
                        i_stunBackImage.GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("StatusEffectManagerScript did not find EnemyStatScript.");
        }
        
    }

    public bool GetBurnImmunity()
    {
        return m_bBurnImmune;
    }
    public bool GetFreezeImmunity()
    {
        return m_bFreezeImmune;
    }
    public bool GetStunImmunity()
    {
        return m_bStunImmune;
    }
}
