using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    private GameState _currentGameState;
    private Object _gameSettings;
    
    
    // Install singleton pattern
    private static GameManager _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
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
   
}

