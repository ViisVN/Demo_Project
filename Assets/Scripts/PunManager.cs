using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QFSW.QC;
using Mono.CSharp;
using Photon.Pun.UtilityScripts;

public class PunManager : MonoBehaviourPunCallbacks
{
    private const byte
    ON_START_GAME = 3,
    ON_ADD_PLAYERLIST = 4,
    ON_FINAL_RESULT = 5,
    RED_TEAM = 2,
    BLUE_TEAM = 1;


    public static PunManager Instance;
    [SerializeField] public TMP_InputField nicknameInput;

    //GetRoom name

    public string CurrentRoomName;

    //Content Object
    [SerializeField] GameObject content, redTeam_content, blueTeam_content;
    [SerializeField] GameObject roomButt, playerButton;

    [SerializeField] TMP_Text player_team, cur_playerNickname;

    [SerializeField] List<RoomInfo> cur_roomlist = new List<RoomInfo>();
    [SerializeField] Button StartGame_Button, LeaveGame_Button;
    bool isplaying = false;

    //Raise event for started game
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    //Final result
    List<PlayerInfo> finaleResult = new List<PlayerInfo>();
    [SerializeField] Sprite redteam_sprite;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    public void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == ON_START_GAME)
        {
            object[] datas = (object[])obj.CustomData;
            isplaying = (bool)datas[0];
            PhotonNetwork.LoadLevel("Movement Testing");
        }
        if (obj.Code == ON_ADD_PLAYERLIST)
        {
            StartCoroutine(WaitToCreatePlayer_teamlist());
        }
    }
    void Start()
    {
        customProperties.Add("Isgamestarted", true);
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conecting...");
        StartGame_Button.onClick.AddListener(StartGame);
        LeaveGame_Button.onClick.AddListener(LeaveRoom);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Connect sucessfully.");
        Save_Settings.Instance.Load_settings();
        UIManager.Instance.HidePopup(UIPoupName.LoadingScene);
        UIManager.Instance.ShowPopup(UIPoupName.LobbyScene);
    }
    public void Create()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 4
        };
        roomOptions.BroadcastPropsChangeToAll = true;
        var nickname = nicknameInput.text;
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions);
    }

    public void Join()
    {
        var nickname = nicknameInput.text;
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.JoinRoom(CurrentRoomName);
    }
    public void Join_Random()
    {
        var nickname = nicknameInput.text;
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join Room Sucess");
        cur_playerNickname.text = "Nickname: " + PhotonNetwork.LocalPlayer.NickName;
        CurrentRoomName = null;
        UIManager.Instance.HidePopup(UIPoupName.LobbyScene);
        UIManager.Instance.ShowPopup(UIPoupName.RoomScene);
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UIManager.Instance.ShowPopup(UIPoupName.StartGame_Button);
        }
        PhotonNetwork.LocalPlayer.JoinTeam(BLUE_TEAM);
        StartCoroutine(WaitToCreatePlayer_teamlist());

        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartGame_Button.image.color = Color.green;
        }
    }
    public void SwitchTeam()
    {

        PhotonTeamsManager.Instance.TryGetTeamByCode(BLUE_TEAM, out var team);
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == team)
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(RED_TEAM);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(BLUE_TEAM);
        }
        StartCoroutine(WaitToCreatePlayer_teamlist());
        Debug.Log("Team Switched");
        object[] datas = new object[] { };
        PhotonNetwork.RaiseEvent(ON_ADD_PLAYERLIST, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }


    IEnumerator WaitToCreatePlayer_teamlist()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        CreatePlayerList();
    }
    public void CreatePlayerList()
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(BLUE_TEAM, out var BLUE);
        //Destroy before greate
        foreach (Transform obj in redTeam_content.transform)
        {
            Destroy(obj.gameObject);
        }
        foreach (Transform obj in blueTeam_content.transform)
        {
            Destroy(obj.gameObject);
        }
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == BLUE)
        {
            player_team.text = "Your team: <color=blue> BLUE";
        }
        else
        {
            player_team.text = "Your team: <color=red> RED";
        }
        //Tao ra player, cap nhap text
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPhotonTeam() == BLUE)
            {
                var butt = Instantiate(playerButton, blueTeam_content.transform);
                var player_butt = butt.GetComponent<Player_Button>();
                player_butt.cur_Player_name.text = player.NickName;
            }
            else
            {
                var butt = Instantiate(playerButton, redTeam_content.transform);
                butt.GetComponent<Image>().sprite = redteam_sprite;
                var player_butt = butt.GetComponent<Player_Button>();
                player_butt.cur_Player_name.text =  player.NickName;
            }
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform obj in content.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach (var room in roomList)
        {
            if (cur_roomlist.Contains(room))
            {
                if (room.PlayerCount == 0)
                {
                    cur_roomlist.Remove(room);
                }
                else
                {
                    cur_roomlist.Remove(cur_roomlist.Find(x => x.Name.Contains(room.Name)));
                    cur_roomlist.Add(room);
                }
            }
            else if (room.IsVisible == false)
            {

            }
            else
            {
                cur_roomlist.Add(room);
            }
        }

        //Break neu cur_roomlist empty
        if (cur_roomlist == null) return;
        // Update the room list
        foreach (RoomInfo roominfo in cur_roomlist)
        {
            var obj = Instantiate(roomButt, content.transform);
            var obj_info = obj.GetComponent<RoomButton>();
            obj_info.roomname.text = roominfo.Name;
            obj_info.playerNums.text = roominfo.PlayerCount.ToString() + "/" + roominfo.MaxPlayers.ToString();
        }
    }

    private IEnumerator UpdateRoomListWithDelay(List<RoomInfo> roomList)
    {
        yield return new WaitForSeconds(1.0f); // 

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " had joined room.");
        //Create a new list;
        StartCoroutine(WaitToCreatePlayer_teamlist());
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartGame_Button.image.color = Color.green;
            Debug.Log("Can Start the game");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " had left room.");
        if (isplaying)
        {
            PhotonTeamsManager.Instance.TryGetTeamByCode(BLUE_TEAM, out var BLUE);
            int red = 0; int blue = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetPhotonTeam() == BLUE)
                {
                    blue += 1;
                }
                else
                {
                    red += 1;
                }
            }
            if (red == 0 || blue == 0)
            {
                finaleResult = PlayerList_Manager.instance.playerslist;
                isplaying = false;
                PhotonNetwork.LeaveRoom();
            }
            return;
        }
        //Create a new list;
        StartCoroutine(WaitToCreatePlayer_teamlist());

        if (PhotonNetwork.CountOfPlayersInRooms <= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartGame_Button.image.color = Color.white;
            Debug.Log("Cannot Start the game");
        }
    }
    public void StartGame()
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(BLUE_TEAM, out var BLUE);
        if (PhotonNetwork.IsMasterClient)
        {
            int red = 0, blue = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.GetPhotonTeam() == BLUE)
                {
                    blue += 1;
                }
                else
                {
                    red += 1;
                }
            }
            if (blue != red) { Debug.Log("Not enough player"); }
            else
            {
                Debug.Log("Start game");


                isplaying = true;
                object[] datas = new object[] { isplaying };
                PhotonNetwork.RaiseEvent(ON_START_GAME, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel("Movement Testing");
            }
        }
    }

    public void LeaveRoom()
    {
        StartGame_Button.image.color = Color.white;
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        PhotonNetwork.LeaveRoom();
        UIManager.Instance.HidePopup(UIPoupName.RoomScene);
        UIManager.Instance.ShowPopup(UIPoupName.LobbyScene);
    }

}