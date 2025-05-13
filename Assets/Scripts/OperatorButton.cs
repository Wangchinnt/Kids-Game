using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OperatorButton : MonoBehaviour
{   
    private string _value;
    private CanvasGroup _canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        // get the value of the button 
        _value = GetComponentInChildren<TextMeshProUGUI>().text;
        // get the canvas group component
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySound(GetComponentInChildren<TextMeshProUGUI>().text);    
    }
    
    public string GetValue()
    {
        return _value;
    }
    public void SetValue(string value)
    {
        _value = value;
        GetComponentInChildren<TextMeshProUGUI>().text = value;
    }
}
