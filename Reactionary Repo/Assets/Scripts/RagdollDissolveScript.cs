using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDissolveScript : MonoBehaviour
{
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    Material[] mats;
    [SerializeField] private float speed = .1f;
    [SerializeField] private float width = 0.05f;
    [SerializeField] private Color color;

    public void ResetValues()
    {
        m_SkinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
        NullCheckScript.NullCheckElseError(m_SkinnedMeshRenderer, "RagdollDissolveScript's m_MeshRenderer was null");

        mats = m_SkinnedMeshRenderer.materials;
        mats[0].SetFloat("_DissolveAmount", 0.0f);
        mats[0].SetFloat("_DissolveWidth", width);
        mats[0].SetColor("_DissolveColor", color);
    }
    public void OnStartOrEnable()
    {
        ResetValues();
    }
    public void OnStart()
    {
        OnStartOrEnable();
    }
    public void OnEnable()
    {
        OnStartOrEnable();
    }

    private float t = 0.0f;
    public void Update()
    {
        mats[0].SetFloat("_DissolveAmount", mats[0].GetFloat("_DissolveAmount") + (t * speed));
        t += Time.deltaTime;

        // Unity does not allow meshRenderer.materials[0]...
        m_SkinnedMeshRenderer.materials = mats;
    }
}
