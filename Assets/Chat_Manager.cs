using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Chat_Manager : MonoBehaviour, IChatClientListener
{
    public static Chat_Manager Instance;
    ChatClient chatclient;
    public bool isConected, isChatting;

    [SerializeField] TMP_InputField chat_mes;
    [SerializeField] TMP_Text chat_content;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        isConected = true;
        chatclient = new ChatClient(this);
        chatclient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues((PhotonNetwork.LocalPlayer.ActorNumber).ToString()));
    }

    // Update is called once per frame
    void Update()
    {
        if (isConected)
        {
            chatclient.Service();
        }
        if (Input.GetKey(KeyCode.C))
        {
            OpenChat();
        }
        if (chat_mes.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicchatOnclick();
        }
    }

    IEnumerator CloseChatTextBox(float time)
    {
        UIManager.Instance.ShowPopup(UIPoupName.Chat_text_content);
        yield return new WaitForSecondsRealtime(time);
        UIManager.Instance.HidePopup(UIPoupName.Chat_text_content);
    }
    void OpenChat()
    {
        isChatting = true;
        Cursor.lockState = CursorLockMode.None;
        UIManager.Instance.ShowPopup(UIPoupName.Chat);
        chat_mes.ActivateInputField();
    }

    void SubmitPublicchatOnclick()
    {
        Debug.Log("SendMess");
        chatclient.PublishMessage("channelA", chat_mes.text);
        chat_mes.text = "";
        Cursor.lockState = CursorLockMode.Locked;
    }
    #region IChatClientListener implementation

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {

    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        Debug.Log("Connected");
    }

    public void OnChatStateChange(ChatState state)
    {
        chatclient.Subscribe(new string[] { "channelA" });
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(1, out var BLUE);
        string mgs = "";
        int actorID;
        for (int i = 0; i < senders.Length; i++)
        {
            actorID = Int32.Parse(senders[i]);
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorID).NickName;
            if (PhotonNetwork.CurrentRoom.GetPlayer(actorID).GetPhotonTeam() == BLUE)
            {
                mgs = string.Format("<color=blue>{0}<color=white>: {1}", nickname, messages[i]);

            }
            else
            {
                mgs = string.Format("<color=red>{0}<color=white>: {1}", nickname, messages[i]);
            }
            chat_content.text += "\n " + mgs;

            Debug.Log("Mgs");
        }
        UIManager.Instance.HidePopup(UIPoupName.Chat);
        isChatting = false;
        StartCoroutine(CloseChatTextBox(5f));

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
