using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltCollisionScript : MonoBehaviour
{
    //[SerializeField] ParticleSystem g_ParticleSystem = null;
    [SerializeField] GameObject g_ChainColliderObject = null;
    [SerializeField] GameObject g_BoltObject = null;

    SphereCollider g_ChainCollider = null;
    private void Start()
    {
        //g_ParticleSystem.Stop();
        g_ChainCollider = g_ChainColliderObject.GetComponent<SphereCollider>();
        g_ChainCollider.enabled = false;
    }

    private void OnEnable()
    {
        g_ChainCollider = g_ChainColliderObject.GetComponent<SphereCollider>();
        g_ChainCollider.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Layer check / Tag check to confirm collision is with an enemy or terrain.
        //Explosion goes here;
        if(collision.gameObject.TryGetComponent<HitLabel>(out HitLabel hitLabel))
        {
            if (hitLabel.GetLabel() != eHIT.UNAFFECTED)
            {
                //SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.FireballExplosion);
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                g_BoltObject.SetActive(false);
                g_ChainCollider.enabled = true;
                //g_ParticleSystem.Play();
                Destroy(gameObject, 0.5f);
            }
        }        
    }
}
