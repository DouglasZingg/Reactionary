using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] float f_speed = 5.0f;
    [SerializeField] float f_height = 0.25f;
    [SerializeField] bool b_switchPickUp = true;
    [SerializeField] Vector3 v_pickUpPosition;
    [SerializeField] GameObject g_attractor;
    [SerializeField] HealthBar g_healthBar;
    [SerializeField] ManaBar g_manaBar;

    void Update()
    {
        transform.Rotate(new Vector3(0, 15, 0) * Time.deltaTime);
        float newY = Mathf.Sin(Time.time * f_speed) * f_height + v_pickUpPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (b_switchPickUp)
        {
            g_healthBar.health += 25.0f;
        }
        else
        {
            g_manaBar.f_mana += 25.0f;
        }

        gameObject.SetActive(false);
        Destroy(g_attractor);
    }
}
