using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableHeadbangerMelee : MonoBehaviour
{
    [SerializeField] private EnemyAIScript m_cScript;

    public void EnableMeleeColliders()
    {
        m_cScript.EnableMeleeColliders();
    }

    public void DisableMeleeColliders()
    {
        m_cScript.DisableMeleeColliders();
    }
}
