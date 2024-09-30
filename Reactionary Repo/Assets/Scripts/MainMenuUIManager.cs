using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void MenuSwitch(string sz_menuNameOne, string sz_menuNameTwo, bool b_activeState)
    {
        GameObject g_menuOne = GameObject.Find(sz_menuNameOne);
        GameObject g_menuTwo = GameObject.Find(sz_menuNameTwo);

        for (int i = 0; i < g_menuOne.transform.childCount; i++)
            g_menuOne.transform.GetChild(i).gameObject.SetActive(b_activeState);

        for (int i = 0; i < g_menuTwo.transform.childCount; i++)
            g_menuTwo.transform.GetChild(i).gameObject.SetActive(!b_activeState);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Level Scene");
    }

    public void BlogButtion()
    {
        string URL = "https://capstonecold.wordpress.com/";
        Application.OpenURL(URL);
    }

    public void ExitButton()
    {
        //Commenting this out because it might be breaking our build -Petra
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OptionButton()
    {
        MenuSwitch("Options Menu", "Main Menu", true);
    }

    public void OptionExitButton()
    {
        MenuSwitch("Main Menu", "Options Menu", true);
    }

    public void HowToPlayExitButton()
    {
        MenuSwitch("Main Menu", "How To Play", true);
    }

    public void HowToPlayButton()
    {
        MenuSwitch("How To Play", "Main Menu", true);
    }

    public void CreditsButton()
    {
        MenuSwitch("Credits", "Main Menu", true);
    }
    public void CreditsExitButton()
    {
        MenuSwitch("Main Menu", "Credits", true);
    }
}
