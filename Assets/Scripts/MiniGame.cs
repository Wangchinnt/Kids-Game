using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
   
    public string gameName;        
    public bool isCompleted;        
    
    public abstract void InitializeGame(); 
    public abstract void StartGame();     
    public abstract bool CheckWin();       
    
    public virtual void EndGame()
    {
        Debug.Log($"MiniGame {gameName} completed: {isCompleted}");
    }
}