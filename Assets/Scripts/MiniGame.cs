using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
   
     public string gameName;        

     public bool isCompleted;        
     public bool isLooping = false;
     
  
    public abstract void InitializeGame(); 
    public abstract void StartGame();     
    public abstract bool CheckWin();       
    
    public abstract void EndGame();
    public abstract void CloseGame();
    public abstract void SetSelectedAnswer(string answer, GameObject answerBox);
    // public abstract void ShowHandGuide();
    // public abstract void OpenYouTubeHelp();
}