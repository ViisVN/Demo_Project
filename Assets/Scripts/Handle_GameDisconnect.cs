using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
public class Handle_GameDisconnect : MonoBehaviourPunCallbacks
{
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(1, out var BLUE);

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
              Score_Manager.Instance.gameOver=true;
              UIManager.Instance.ShowPopup(UIPoupName.FinalResult);
            }
    }
}
