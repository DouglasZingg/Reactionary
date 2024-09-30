using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class JumpBar : MonoBehaviour
{
    public Image jumpBar;

    public float cooldown = 10.0f;

    public FPSMovementScript movementScript;

    private void Update()
    {
        if(!movementScript.isCooldown)
        {
            jumpBar.fillAmount += 1 / cooldown * Time.deltaTime;

            if(jumpBar.fillAmount >= 1)
            {
                movementScript.isCooldown = true;
            }
        }
    }

    //Name: Get[Variable]
    //Ins:
    //Outs: [Variable]
    public float GetCooldown()
    {
        return cooldown;
    }
    public bool GetIsCooldown()
    {
        return movementScript.isCooldown;
    }

    //Name: Set[Variable]
    //Ins: newVariable
    //Outs:
    //Function: Sets the value of [Variable] to be equal to newVariable
    public void SetCooldown(float f_newCooldown)
    {
        cooldown = f_newCooldown;
    }
    public void SetIsCooldown(bool b_newIsCooldown)
    {
        movementScript.isCooldown = b_newIsCooldown;
    }

    //Name: Sum[Variable]
    //Ins: deltaVariable
    //Outs:
    //Function: Variable += deltaVariable.
    public void SumCooldown(float f_deltaCooldown)
    {
        cooldown += f_deltaCooldown;
    }


    //Name: Toggle[Variable]
    //Ins:
    //Outs:
    //Function: Inverts the value of [Variable]
    public void ToggleIsCooldown()
    {
        movementScript.isCooldown = !movementScript.isCooldown;
    }
}