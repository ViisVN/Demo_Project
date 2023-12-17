using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    public static UIController instance;
    public Slider tempSlider;

    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI overheatedText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
