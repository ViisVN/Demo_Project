using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] Button btn_option;
    void Start()
    {
        btn_option.onClick.AddListener(()=>UIManager.Instance.ShowPopup(UIPoupName.Option));
    }
}
