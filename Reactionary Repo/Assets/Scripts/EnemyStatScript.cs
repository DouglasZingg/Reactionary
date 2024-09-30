using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStatScript : MonoBehaviour
{
    [SerializeField] private EnemyCommonsScript m_cEnemyCommons = null;
    [SerializeField] Image m_enemyHealthBar = null;
    [SerializeField] public Canvas m_enemyHealthCanvas = null;

    [SerializeField] private float m_fMaxHealth = 10;
    [SerializeField] private float m_fCurrentHealth = 10;

    [SerializeField] private float m_fGenericResistance = 1.0f;
    [SerializeField] private float m_fFireResistance = 1.0f;
    [SerializeField] private float m_fIceResistance = 1.0f;
    [SerializeField] private float m_fLightningResistance = 1.0f;

    public void Update()
    {
        EnemyHealthBar();
    }


    #region Accessors
    public float GetMaxHP()
    {
        return m_fMaxHealth;
    }
    public float GetCurrentHP()
    {
        return m_fCurrentHealth;
    }
    public float GetGenericResistance()
    {
        return m_fGenericResistance;
    }
    public float GetFireResistance()
    {
        return m_fFireResistance;
    }
    public float GetIceResistance()
    {
        return m_fIceResistance;
    }
    public float GetLightningResistance()
    {
        return m_fLightningResistance;
    }
    #endregion

    #region Mutators
    public void SetMaxHP(float f_newMaxHP)
    {
        m_fMaxHealth = f_newMaxHP;
    }
    public void SetCurrentHP(float f_newCurrentHP)
    {
        m_fCurrentHealth = f_newCurrentHP;
    }

    public void SumMaxHP(float f_deltaMaxHP)
    {
        m_fMaxHealth += f_deltaMaxHP;
    }
    public void SumCurrentHP(float f_deltaCurrentHP)
    {
        m_fCurrentHealth += f_deltaCurrentHP;
    }
    public void SetGenericResistance(float amount)
    {
        m_fGenericResistance = amount;
    }
    public void SetFireResistance(float amount)
    {
        m_fFireResistance = amount;
    }
    public void SetIceResistance(float amount)
    {
        m_fIceResistance = amount;
    }
    public void SetLightningResistance(float amount)
    {
        m_fLightningResistance = amount;
    }
    #endregion

    public void EnemyHealthBar()
    {
        if (NullCheckScript.NullCheckElseWarning(m_enemyHealthBar, "m_enemyHealthBar in EnemyStatScript's EnemyHealthBar() was null"))
        {
            GameObject g_playerHands = GameObject.Find("PlayerHands");
            if (NullCheckScript.NullCheckElseWarning(g_playerHands, "g_playerHands in EnemyStateScript EnemyHealthBar() was null"))
            {
                m_enemyHealthBar.fillAmount = Mathf.Lerp(m_enemyHealthBar.fillAmount, m_fCurrentHealth / m_fMaxHealth, (3.0f * Time.deltaTime));
                Vector3 v_playerPos = new Vector3(g_playerHands.transform.position.x, transform.position.y, g_playerHands.transform.position.z);
                m_enemyHealthCanvas.transform.LookAt(v_playerPos);
            }
        }
    }
}
