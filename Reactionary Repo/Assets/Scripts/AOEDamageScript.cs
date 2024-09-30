using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamageScript : MonoBehaviour
{
    [SerializeField] float m_fInstantDamage = 0.5f;
    [SerializeField] float m_fBurnTimer = 0.01f;

    public void OnTriggerEnter(Collider other)
    {
        HitLabel hitLabel = other.GetComponent<HitLabel>();
        if (NullCheckScript.NullCheckElseWarning(hitLabel, "AOEDamageScript could not find " + other.gameObject.name + "'s hitLabel") && hitLabel.GetLabel() == eHIT.ENEMY) 
        {
            EnemyCommonsScript enemyCommons = other.gameObject.GetComponent<EnemyCommonsScript>();
            if(NullCheckScript.NullCheck(enemyCommons))
            {
                enemyCommons.SumCurrentHP(-1 * m_fInstantDamage);
            }            
        }
    }
    public void OnTriggerStay(Collider other)
    {
        HitLabel hitLabel = other.GetComponent<HitLabel>();
        if (NullCheckScript.NullCheckElseWarning(hitLabel, "AOEDamageScript could not find " + other.gameObject.name + "'s hitLabel") && hitLabel.GetLabel() == eHIT.ENEMY)
        {
            EnemyCommonsScript enemyCommons = other.gameObject.GetComponentInParent<EnemyCommonsScript>();
            if (NullCheckScript.NullCheck(enemyCommons))
            {
                enemyCommons.AddTimer(eSTATUS.BURN, 0.005f);
            }
        }
    }
}
