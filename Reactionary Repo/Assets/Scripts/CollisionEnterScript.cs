using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnterScript : MonoBehaviour
{
    [SerializeField] public ePROJECTILES n_ProjectileType = ePROJECTILES.MAGICBOLT;

    private void OnCollisionEnter(Collision collision)
    {
        HitEncountered.SendHitResponse(n_ProjectileType, collision.gameObject, gameObject);
        if (n_ProjectileType == ePROJECTILES.MAGICBOLT)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(n_ProjectileType == ePROJECTILES.COLDCONE)
        {
            HitEncountered.SendHitResponse(n_ProjectileType, other.gameObject, gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(n_ProjectileType == ePROJECTILES.ENEMY)
        {
            HitEncountered.SendHitResponse(n_ProjectileType, other.gameObject, gameObject);
        }
    }
}
