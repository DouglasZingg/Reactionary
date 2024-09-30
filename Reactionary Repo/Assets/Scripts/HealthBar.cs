using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text healthText;
    public Image i_frontHealthBar;
    public Image i_backHealthBar;

    Color color;

    public float health, maxHealth = 200;
    float lerpSpeed, counter;

    bool b_flash = false;

    private void Start()
    {
        health = maxHealth;
        color = i_frontHealthBar.color;
    }

    private void Update()
    {
        healthText.text = health + "";

        if (health > maxHealth)
            health = maxHealth;
        if (health <= 0.0f)
            health = 0.0f;



        lerpSpeed = 3.0f * Time.deltaTime;

        //if (Input.GetKeyDown("a"))
        //    health = health - 10.0f;
        //if (Input.GetKeyDown("d"))
        //    health = health + 10.0f;

        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {
        float f_fillFront = i_frontHealthBar.fillAmount;
        float f_fillBack = i_backHealthBar.fillAmount;
        float f_hFraction = health / maxHealth;
        if (f_fillBack > f_hFraction)
        {
            i_frontHealthBar.fillAmount = f_hFraction;
            i_backHealthBar.color = Color.red;
            lerpSpeed += Time.deltaTime;
            float f_percentComplete = lerpSpeed / 2.0f;
            i_backHealthBar.fillAmount = Mathf.Lerp(f_fillBack, f_hFraction, f_percentComplete);
        }
        if (f_fillFront < f_hFraction)
        {
            i_backHealthBar.color = Color.red;
            i_backHealthBar.fillAmount = f_hFraction;
            lerpSpeed += Time.deltaTime;
            float f_percentComplete = lerpSpeed / 2.0f;
            i_frontHealthBar.fillAmount = Mathf.Lerp(f_fillFront, i_backHealthBar.fillAmount, f_percentComplete);
        }
    }

    void ColorChanger()
    {
        Color c_darkYellow = new Color(1.0f, 0.81f, 0.0f);
        Color healthColor = Color.Lerp(c_darkYellow, color, (health / maxHealth));

        if (health <= (maxHealth * 0.3f))
            FlashHealthBar();
        else
            i_frontHealthBar.color = healthColor;
    }

    void FlashHealthBar()
    {
        counter += Time.deltaTime;

        if (counter > 0.5f)
        {
            i_frontHealthBar.color = Color.white;
        }
        else
            i_frontHealthBar.color = Color.red;

        if (counter > 1.0f)
            counter = 0;
    }

    //Added by Petra
    #region Accessors
    //Name: Get[Variable]
    //Ins:
    //Outs: [Variable]
    public float GetMaxHP()
    {
        return maxHealth;
    }
    public float GetCurrentHP()
    {
        return health;
    }
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    #endregion

    #region Mutators
    //Name: Set[Variable]
    //Ins: newVariable
    //Outs:
    //Function: Sets the value of [Variable] to be equal to newVariable
    public void SetMaxHP(float f_newMaxHP)
    {
        maxHealth = f_newMaxHP;
    }
    public void SetCurrentHP(float f_newCurrentHP)
    {
        health = f_newCurrentHP;
    }
    public void SetLerpSpeed(float f_newLerpSpeed)
    {
        lerpSpeed = f_newLerpSpeed;
    }

    //Name: Sum[Variable]
    //Ins: deltaVariable
    //Outs:
    //Function: Variable += deltaVariable.
    public void SumMaxHP(float f_deltaMaxHP)
    {
        maxHealth += f_deltaMaxHP;
    }
    public void SumCurrentHP(float f_deltaCurrentHP)
    {
        health += f_deltaCurrentHP;
    }
    public void SumLerpSpeed(float f_deltaLerpSpeed)
    {
        lerpSpeed += f_deltaLerpSpeed;
    }
    #endregion
}
