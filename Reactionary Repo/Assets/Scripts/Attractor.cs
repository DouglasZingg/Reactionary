using UnityEngine;

public class Attractor : MonoBehaviour
{
    [SerializeField] float f_speed;

    void Update()
    {
        if (transform.childCount < 1)
        {
            Destroy(gameObject);
        }   
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = Vector3.MoveTowards(transform.position, other.transform.position, f_speed * Time.deltaTime);
        } 
    }
}
