using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollisionScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Layer check / Tag check to confirm collision is with an enemy or terrain.
        //Explosion goes here;
        HitLabel hitLabel = collision.gameObject.GetComponent<HitLabel>();
        if (hitLabel != null && hitLabel.GetLabel() != eHIT.UNAFFECTED && hitLabel.GetLabel() != eHIT.PLAYER)
        {
            gameObject.SetActive(false);
        }
    }
}
