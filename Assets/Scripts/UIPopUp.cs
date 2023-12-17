using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class UIPopUp : MonoBehaviour
{
    public new UIPoupName name;
    public static UIPopUp Instance;
    private void Awake() {
        if(Instance == null)
        Instance = this;
    }
    public void Show()
    {
        OnShown();
    }
    public void Hide()
    {
        OnHiding();
    }
    protected virtual void OnShown()
    {
        this.gameObject.SetActive(true);
    }
    protected virtual void OnHiding()
    {
        this.gameObject.SetActive(false);
    }
}