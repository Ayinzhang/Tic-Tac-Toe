using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleGame : MonoBehaviour
{
    bool isPlayerFirst = true, canMove = true;
    string board; // 0 - Default, 1 - Cross, 2 - Circle
    Stack<string> boardRec = new();
    int[] winConditions = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 3, 6, 1, 4, 7, 2, 5, 8, 0, 4, 8, 2, 4, 6 };

    public Material[] mats;
    public Image[] images;

    void Start()
    {
        board = "000000000";
        if (PlayerPrefs.GetInt("Language", 0) == 1)
        {
            Text[] texts = GameObject.Find("Canvas").GetComponentsInChildren<Text>(true);
            Dictionary<string, string> dte = new Dictionary<string, string>(){
            {"基本规则", "Basic rules" },
            {"双方执叉圈相继下子，当横竖斜向有一条直线的三颗棋子均为己方棋子是己方获胜，敌方失败，反之亦然。",
             "You two hold the cross and circle and place your pieces one after another. When three pieces in a straight line horizontally, vertically and diagonally are all one's own pieces, that one wins and the enemy loses, and vice versa." }};
            for (int i = 0; i < texts.Length; i++)
                if (dte.ContainsKey(texts[i].text)) texts[i].text = dte[texts[i].text];
        }
        isPlayerFirst = PlayerPrefs.GetInt("PlayerFirst", 1) == 1;
        if (isPlayerFirst) PlayerPrefs.SetInt("PlayerFirst", 0);
        else { StartCoroutine(Move(Random.Range(0, 9), -1)); PlayerPrefs.SetInt("PlayerFirst", 1); }
    }

    void PlayerMove(int pos, int num)
    {
        boardRec.Push(board);
        StartCoroutine(Move(pos, num));
        AgentMove(-num);
    }

    void AgentMove(int num)
    {
        if (!board.Contains("0")) return;
        StartCoroutine(Move(GetPos(), num));
    }

    int GetPos()
    {
        int cnt = isPlayerFirst ? -1 : 1, cal, pos = -1;
        for (int i = 0; i < 9; i++)
            if (board[i] == '0')
            {
                StringBuilder sb = new StringBuilder(board); sb[i] = isPlayerFirst ? '2' : '1';
                cal = MinMax(sb.ToString(), !isPlayerFirst); 
                if (isPlayerFirst ^ (cnt > cal)) { cnt = cal; pos = i; }
            }
        return pos;
    }

    int MinMax(string board, bool isMax)
    {
        int winResult = CheckWin(board);
        if (winResult != 0) return winResult;
        else if (!board.Contains('0')) return 0;

        int cnt = isMax ? -1 : 1, cal;
        for (int i = 0; i < 9; i++)
            if (board[i] == '0')
            {
                StringBuilder sb = new StringBuilder(board); sb[i] = isMax ? '2' : '1';
                cal = MinMax(sb.ToString(), !isMax);
                if (isMax ^ (cnt > cal)) cnt = cal;
            }
        return cnt;
    }

    int CheckWin(string board)
    {
        for (int i = 0; i < winConditions.Length; i += 3)
        {
            int a = winConditions[i], b = winConditions[i + 1], c = winConditions[i + 2];
            if (board[a] != '0' && board[a] == board[b] && board[b] == board[c]) return board[a] == '1' ? -1 : 1;
        }
        return 0;
    }

    IEnumerator Move(int pos, int num)
    {
        while (!canMove) yield return new WaitForSeconds(1);
        canMove = false;
        StringBuilder sb = new StringBuilder(board); sb[pos] = num == -1 ? '1' : '2'; board = sb.ToString();
        images[pos].material = Instantiate(mats[num == -1 ? 0 : 1]); images[pos].color = new Color(1, 1, 1, 1);
        for (float i = 0; i < 1; i += 1 / 60f) { images[pos].material.SetFloat("_Fade", i); yield return new WaitForSeconds(1f / 60); }
        canMove = true;
    }

    public void OnButtonClicked(int num)
    {
        switch (num)
        {
            case 9:
                board = boardRec.Pop();
                for (int i = 0; i < 9; i++)
                    if (board[i] == '0' && images[i].color.a > 0)
                        images[i].color = new Color(1, 1, 1, 0);
                break;
            case 10:
                SceneManager.LoadScene(1);
                break;
            case 11:
                SceneManager.LoadScene(0);
                break;
            default:
                if(canMove && board[num] == '0')
                    PlayerMove(num, isPlayerFirst ? -1 : 1);
                break;
        }
    }
}
