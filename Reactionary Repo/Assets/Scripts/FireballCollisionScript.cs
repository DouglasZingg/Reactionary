using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballCollisionScript : MonoBehaviour
{
    [SerializeField] ParticleSystem g_particleSystem = null;
    [SerializeField] GameObject g_AOEColliderObject = null;
    [SerializeField] GameObject g_sphere = null;
    public void Start()
    {
        g_particleSystem.Stop();
    }
    private void OnCollisionEnter(Collision collision)
    {
        HitLabel hitLabel = collision.gameObject.GetComponent<HitLabel>();
        if(NullCheckScript.NullCheckElseError(hitLabel, "Fireball collided with untagged object!"))
        {
            if (hitLabel.GetLabel() != eHIT.UNAFFECTED)
            {
                SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.FireballExplosion);
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                g_sphere.SetActive(false);
                g_AOEColliderObject.SetActive(true);
                Invoke("DisableAOE", 0.5f);
                
                g_particleSystem.Play();
                Destroy(gameObject, 10.0f);
            }
        }
    }

    private void DisableAOE()
    {
        g_AOEColliderObject.SetActive(false);
    }
}
