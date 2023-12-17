using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] 
public  class PlayerInfo
    {
        public string name;
        public int actorID;
        public int kill, death;
        public PhotonTeam team;
        public PlayerInfo(string name,int actorID,int kill,int death,PhotonTeam team)
        {
            this.name = name;
            this.actorID = actorID;
            this.kill = kill;
            this.death = death;
            this.team = team;
        }
    }
public class PlayerList_Manager : MonoBehaviour
{
    public static PlayerList_Manager instance;
    [SerializeField] GameObject playerinfo_button;

    [SerializeField] Transform redcontent,bluecontent;
    
    
     [SerializeField]
    public List<PlayerInfo> playerslist = new List<PlayerInfo>();

    public Sprite red_sprite;

    public const byte UPDATE_WHEN_KILL_OR_DEATH = 10;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += Client_ReceivedEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= Client_ReceivedEvent;
    }
     
    private void Client_ReceivedEvent(EventData obj)
    {
        if (obj.Code == UPDATE_WHEN_KILL_OR_DEATH)
        {
           object[] datas = (object[])obj.CustomData;
           playerslist = (List<PlayerInfo>)datas[0];
        }
    } 
     public void Awake()
    {
        instance = this;
        foreach(var player in PhotonNetwork.PlayerList)
        {
            var data = new PlayerInfo(player.NickName,player.ActorNumber,0,0,player.GetPhotonTeam());
            playerslist.Add(data);
        }
    }

    public void Update()
    {
       if(Input.GetKey(KeyCode.Tab))
       {
        ShowBoard();
       }
       if(Input.GetKeyUp(KeyCode.Tab))
       {
        UIManager.Instance.HidePopup(UIPoupName.LeaderBoard);
       }
    }

    private void ShowBoard()
    {        //DELETE BOARD
        foreach(Transform obj in redcontent.transform)
        {
            Destroy(obj.gameObject);
        }
         foreach(Transform obj in bluecontent.transform)
        {
            Destroy(obj.gameObject);
        }
        //GetTEAMBLUE CODE
        PhotonTeamsManager.Instance.TryGetTeamByCode(1,out var BLUE);

        //CREATE
        foreach(var player in playerslist)
        {
           if(player.team==BLUE)
           {
              var obj = Instantiate(playerinfo_button,bluecontent);
              var info = obj.GetComponent<PlayerInfo_Button>();
              info.nickname.text = "Nickname: "+player.name;
              info.kill.text = "KILL: "+ player.kill;
              info.death.text = "DEATH: "+ player.death;
           }
           else
           {
            var obj = Instantiate(playerinfo_button,redcontent);
              var info = obj.GetComponent<PlayerInfo_Button>();
              obj.GetComponent<Image>().sprite = red_sprite;
              info.nickname.text = "Nickname: "+player.name;
              info.kill.text = "KILL: "+ player.kill;
              info.death.text = "DEATH: "+ player.death;
           }
        }

        //SHOW BOARD
        UIManager.Instance.ShowPopup(UIPoupName.LeaderBoard);
    }
}
