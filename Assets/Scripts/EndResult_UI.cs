using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Mono.CSharp;
using System;
using Photon.Realtime;
using UnityEngine.UI;

public class EndResult_UI : MonoBehaviourPunCallbacks
{
    public TMP_Text textObject;
    public float startFontSize;
    public float endFontSize;
    public float duration;
    public float letterSpacing;

    public GameObject resultboard, playerResult_button, playerResult_content;
    public TMP_Text winTeamUI,resultNumber;
    public void Start()
    {
        textObject.gameObject.SetActive(true);
        textObject.fontSize = startFontSize;
        textObject.characterSpacing = letterSpacing;


        DOTween.Sequence()
            .Append(textObject.DOFade(endFontSize, duration)
                .SetEase(Ease.OutExpo))
            .Append(DOTween.To(() => textObject.characterSpacing, x => textObject.characterSpacing = x, 0f, 0.5f)
                .SetEase(Ease.OutQuad))
            .OnStart(() => textObject.alpha = 0f)
            .OnComplete(() =>
            textObject.alpha = 1f).
            OnComplete(() => AppearResultBoard());
    }
    public void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            BacktoLobby();
        }
    }

    public void AppearResultBoard()
    {
        resultboard.SetActive(true);
        resultboard.GetComponent<CanvasGroup>().alpha = 0f;

        // Fade in over specified duration
        resultboard.GetComponent<CanvasGroup>().DOFade(1f, 3f)
        .OnComplete(() => CreatePlayerBoard());
    }
    public void CreatePlayerBoard()
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(1,out var BLUE);
        List<PlayerInfo> finalList = PlayerList_Manager.instance.playerslist;
        IComparer<int> highestValueComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
        finalList = SortByKill(finalList);
        int redKill=0, blueKill=0;
        for(int i = 0; i<finalList.Count;i++)
        {
            var player = Instantiate(playerResult_button, playerResult_content.transform).GetComponent<Player_Result>();
            player.No.text = (i+1).ToString();
            player.nickname.text = finalList[i].name;
            if(finalList[i].team==BLUE)
            {
                player.team.text = "<color=blue>BLUE";
                blueKill += finalList[i].kill;
            }
            else
            {
                 player.team.text = "<color=red>RED";
                 redKill += finalList[i].kill;
            }
            player.kill.text = finalList[i].kill.ToString();
            player.death.text = finalList[i].death.ToString();
            winTeamUI.gameObject.SetActive(true);
            resultNumber.gameObject.SetActive(true);
            if(blueKill==redKill)
            {
                winTeamUI.text = "<color=white> DRAW";
                resultNumber.text = "<color=red>"+redKill.ToString()+"<color=white> - <color=blue>"+blueKill.ToString();
            }
            else if(blueKill<redKill)
            {
               winTeamUI.text = "<color=red> RED WIN";
               resultNumber.text = "<color=red>"+redKill.ToString()+"<color=white> - <color=blue>"+blueKill.ToString(); 
            }
            else
            {
                winTeamUI.text = "<color=blue> BLUE WIN";
               resultNumber.text = "<color=red>"+redKill.ToString()+"<color=white> - <color=blue>"+blueKill.ToString();  
            }
        }
    }
    public static List<PlayerInfo> SortByKill(List<PlayerInfo> players)
    {
        // Use List<T>.Sort with a custom comparer function
        players.Sort((player1, player2) => player2.kill.CompareTo(player1.kill));

        return players;
    }
    
    public void BacktoLobby()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        PhotonNetwork.LoadLevel("Connect_Lobby_Room");
    }

}
