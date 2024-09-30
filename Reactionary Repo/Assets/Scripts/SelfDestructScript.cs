using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructScript : MonoBehaviour
{
    [SerializeField] public float g_fLifetime;
    private float g_fSpawnTime = 0.0f;

    private void Start()
    {
        g_fSpawnTime = Time.time;
    }

    private void OnEnable()
    {
        //g_fRemainingLifetime = g_fLifetime;
        g_fSpawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > g_fSpawnTime + g_fLifetime)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        g_fSpawnTime = Time.time;
    }
}
