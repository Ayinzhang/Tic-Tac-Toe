using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    Dictionary<string, string> dte = new Dictionary<string, string>()
    {
        {"井字棋", "Tic Tac Toe" },
        {"单机模式", "Vs Computer" },
        {"联机模式", "Vs Player" },
        {"退出游戏", "Exit Game" },
        {"EN", "ZH" }},
        dtz = new Dictionary<string, string>()
    {
        {"Tic Tac Toe", "井字棋"},
        {"Vs Computer", "单机模式" },
        {"Vs Player", "联机模式" },
        {"Exit Game", "退出游戏" },
        {"ZH", "EN" }};

    void SwitchLanguage()
    {
        Text[] texts = transform.parent.GetComponentsInChildren<Text>(true);
        if(PlayerPrefs.GetInt("Language", 0) == 0)
        {
            for (int i = 0; i < texts.Length; i++) 
                if (dtz.ContainsKey(texts[i].text)) texts[i].text = dtz[texts[i].text];
        }
        else
        {
            for (int i = 0; i < texts.Length; i++)
                if (dte.ContainsKey(texts[i].text)) texts[i].text = dte[texts[i].text];
        }
    }

    public void OnButtonClicked(int num)
    {
        switch (num)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                SceneManager.LoadScene(2);
                break;
            case 2:
                Application.Quit();
                break;
            case 3:
                PlayerPrefs.SetInt("Language", 1 - PlayerPrefs.GetInt("Language", 0));
                SwitchLanguage();
                break;
        }
    }
}
