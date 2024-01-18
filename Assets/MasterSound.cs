using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterSound : MonoBehaviour
{
    [SerializeField] Slider _slider;
    
    void Start()
    {
        Sound_Settings.Instance.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(volume => Sound_Settings.Instance.ChangeMasterVolume(volume));
    }
}
