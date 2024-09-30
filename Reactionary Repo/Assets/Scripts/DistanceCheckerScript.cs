using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCheckerScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    void Start()
    {
        player = GameObject.Find("PlayerHands");

    }
    private void Update()
    {
        Debug.Log("Distance from player: " + Vector3.Distance(gameObject.transform.position, player.transform.position));
        //
    }
}
