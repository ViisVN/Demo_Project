using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class RoomButton : MonoBehaviour
{
    public TMP_Text roomname,playerNums;
    public int player_number;
    public Button button;
    private void Start()
    {
        button.onClick.AddListener(setRoomname);
    }
    
    private void setRoomname()
    {
        PunManager.Instance.CurrentRoomName = roomname.text;
    }
}
