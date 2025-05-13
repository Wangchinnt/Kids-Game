using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CloseMiniGameButton : MonoBehaviour
{
    private Button _button; 
    private MiniGame _currentMiniGame;
    private GameObject _leafParticles;
    private void Start()
    {
        _button = GetComponent<Button>();
        _currentMiniGame = FindObjectOfType<MiniGame>();
        _leafParticles = GameObject.Find("LeafParticles");
        _button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {   
        _leafParticles.SetActive(false);
        _currentMiniGame.CloseGame();
        SceneManager.Instance.LoadSceneWithLoadingScene("MainMenuScene");
    }
}
