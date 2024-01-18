using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Save_Settings : MonoBehaviour
{
    public static Save_Settings Instance;
    public class settings
    {
        public string name;
        public float volume;
         
        public settings(string name,float volume)
        {
            this.name = name;
            this.volume = volume;
        }
    }    
    
    [SerializeField] TMP_InputField nickname;
    [SerializeField] Slider volume,cur_light;

    [SerializeField] Button save_set_btn;
    [SerializeField] string filePath;
    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
       save_set_btn.onClick.AddListener(()=> Save_setting(nickname.text,volume.value));
       filePath = Application.persistentDataPath+"savesettings.json";
    }

    private void Save_setting(string name, float volume)
    {
        settings set = new settings(name,volume);
        string json = JsonUtility.ToJson(set);
        File.WriteAllText(filePath,json);
        PunManager.Instance.nicknameInput.text = name;
        UIManager.Instance.HidePopup(UIPoupName.Option);
    }
    public void Load_settings()
    {
        if(File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            settings set = JsonUtility.FromJson<settings>(jsonData);
            PunManager.Instance.nicknameInput.text = set.name;
            nickname.text = set.name;
            volume.value = set.volume;
        }
    }
}
