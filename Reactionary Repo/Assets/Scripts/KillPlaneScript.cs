using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlaneScript : MonoBehaviour
{
    [SerializeField] GameObject g_Spawn = null;

    //Function: If player collides with the killplane, teleport player to playerSpawn's transform.
    private void OnCollisionEnter(Collision collision)
    {
        Transform SpawnTransform = g_Spawn.transform;
        collision.gameObject.transform.SetPositionAndRotation(SpawnTransform.position, SpawnTransform.rotation);
    }
}
