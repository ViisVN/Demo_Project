using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class Score_Manager : MonoBehaviour
{
    public static Score_Manager Instance;
    public int RedScore, BlueScore;
    public TMP_Text scoreText;
    PhotonView view;
    public bool gameOver = false;
    public int winScore = 1;
    public void Awake()
    {
        Instance = this;
        view = GetComponent<PhotonView>();
    }
    public void Update()
    {
        if(RedScore ==winScore ||BlueScore ==winScore)
        {
            gameOver = true;
            UIManager.Instance.ShowPopup(UIPoupName.FinalResult);
        }
    }
    public void UpdateScore()
    {
       view.RPC(nameof(UpdateScore_RPC),RpcTarget.All);
    }

    [PunRPC]
    private void UpdateScore_RPC()
    {
        scoreText.text = "<color=red>"+RedScore.ToString()+" <color=white>-<color=blue> "+BlueScore.ToString();
    }
}
