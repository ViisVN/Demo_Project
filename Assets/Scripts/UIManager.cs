using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UIPoupName
{
    None = 0,
    LoadingScene = 1,
    LobbyScene =2,
    RoomScene =3,
    StartGame_Button=6,
    Option =7,
    LeaderBoard =4,
    FinalResult =5
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] protected List<UIPopUp> _listPopup = new List<UIPopUp>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public B GetPopUpwithType<B>(UIPoupName name)
    {
        return _listPopup.FirstOrDefault(item =>item.name==name).GetComponent<B>();
    }

    public void ShowPopup(UIPoupName name)
    {

        UIPopUp popupToShow = _listPopup.FirstOrDefault(item =>item.name==name);
        if (popupToShow != null)
        {
            popupToShow.gameObject.SetActive(true);
            popupToShow.Show();
        }
        else
        {
            Debug.LogError("No Popup with name : " + name.ToString());
        }
    }
    public void HidePopup(UIPoupName name)
    {

        UIPopUp popupToHide = _listPopup.FirstOrDefault(item => item.name == name);
        if (popupToHide != null)
        {
            popupToHide.gameObject.SetActive(false);
            popupToHide.Hide();
        }
        else
        {
            Debug.LogError("No Popup with name : " + name.ToString());
        }
    }
    public void HideAllPopup()
    {
        foreach (var item in _listPopup)
        {
            item.gameObject.SetActive(false);
        }
    }
    public void AddUIPopup(List<UIPopUp> _addList)
    {
        foreach(var Popup in _addList)
        {
            _listPopup.Add(Popup);
        }
    }

}
