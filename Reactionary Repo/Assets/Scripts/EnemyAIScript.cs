using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting;
public class EnemyAIScript : MonoBehaviour
{
    public Transform target;

    [SerializeField] private EnemyCommonsScript m_cEnemyCommons = null;
    private EnemyStatScript m_cEnemyStatScript = null;
    private NavMeshAgent m_navMeshAgent = null;
    private StatusEffectManagerScript m_cStatusEffectScript = null;

    [SerializeField] private CapsuleCollider m_capsuleCollider = null;
    [SerializeField] private Animator m_Animator = null;
    
    [SerializeField] private GameObject m_leftArmObject = null;
    private CapsuleCollider m_leftArmCollider = null;

    [SerializeField] private GameObject m_rightArmObject = null;
    private CapsuleCollider m_rightArmCollider = null;

    [SerializeField] private GameObject m_headObject = null;
    private SphereCollider m_headCollider = null;

    [SerializeField] private Material m_ZombieDissolve;
    [SerializeField] private Material m_ZombieAlwaysVisible;


    private float m_fAttackDistance = -1;
    private bool m_bDeathSwitch = false;

    [SerializeField] private GameObject m_Ragdoll;

    [SerializeField] private GameObject m_AnimatedModel;
    [SerializeField] private GameObject m_BodyObject;
    private SkinnedMeshRenderer m_HeadRenderer = null;
    private SkinnedMeshRenderer m_BodyRenderer = null;
    public ePROJECTILES m_LastHitBy { get; set; }
    public Vector3 m_lastAppliedForce { get; set; }

    [SerializeField] private float m_fAttackSpeed = 0.8f;
    private float m_fLastTimeAttacked;

    [SerializeField] Rebuild g_switchTarget = null;

    private void SetDissolveScriptsActive(bool active)
    {
        RagdollDissolveScript[] dissolveScripts = m_AnimatedModel.GetComponentsInChildren<RagdollDissolveScript>();
        for (int i = 0; i < dissolveScripts.Length; i++)
        {
            dissolveScripts[i].enabled = active;
            dissolveScripts[i].ResetValues();
        }
        dissolveScripts = m_Ragdoll.GetComponentsInChildren<RagdollDissolveScript>();
        for (int i = 0; i < dissolveScripts.Length; i++)
        {
            dissolveScripts[i].enabled = active;
            dissolveScripts[i].ResetValues();
        }
    }

    private void OnStartOrEnable()
    {
        m_AnimatedModel.SetActive(true);
        m_Ragdoll.SetActive(false);

        target = GameObject.Find("CzechHedgehog").transform;
        g_switchTarget = GameObject.Find("CzechHedgehog").GetComponent<Rebuild>();
        m_navMeshAgent = m_cEnemyCommons.GetNavMeshAgent();
        m_cEnemyStatScript = m_cEnemyCommons.GetEnemyStatScript();
        m_cStatusEffectScript = m_cEnemyCommons.GetStatusEffectManager();
        m_cEnemyCommons.SetLabel(eHIT.ENEMY);

        m_rightArmCollider = m_rightArmObject.GetComponent<CapsuleCollider>();

        m_leftArmCollider = m_leftArmObject.GetComponent<CapsuleCollider>();

        m_headCollider = m_headObject.GetComponent<SphereCollider>();

        m_HeadRenderer = m_headObject.GetComponent<SkinnedMeshRenderer>();
        m_HeadRenderer.materials[0] = m_ZombieAlwaysVisible;

        m_BodyRenderer = m_BodyObject.GetComponent<SkinnedMeshRenderer>();
        m_BodyRenderer.materials[0] = m_ZombieAlwaysVisible;

        m_navMeshAgent.enabled = true;
        m_fAttackDistance = m_navMeshAgent.stoppingDistance;
        m_cEnemyStatScript.SetCurrentHP(m_cEnemyStatScript.GetMaxHP());
        m_capsuleCollider.enabled = true;
        m_bDeathSwitch = false;
        m_Animator.SetFloat("MoveSpeed", 1.0f);
        m_headCollider.enabled = true;

        m_cEnemyStatScript.m_enemyHealthCanvas.enabled = true;
        m_cEnemyCommons.SetDissolveScriptsActive(false);
        
    }
    public void Start()
    {
        OnStartOrEnable();
    }

    public void OnEnable()
    {
        Debug.Log("ENABLED");
        OnStartOrEnable();
    }

    public void Update()
    {
        //Only find player once, update destination to position every frame.
        //If target is null, re-find in navMesh.enabled check.
        //Refactor targeting/line of sight code when Wizard's keep.
        //Attack distance and navMesh stopping distance should be equal, so that tenemies don't keep trying to bump into the player.
        //When attack starts, disable navMesh until attack ends.
        //If not in attack distance re-enable navMesh
        //If in attack distance check if animation is finished, attack again.
        if (!m_bDeathSwitch)
        {
            if (m_cEnemyStatScript.GetCurrentHP() <= 0)
            {
                m_bDeathSwitch = true;
                Death();
            }
            else
            {
                if (m_navMeshAgent.enabled)
                {
                    if(target == null)
                    {
                        //TODO: Expand targeting system to target Wizard's Keep when player is not within "Line of Sight". Might want to give a cooldown on changing targets to prevent player exploiting the AI.
                        target = GameObject.Find("PlayerHands").transform;
                    }
                    m_navMeshAgent.SetDestination(target.transform.position);
                }
                //One of the following solutions (rate of fire first):
                //1. Implement a time based fire-rate on zombie
                //2. Check if animation is finished (animation clip info)
                
                if((Time.time - m_fLastTimeAttacked >= m_fAttackSpeed) && (Vector3.Distance(target.position, this.gameObject.transform.position) < m_fAttackDistance))
                {
                    m_Animator.SetTrigger("Attack");
                    m_fLastTimeAttacked = Time.time;
                }
            }
        }

        if (g_switchTarget.i_progressBar.fillAmount == 0.0f)
            target = GameObject.Find("PlayerHands").transform;
        if (g_switchTarget.i_progressBar.fillAmount == 1.0f)
            target = GameObject.Find("CzechHedgehog").transform;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    public void Death()
    {
        m_cStatusEffectScript.ClearTimers();
        SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.EnemyDied);
        DisableMeleeColliders();
        m_headCollider.enabled = false;
        m_capsuleCollider.enabled = false;
        m_navMeshAgent.enabled = false;

        m_HeadRenderer.materials[0].shader = m_ZombieDissolve.shader;
        m_BodyRenderer.materials[0].shader = m_ZombieDissolve.shader;

        //if died by cone of cold or explosion, ragdoll
        if (m_LastHitBy == ePROJECTILES.FIREBALL || m_LastHitBy == ePROJECTILES.COLDCONE)
        {
            CopyTranformToRagdoll(m_AnimatedModel.transform, m_Ragdoll.transform);
            m_AnimatedModel.SetActive(false);
            m_Ragdoll.SetActive(true);

        }
        //else, death animation
        else
        {
            m_Animator.SetBool("Death", true);
        }
        m_cEnemyCommons.SetDissolveScriptsActive(true);
        WaveManager.g_cInstance.EnemyDied();
        m_cEnemyStatScript.m_enemyHealthCanvas.enabled = false;

        Invoke("Despawn", 10.75f);
        
    }

    public void EnableMeleeColliders()
    {
        if(m_cEnemyStatScript.GetCurrentHP() > 0)
        {
            m_leftArmCollider.enabled = true;
            m_rightArmCollider.enabled = true;
        }
    }

    public void DisableMeleeColliders()
    {
        m_leftArmCollider.enabled = false;
        m_rightArmCollider.enabled = false;
    }

    //Copies the children transforms to match when the enemy died
    private void CopyTranformToRagdoll(Transform sourceTransform, Transform destinationTransform)
    {
        if (sourceTransform.childCount == destinationTransform.childCount)
        {
            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                Transform source = sourceTransform.GetChild(i);
                Transform destination = destinationTransform.GetChild(i);

                destination.position = source.position;
                destination.rotation = source.rotation;

                Rigidbody rigidBody = destination.GetComponent<Rigidbody>();

                if (rigidBody != null)
                {
                    rigidBody.velocity = m_lastAppliedForce * 15;
                }

                CopyTranformToRagdoll(source, destination);
            }
        }
    }
}