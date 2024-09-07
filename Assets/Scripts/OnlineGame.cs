using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlineGame : NetworkBehaviour
{
    bool canMove = true;

    public Text text;
    public NetworkManager manager;
    public Material[] mats;
    public Image[] images;
    public GameObject[] bottons;

    void Start()
    {
        if (PlayerPrefs.GetInt("Language", 0) == 1)
        {
            Text[] texts = GameObject.Find("Canvas").GetComponentsInChildren<Text>(true);
            Dictionary<string, string> dte = new Dictionary<string, string>(){
            {"启动服务器", "Start Server" },
            {"启动客户端", "Start Client" },
            {"基本规则", "Basic rules" },
            {"双方执叉圈相继下子，当横竖斜向有一条直线的三颗棋子均为己方棋子是己方获胜，敌方失败，反之亦然。",
             "You two hold the cross and circle and place your pieces one after another. When three pieces in a straight line horizontally, vertically and diagonally are all one's own pieces, that one wins and the enemy loses, and vice versa." }};
            for (int i = 0; i < texts.Length; i++)
                if (dte.ContainsKey(texts[i].text)) texts[i].text = dte[texts[i].text];
        }
    }

    [ClientRpc]
    void ServerMove(int pos) { StartCoroutine(Move(pos, 0)); }

    [Command(requiresAuthority = false)]
    void ClientMove(int pos) { StartCoroutine(Move(pos, 1)); }

    IEnumerator Move(int pos, int num)
    {
        while (!canMove) yield return new WaitForSeconds(1);
        canMove = false;
        images[pos].material = Instantiate(mats[num]); images[pos].color = new Color(1, 1, 1, 1);
        for (float i = 0; i < 1; i += 1 / 30f) { images[pos].material.SetFloat("_Fade", i); yield return new WaitForSeconds(1f / 60); }
        images[pos].material.SetFloat("_Fade", 1); canMove = true;
    }

    [ClientRpc]
    void ServerRefresh() { for (int i = 0; i < 9; i++) images[i].color = new Color(1, 1, 1, 0); }

    [Command(requiresAuthority = false)]
    void ClientRefresh() { for (int i = 0; i < 9; i++) images[i].color = new Color(1, 1, 1, 0); }

    [ClientRpc]
    void ServerReturn() { SceneManager.LoadScene(0); }

    [Command(requiresAuthority = false)]
    void ClientReturn() { SceneManager.LoadScene(0); }

    public void OnButtonClicked(int num)
    {
        switch (num)
        {
            case 9:
                if (isServer) ServerRefresh();
                else { ClientRefresh(); SceneManager.LoadScene(2); }
                break;
            case 10:
                if (isServer) ServerReturn();
                else { ClientReturn(); SceneManager.LoadScene(0); }
                break;
            case 11:
                manager.StartHost();
                bottons[0].SetActive(false); bottons[1].SetActive(false); bottons[2].SetActive(false);
                break;
            case 12:
                if(text.text.Length > 10) manager.networkAddress = text.text; manager.StartClient();
                bottons[0].SetActive(false); bottons[1].SetActive(false); bottons[2].SetActive(false);
                break;
            default:
                if (canMove)
                    if (isServer) ServerMove(num);
                    else { ClientMove(num); StartCoroutine(Move(num, 1)); }
                break;
        }
    }
}
