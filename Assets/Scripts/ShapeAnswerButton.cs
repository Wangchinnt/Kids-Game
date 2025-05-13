using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ShapeAnswerButton : MonoBehaviour
{
    private string _value;
    private CanvasGroup _canvasGroup;
    private MiniGame _currentGame;
    private Button _button;
    // Start is called before the first frame update
    void Start()
    {
        _currentGame = FindObjectOfType<MiniGame>();
        // don't take the number in name
        _value = RemoveNumbersFromName(this.gameObject.name);
        _button = GetComponent<Button>();
        // get the canvas group component
        _canvasGroup = GetComponent<CanvasGroup>();
        // Add a listener to the button
        _button.onClick.AddListener(OnButtonClick);
    }

    private string RemoveNumbersFromName(string objectName)
    {   
        // remove all numbers and (clone) from the object name
        return Regex.Replace(name, @"\d|\(Clone\)", "").Trim();
    }
    public void OnButtonClick()
    {
        //AudioManager.Instance.PlaySound(_value);    
        Debug.Log($"{name} is clicked and its value is {_value}");
        _currentGame.SetSelectedAnswer(_value, this.gameObject);
    }
    
    public string GetValue()
    {
        return _value;
    }
    public void SetValue(string value)
    {
        _value = value;
    }
}
