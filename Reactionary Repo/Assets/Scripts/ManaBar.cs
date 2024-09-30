using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Text t_manaText;
    public Image i_manaBar;

    Color c_color;

    public float f_mana, f_maxMana = 100;
    float f_lerpSpeed, f_counter;

    private void Start()
    {
        f_mana = f_maxMana;
        c_color = i_manaBar.color;
    }

    private void Update()
    {
        t_manaText.text = (int)f_mana + "";

        if (f_mana > f_maxMana)
        {
            f_mana = f_maxMana;
        }
        if (f_mana < 0.0f)
        {
            f_mana = 0.0f;
        }


        f_lerpSpeed = 3.0f * Time.deltaTime;

        ManaBarFiller();
        if (f_mana <= (f_maxMana * 0.25f))
        {
            FlashManaBar();
        }
        if (i_manaBar.color != c_color && f_mana > (f_maxMana * 0.25f))
        {
            i_manaBar.color = Color.blue;
        }

        f_mana += 1.0f * Time.deltaTime;
    }

    void ManaBarFiller()
    {
        i_manaBar.fillAmount = Mathf.Lerp(i_manaBar.fillAmount, f_mana / f_maxMana, f_lerpSpeed);
    }

    void ColorChanger()
    {
        Color manaColor = Color.Lerp(Color.gray, c_color, (f_mana / f_maxMana));

        i_manaBar.color = manaColor;
    }

    void FlashManaBar()
    {
        f_counter += Time.deltaTime;
        Color c_darkBlue = new Color(0.0f, 0.0f, 0.55f);

        if (f_counter > 0.5f)
        {
            i_manaBar.color = Color.white;
        }
        else
        {
            i_manaBar.color = c_darkBlue;
        }
            

        if (f_counter > 1.0f)
        {
            f_counter = 0;
        }
    }

    //Added by Petra
    #region Accessors
    //Name: Get[Variable]
    //Ins:
    //Outs: [Variable]
    public float GetMaxMana()
    {
        return f_maxMana;
    }
    public float GetCurrentMana()
    {
        return f_mana;
    }
    public float GetLerpSpeed()
    {
        return f_lerpSpeed;
    }
    #endregion

    #region Mutators
    //Name: Set[Variable]
    //Ins: newVariable
    //Outs:
    //Function: Sets the value of [Variable] to be equal to newVariable
    public void SetMaxMana(float f_newMaxMana)
    {
        f_maxMana = f_newMaxMana;
    }
    public void SetCurrentMana(float f_newCurrentMana)
    {
        f_mana = f_newCurrentMana;
    }
    public void SetLerpSpeed(float f_newLerpSpeed)
    {
        f_lerpSpeed = f_newLerpSpeed;
    }

    //Name: Sum[Variable]
    //Ins: deltaVariable
    //Outs:
    //Function: Variable += deltaVariable.
    public void SumMaxMana(float f_deltaMaxMana)
    {
        f_maxMana += f_deltaMaxMana;
    }
    public void SumCurrentMana(float f_deltaCurrentMana)
    {
        f_mana += f_deltaCurrentMana;
    }
    public void SumLerpSpeed(float f_deltaLerpSpeed)
    {
        f_lerpSpeed += f_deltaLerpSpeed;
    }
    #endregion
}
