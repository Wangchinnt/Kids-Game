using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    private GameState _currentGameState;
    private Object _gameSettings;
    public int TimeLooping = 3; // Biến toàn cục lưu số lần lặp 
    
    
    // Install singleton pattern
    public  static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void InitializeGame()
    {
        
    }
    
    public void StartGame()
    {
        
    }
    
    public void PauseGame()
    {
        
    }
    
    public void ResumeGame()
    {
        
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void SwitchGameState(GameState newGameState)
    {
        
    }
    
    public bool SaveGameProgress()
    {
        return false;
    }
    
    public bool LoadGameProgress()
    {
        return false;
    }
    public void ResetTimeLooping()
    {
        TimeLooping = 3; // Reset khi cần (ví dụ: khi người chơi bắt đầu lại)
    }
}

