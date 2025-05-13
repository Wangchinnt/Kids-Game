using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameButton : MonoBehaviour
{
    private Button _button;
    private string _sceneName;
    private void Start()
    {
        _button = GetComponent<Button>();
        _sceneName = this.name + "Scene";
        _button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        SceneManager.Instance.LoadSceneWithLoadingScene(_sceneName);
    }
}
