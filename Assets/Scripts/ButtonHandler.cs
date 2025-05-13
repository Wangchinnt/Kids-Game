using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    
    private Button button; // Không cần gán trong Inspector

    void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Không tìm thấy Button trên GameObject này!");
        }
        if (uIManager == null)
        {
            uIManager = FindObjectOfType<UIManager>();
        }
    }
    
    private void OnEnable()
    {   
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            uIManager = FindObjectOfType<UIManager>();
            
            Debug.Log("Main Menu Scene Loaded");
        }
    }
    void OnButtonClick()
    {
        if (name == "LearnButton")
        {
            uIManager.ShowLearningMenu();
        }
        else if (name == "PracticeButton")
        {
            uIManager.ShowPracticeMenu();
        }
        else if (name == "ExitButton")
        {
            uIManager.ExitGame();
        }
        else if (name == "PlusGameButton")
        {
            uIManager.ShowPlusGameMenu();
        }
        else if (name == "MinusGameButton")
        {
            uIManager.ShowMinusGameMenu();
        }
        else if (name == "FunGameButton")
        {
            uIManager.ShowFunGameMenu();
        }
        else if (name == "BackButton")
        {
            uIManager.BackButton();
        }
    }
}
