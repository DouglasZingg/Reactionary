using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject m_ChildProjectile = null;
    [SerializeField] private SphereCollider m_SphereCollider = null;
    [SerializeField] private float m_fChildVelocity = 10.0f;
    //private List<GameObject> enemies
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out HitLabel label))
        {
            if (m_ChildProjectile != null && label.GetLabel() == eHIT.ENEMY)
            {
                GameObject clone;
                clone = Instantiate(m_ChildProjectile, gameObject.transform.position, m_ChildProjectile.transform.rotation);
                if (clone != null)
                {
                    clone.gameObject.layer = 10;
                    clone.gameObject.SetActive(true);
                    clone.transform.LookAt(other.transform);
                    clone.GetComponent<Rigidbody>().velocity = clone.transform.forward * m_fChildVelocity;
                }
            }
        }

    }
}
