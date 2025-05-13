using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using GameData;
using TMPro;
public class CountingGame : MiniGame, IHaveCountingElement
{   
    public GameObject answerBox; 
    public int Count { get; set; }
    
    [SerializeField] private int expectedValue; 
    [SerializeField] private int[] valueRange = new int[2] { 1, 10 };
    [SerializeField] private int[] answers = new int[3]; 
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private GameObject countingArea;
    [SerializeField] private GameObject answerArea;
    [SerializeField] private GameObject answerButtonArea;
    [SerializeField] private GameObject[] gridElements;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject ground;
    private string _selectedAnswer = "-1"; 
    private float _delay = 0f;
    private bool _isNextGame = false;
    private bool _isResetElementsScheduled = false;
    private List<GameObject> _particles = new List<GameObject>();

    private bool _isLearningMode = false;
    private QuestionConfig _currentQuestion;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI _ScoreStatusText;
    [SerializeField] private TextMeshProUGUI _TimeText;
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private TextMeshProUGUI _DiamondText;
    private int _wrongAttempts = 0;
    [SerializeField] private GameObject handGuide;
    private string youtubeHelpUrl;

    public override void InitializeGame()
    {
        gameName = "Counting Game";
        isCompleted = false;
        _selectedAnswer = "-1"; 
        Debug.Log("Counting Game Initialized");
        countingArea.GameObject().SetActive(true);
        answerArea.GameObject().SetActive(true);
        answerButtonArea.GameObject().SetActive(true);
        _isLearningMode = LessonManager.Instance.IsLearningMode();
        if (_isLearningMode)
        {   
            youtubeHelpUrl = LessonManager.Instance.GetCurrentLesson().videoSupportLink;
            _currentQuestion = LessonManager.Instance.GetCurrentQuestion();
            valueRange = LessonManager.Instance.GetCurrentLesson().valueRange;
            if (_currentQuestion.isFixedValue)
            {
                expectedValue = _currentQuestion.expectedValue;
            }
            else
            {
                expectedValue = Random.Range(valueRange[0], valueRange[1] + 1);
            }
        }
        else
        {
            expectedValue = Random.Range(valueRange[0], valueRange[1] + 1);
        }
        GenerateCountingElements(expectedValue);
        GenerateAnswers();
        AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
        LessonManager.Instance.StartTimer();
        _wrongAttempts = 0;
    }

    private void Start()
    {
        InitializeGame();
    }
    
    public override void StartGame()
    {
        Debug.Log("Counting Game Started");
    }
    
    private void Update()
    {
        if (Count >= expectedValue && !_isResetElementsScheduled)
        {   
            _isResetElementsScheduled = true;
            Invoke(nameof(ResetCountingElements), 5f);
        }
        if (_isNextGame)
        {
            MoveBackground();
        }
        
    }
    private void GenerateAnswers()
    {   
        // Put the correct answer in a random index
        int correctIndex = Random.Range(0, 3);
        answers[correctIndex] = expectedValue;

        // Generate wrong answers
        for (int i = 0; i < answers.Length; i++)
        {
            if (i != correctIndex)
            {
                int wrongAnswer;
                do
                {
                    wrongAnswer = Random.Range(valueRange[0], valueRange[1] + 1);
                } while (wrongAnswer == expectedValue || System.Array.Exists(answers, a => a == wrongAnswer));

                answers[i] = wrongAnswer;
            }
        }
        
        // Assign answers to buttons
        for (int i = 0; i < answerButtons.Count; i++)
        {
            AnswerButton buttonScript = answerButtons[i].GetComponent<AnswerButton>();
            buttonScript.SetValue(answers[i].ToString());
            buttonScript.SetText(answers[i].ToString()); 
            CanvasGroup canvasGroup = answerButtons[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            // make fade in effect for newElement
            buttonScript.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.3f;
        }
    }

    private void GenerateCountingElements(int numberOfElements)
    {
        // Get GridLayoutGroup and RectTransform of the countingBoard
        var grid = countingArea.GetComponent<GridLayoutGroup>();
        var boardRectTransform = countingArea.GetComponent<RectTransform>();

        // Check if all necessary components are assigned
        if (grid == null || boardRectTransform == null || gridElements == null || gridElements.Length == 0)
        {
            Debug.LogError("GridLayoutGroup, RectTransform, or gridElements array is not properly set!");
            return;
        }

        // Calculate rows and columns based on the number of elements (assuming square layout)
        int columns = Mathf.CeilToInt(Mathf.Sqrt(numberOfElements));
        int rows = Mathf.CeilToInt((float)numberOfElements / columns);

        // Calculate cell size dynamically
        float cellWidth = (boardRectTransform.rect.width - grid.spacing.x * (columns - 1)) / columns;
        float cellHeight = (boardRectTransform.rect.height - grid.spacing.y * (rows - 1)) / rows;
        Debug.Log($"Cell width: {cellWidth}, Cell height: {cellHeight}");
        float cellSize = Mathf.Min(cellWidth, cellHeight); 
        grid.cellSize = new Vector2(cellSize, cellSize);
        
        var selectedElement = gridElements[Random.Range(0, gridElements.Length)];
        // Generate elements
        for (int i = 0; i < numberOfElements; i++)
        {
            var newElement = Instantiate(selectedElement, countingArea.transform);
            newElement.transform.localScale = Vector3.one; // Ensure correct scale
            newElement.transform.rotation = Quaternion.identity; // Reset rotation
            CanvasGroup canvasGroup = newElement.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            // make fade in effect for newElement
            newElement.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
                {
                    AudioManager.Instance.PlaySound("Object Appear Sound");
                });     
            _delay += 0.3f;
        }

        Debug.Log($"Generated {numberOfElements} grid elements with cell size: {grid.cellSize}");
    }
    
    private void ResetCountingElements()
    { 
        ResetCount();
        _isResetElementsScheduled = false;
        foreach (Object element in countingArea.transform)
        {
            element.GetComponent<CountingElement>().Reset();
        }
    }
    
    public override bool CheckWin()
    {
        if (_selectedAnswer == expectedValue.ToString())
        {
            isCompleted = true;
            AudioManager.Instance.PlaySound("Object Done Right place");
            answerBox.GetComponent<AnswerBox>().SetText(expectedValue.ToString());
            answerButtonArea.GameObject().SetActive(false);
            countingArea.GetComponent<RectTransform>().DOMoveY(0, 1).SetDelay(2f);
            answerArea.GetComponent<RectTransform>().DOMoveY(0, 1).SetDelay(2f);
            // Delay 
            DOVirtual.DelayedCall(0.2f, () => AudioManager.Instance.PlaySound("Kids Cheering And Laughing"));
            DOVirtual.DelayedCall(2f, () => AudioManager.Instance.PlaySound(expectedValue.ToString()));
            DOVirtual.DelayedCall(3.2f, () => AudioManager.Instance.PlaySound("Task complete whistel sound"));
            DOVirtual.DelayedCall(3.3f, () => AudioManager.Instance.PlayMusic("Task Complete enjoy music"));
            DOVirtual.DelayedCall(3.35f, SpawnParticles);
            if (_isLearningMode)
            {   
                LessonManager.Instance.UpdateScore(1);
                LessonManager.Instance.UpdateDiamond();
                LessonManager.Instance.NextQuestion();
            
                if (LessonManager.Instance.IsCurrentLessonFinished())
                {
                    DOVirtual.DelayedCall(4f, () => EndGame());
                }
                else DOVirtual.DelayedCall(3.35f, () => nextButton.GameObject().SetActive(true));
            }
            else
            {
                DOVirtual.DelayedCall(3.35f, () => nextButton.GameObject().SetActive(true));
            }
            return true;
        }
        else
        {   
            _wrongAttempts++;
            Debug.Log("Incorrect Answer. Try Again!");
            AudioManager.Instance.PlaySound("lion no no no no");
            
            if (_wrongAttempts == 1)
            {
                Debug.Log("Show Hand Guide");
                ShowHandGuide();
            }
            else if (_wrongAttempts == 2 && _isLearningMode)
            {
                Debug.Log("Open YouTube Help");
                OpenYouTubeHelp();
            }
            else if (_wrongAttempts >= 3 && _isLearningMode)
            {
                // Next question
                LessonManager.Instance.NextQuestion();
                if (LessonManager.Instance.IsCurrentLessonFinished())
                {
                    EndGame();
                }
                else
                {
                    NextGame();
                }
            }
    
            return false;
        }
    }
    
    public override void SetSelectedAnswer(string answer, GameObject answerBox)
    {
        _selectedAnswer = answer;
        CheckWin();
    }
    
    public void NextGame()
    {
        Debug.Log("Next Game!");
        countingArea.GameObject().SetActive(false);
        answerArea.GameObject().SetActive(false);
        answerButtonArea.GameObject().SetActive(false);
        nextButton.GameObject().SetActive(false);
        // Practice mode 
        board.GetComponent<RectTransform>().anchoredPosition = new Vector2(2011, 0);
        DestroyParticles();
        _isNextGame = true;
     }

    public void ResetScene()
    {
        // Deactivate the counting area
        ResetCount();
        // Reload the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void MoveBackground()
    {
        // Move the background to the right 1913 and the left -1913 (rectTransform size)
        Vector2 position = background.GetComponent<RectTransform>().anchoredPosition;
        position.x -= 500 * Time.deltaTime;
        background.GetComponent<RectTransform>().anchoredPosition = position;
        if (position.x <= -1913)
        {
            ResetScene();
        }
    }
    
    private void SpawnParticles()
    {
        foreach (var prefab in particlePrefab)
        {
            GameObject particle = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
            // Set trigger to destroy particle after animation
            particle.GetComponent<ParticleSystem>().trigger.SetCollider(0, ground.GetComponent<BoxCollider2D>());
            _particles.Add(particle);
        }
    }
    
    private void DestroyParticles()
    {
        foreach (var particle in _particles)
        {
            Destroy(particle);
        }
    }
    

    public void ResetCount()
    {
        Count = 0;
    }

    public void IncreaseCountAndPlaySoundCount(GameObject countingElement)
    {
        Count++;
        AudioManager.Instance.PlaySound(Count.ToString());
    }
    
    public override void CloseGame()
    {
        // Stop the music
        AudioManager.Instance.StopMusic();
        board.GameObject().SetActive(false);
        nextButton.GameObject().SetActive(false);
        DestroyParticles();
    }

    public override void EndGame()
    {   
        LessonManager.Instance.StopTimer();
        // Process the result, save the result
        LessonManager.Instance.ProcessLessonResult();
        // Input the result show end game panel 
        _ScoreStatusText.text = LessonManager.Instance.GetScoreText();
        _TimeText.text = LessonManager.Instance.GetCurrentTime();
        _ScoreText.text = LessonManager.Instance.GetCurrentScore();
        _DiamondText.text = LessonManager.Instance.GetCurrentDiamond().ToString();
        endGamePanel.GameObject().SetActive(true);
        LessonManager.Instance.SaveLessonResult();
        LessonManager.Instance.ResetLessonManager();
    }

    private void ShowHandGuide()
    {
        GameObject correctAnswerButton = answerButtons.Find(btn => 
            btn.GetComponent<AnswerButton>().GetValues() == expectedValue.ToString());

        if (correctAnswerButton != null)
        {   
            Debug.Log("Show Hand Guide");
            
            handGuide.transform.SetParent(correctAnswerButton.transform.parent);
            RectTransform handGuideRect = handGuide.GetComponent<RectTransform>();
            RectTransform startRect = correctAnswerButton.GetComponent<RectTransform>();
            Vector2 endRect = new Vector2(790, 370);
            
            // Set the initial position same as in AnswerButton
            handGuideRect.anchoredPosition = startRect.anchoredPosition;
            handGuide.SetActive(true);
            
           
            handGuideRect.DOAnchorPos(endRect, 1.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                handGuide.SetActive(false);
            }); 
        }
        else
        {
            Debug.Log("No correct answer button found");
        }
    }

    private void OpenYouTubeHelp()
    {
        Application.OpenURL(youtubeHelpUrl);
    }
}
