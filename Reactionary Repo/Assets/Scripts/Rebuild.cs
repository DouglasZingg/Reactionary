using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Rebuild : MonoBehaviour
{
    [SerializeField] GameObject g_barTwo;
    [SerializeField] GameObject g_barThree;
    [SerializeField] GameObject t_repairUI;
    [SerializeField] GameObject t_repairBar;
    [SerializeField] public Image i_progressBar;
    [SerializeField] Text t_repairText;
    [SerializeField] float f_fillAmount = 0.0f;
    bool b_enemyAttack = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (i_progressBar.fillAmount >= 0.45f)
            g_barTwo.SetActive(true);
        else
            g_barTwo.SetActive(false);

        if (i_progressBar.fillAmount >= 0.90f)
            g_barThree.SetActive(true);
        else
            g_barThree.SetActive(false);

        if (Input.GetKey(KeyCode.R) && t_repairUI.activeSelf)
        {
            t_repairBar.SetActive(true);
            t_repairText.enabled = false;
            f_fillAmount += 0.0025f;
            i_progressBar.fillAmount = f_fillAmount;

            if (i_progressBar.fillAmount >= 0.45f)
                g_barTwo.SetActive(true);
            else
                g_barTwo.SetActive(false);

            if (i_progressBar.fillAmount >= 0.90f)
                g_barThree.SetActive(true);
            else
                g_barThree.SetActive(false);

            if (i_progressBar.fillAmount == 1.0f)
                t_repairUI.SetActive(false);
        }

        if (b_enemyAttack)
            i_progressBar.fillAmount -= 0.001f;
        if (i_progressBar.fillAmount == 0.0f)
            b_enemyAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && i_progressBar.fillAmount != 1.0f)
        {
            t_repairUI.SetActive(true);
            t_repairText.enabled = true;
        }
        if (other.CompareTag("ENEMY"))
        {
            b_enemyAttack = true;
            f_fillAmount = 0.0f;
        }    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            t_repairUI.SetActive(false);
            t_repairBar.SetActive(false);

            if (i_progressBar.fillAmount != 1.0f)
            {
                g_barTwo.SetActive(false);
                g_barThree.SetActive(false);
                i_progressBar.fillAmount = 0.0f;
                f_fillAmount = 0.0f;
            }
            if (i_progressBar.fillAmount == 1.0f)
            {
                g_barTwo.SetActive(true);
                g_barThree.SetActive(true);
            }

        }
        if (other.CompareTag("ENEMY"))
            b_enemyAttack = false;
    }
}
