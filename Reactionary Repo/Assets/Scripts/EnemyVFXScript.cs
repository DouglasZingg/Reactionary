using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFXScript : MonoBehaviour
{
    [SerializeField] private EnemyCommonsScript m_cEnemyCommons = null;
    [SerializeField] private GameObject m_headObject = null;
    [SerializeField] private GameObject m_bodyObject = null;
    private SkinnedMeshRenderer m_headRenderer = null;
    private SkinnedMeshRenderer m_bodyRenderer = null;
    private Color m_headColor;
    private Color m_bodyColor;

    [SerializeField] private float m_lerpSpeed;
    [SerializeField] private Material m_ZombieDissolve;
    [SerializeField] private Material m_ZombieAlwaysVisible;

    #region StartOrEnable
    public void Start()
    {
        OnStartOrEnable();
    }

    public void OnEnable()
    {
        OnStartOrEnable();
    }

    private void OnStartOrEnable()
    {
        m_headRenderer = m_headObject.GetComponent<SkinnedMeshRenderer>();
        m_bodyRenderer = m_bodyObject.GetComponent<SkinnedMeshRenderer>();
        if (NullCheckScript.NullCheckElseWarning(m_headRenderer, "m_headRenderer in EnemyVisualManager's OnStartOrEnable() was null"))
        {
            m_headRenderer.materials[0] = m_ZombieAlwaysVisible;
            m_headColor = m_headRenderer.material.color;
            if (NullCheckScript.NullCheckElseError(m_bodyRenderer, "m_bodyRenderer in EnemyVisualManager's OnStartOrEnable() was null"))
            {
                m_bodyRenderer.materials[0] = m_ZombieAlwaysVisible;
                m_bodyColor = m_bodyRenderer.material.color;
            }
            SetDissolveScriptsActive(false);
        }
        else
        {

        }
    }
    #endregion

    #region DissolveShaders
    public void SetDissolveScriptsActive(bool active)
    {
        if (NullCheckScript.NullCheckElseWarning(m_ZombieDissolve, "EnemyVFXScript's m_ZombieDissolve was null in SetDissolveScriptsActive()")
                && NullCheckScript.NullCheckElseWarning(m_headRenderer, "EnemyVFXScript's m_headRenderer was null in SetDissolveScriptsActive()")
                && NullCheckScript.NullCheckElseWarning(m_bodyRenderer, "EnemyVFXScript's m_bodyRenderer was null in SetDissolveScriptsActive()"))
        {
            Material mat;
            if (active)
            {
                mat = m_ZombieDissolve;
            }
            else
            {
                mat = m_ZombieAlwaysVisible;
            }
            m_headRenderer.material = mat;
            m_bodyRenderer.material = mat;
        }
        else
        {

        }
        RagdollDissolveScript[] dissolveScripts = m_cEnemyCommons.GetAnimatedModel().GetComponentsInChildren<RagdollDissolveScript>();
        for (int i = 0; i < dissolveScripts.Length; i++)
        {
            dissolveScripts[i].enabled = active;
            dissolveScripts[i].ResetValues();
        }
        dissolveScripts = m_cEnemyCommons.GetRagdoll().GetComponentsInChildren<RagdollDissolveScript>();
        for (int i = 0; i < dissolveScripts.Length; i++)
        {
            dissolveScripts[i].enabled = active;
            dissolveScripts[i].ResetValues();
        }
    }
    #endregion

    #region Colors
    public void SetHeadColor(Color _color)
    {
        if (NullCheckScript.NullCheckElseWarning(m_headRenderer, "m_headRenderer in EnemyStatScript's SetHeadColor was null"))
        {
            m_headRenderer.material.color = _color;
        }
    }
    public void SetBodyColor(Color _color)
    {
        if (NullCheckScript.NullCheckElseWarning(m_bodyRenderer, "m_bodyRenderer in EnemyStatScript's SetBodyColor was null"))
        {
            m_bodyRenderer.material.color = _color;
        }
    }

    private void Update()
    {
        if (NullCheckScript.NullCheckElseWarning(m_headRenderer, "m_headRenderer in EnemyStatScript's Update was null"))
        {
            m_headRenderer.material.color = Color.Lerp(m_headRenderer.material.color, m_headColor, m_lerpSpeed);
        }
        if (NullCheckScript.NullCheckElseWarning(m_bodyRenderer, "m_bodyRenderer in EnemyStatScript's Update was null"))
        {
            m_bodyRenderer.material.color = Color.Lerp(m_bodyRenderer.material.color, m_bodyColor, m_lerpSpeed);
        }
    }
    #endregion
}
