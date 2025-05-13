using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance;
    
     [SerializeField]private GameObject mainMenu;
     [SerializeField]private GameObject learningMenu;
     [SerializeField]private GameObject practiceMenu;
     [SerializeField]private GameObject plusGameMenu;
     [SerializeField]private GameObject minusGameMenu;
     [SerializeField]private GameObject funGameMenu;
     [SerializeField]private GameObject backButton;
     [SerializeField]private GameObject gameName;
     [SerializeField]private GameObject logoGame;
    [SerializeField]private TextMeshProUGUI _DiamondText;
    [SerializeField]private TextMeshProUGUI _StreakText;
    

    private bool _isMainMenu = true;
    private bool _isLearningMenu = false;
    private bool _isPracticeMenu = false;
    private bool _isPlusGameMenu = false;
    private bool _isMinusGameMenu = false;
    private bool _isFunGameMenu = false;
    
    
    /*Main menu -> Learning or practise -> 
    Learning -> Math or Vnmese
    Practise -> +, -, Other
    Math -> Math map -> Map scene -> Lesson scene
    VNmese-> Vietnamese -> Map scene -> Lesson scene*/

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

    private void Start()
    {
        _DiamondText = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().FirstOrDefault(obj => obj.name == "DiamondText");
        _StreakText = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().FirstOrDefault(obj => obj.name == "StreakText");
        if (mainMenu == null)
        {
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MainMenu");
        }
        else
        {
            Debug.Log(mainMenu.name);
        }
        if (learningMenu == null)
        {
            learningMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LearningMenu");
        }
        if (practiceMenu == null)
        {
            practiceMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "PracticeMenu");
        }
        if (plusGameMenu == null)
        {
            plusGameMenu =  Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "PlusGameMenu");
        }
        if (minusGameMenu == null)
        {
            minusGameMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MinusGameMenu");
        }
        if (funGameMenu == null)
        {
            funGameMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "FunGameMenu");
        }
        if (backButton == null)
        {
            backButton =  Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BackButton");
        }
        
        if (gameName == null)
        {
            gameName = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MenuGameName");
        }
        if (logoGame == null)
        {
            logoGame = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LogoGame");
        }

        // set text for diamond and streak
        UpdateDiamondText();
        UpdateStreakText();
    }

    public void ReassignUIObjects()
    {
        // Find all GameObjects if they are not assigned
        Debug.Log("Reassigning UI Objects");
        if (mainMenu == null)
        {
            mainMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MainMenu");
        }
        if (learningMenu == null)
        {
            learningMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LearningMenu");
        }
        if (practiceMenu == null)
        {
            practiceMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "PracticeMenu");
        }
        if (plusGameMenu == null)
        {
            plusGameMenu =  Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "PlusGameMenu");
        }
        if (minusGameMenu == null)
        {
            minusGameMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MinusGameMenu");
        }
        if (funGameMenu == null)
        {
            funGameMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "FunGameMenu");
        }
        if (backButton == null)
        {
            backButton =  Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BackButton");
        }
        if (gameName == null)
        {
            gameName = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "MenuGameName");
        }
        if (logoGame == null)
        {
            logoGame = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LogoGame");
        }
        Debug.Log("MenuGameName found: " + (gameName != null ? gameName.name : "NULL"));
    }
    private void HideAllMenus()
    {
        _isMainMenu = false;
        _isLearningMenu = false;
        _isPracticeMenu = false;
        _isPlusGameMenu = false;
        _isMinusGameMenu = false;
        _isFunGameMenu = false;
        mainMenu.SetActive(false);
        gameName.SetActive(false);
        logoGame.SetActive(false);
        
        learningMenu.SetActive(false);
        practiceMenu.SetActive(false);
        plusGameMenu.SetActive(false);
        minusGameMenu.SetActive(false);
        funGameMenu.SetActive(false);
        backButton.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAllMenus();
        _isMainMenu = true;
        gameName.SetActive(true);
        logoGame.SetActive(true);
        mainMenu.SetActive(true);
    }

    public void ShowLearningMenu()
    {
        HideAllMenus();
        _isLearningMenu = true;
        learningMenu.SetActive(true);
        backButton.SetActive(true);
    }

    public void ShowPracticeMenu()
    {
        HideAllMenus();
        _isPracticeMenu = true;
        practiceMenu.SetActive(true);
        logoGame.SetActive(true);
        gameName.SetActive(true);
        backButton.SetActive(true);
    }

    public void ShowPlusGameMenu()
    {
        HideAllMenus();
        _isPlusGameMenu = true;
        backButton.SetActive(true);
        plusGameMenu.SetActive(true);
    }

    public void ShowMinusGameMenu()
    {
        HideAllMenus();
        _isMinusGameMenu = true;
        backButton.SetActive(true);
        minusGameMenu.SetActive(true);
    }
    
    public void ShowFunGameMenu()
    {
        HideAllMenus();
        _isFunGameMenu = true;
        backButton.SetActive(true);
        funGameMenu.SetActive(true);
    }
    public void BackButton()
    {
        if (_isLearningMenu || _isPracticeMenu)
        {
            ShowMainMenu();
        }
        else if (_isPlusGameMenu || _isMinusGameMenu || _isFunGameMenu)
        {
            ShowPracticeMenu();
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void UpdateStreakText()
    {
        _StreakText.text = LocalDataManager.Instance.LoadStudent(AuthManager.Instance.GetCurrentUserId()).Streak.ToString() + " ngày";
    }
    public void UpdateDiamondText()
    {
        _DiamondText.text = LocalDataManager.Instance.LoadStudent(AuthManager.Instance.GetCurrentUserId()).Diamonds.ToString();
    }
}
