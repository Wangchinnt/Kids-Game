using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TextMeshProUGUI = TMPro.TextMeshProUGUI;
using GameData;
public class ComparingGame : MiniGame, IHaveCountingElement
{
    [SerializeField] private int expectedValue;
    [SerializeField] private int[] valueRange = new int[2] { 1, 10 };
    [SerializeField] private int[] answers = new int[3] {-2,0,-1};
    [SerializeField] private int[] operators = new int[2];
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private GameObject countingArea1;
    [SerializeField] private GameObject countingArea2;
    [SerializeField] private GameObject[] operatorButtons;
    [SerializeField] private GameObject answerArea;
    [SerializeField] private GameObject answerBox;
    [SerializeField] private GameObject answerButtonArea;
    [SerializeField] private GameObject[] gridElements;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject handGuide;
    private string youtubeHelpUrl;
    private bool _isLearningMode = false;
    private QuestionConfig _currentQuestion;
    private string _selectedAnswer;
    private float _delay = 0f;
    private int _count1 = 0;
    private int _count2 = 0;
    public int Count { get; set; }
    
    private bool _isNextGame = false;
    private bool _isResetElementsScheduledInArea1 = false;
    private bool _isResetElementsScheduledInArea2 = false;
    private List<GameObject> _particles = new List<GameObject>();
    private int _wrongAttempts = 0;
    [SerializeField] private TextMeshProUGUI _scoreStatusText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _diamondText;
    [SerializeField] private GameObject _endGamePanel;
    
    public override void InitializeGame()
    { 
        gameName = "Comparing Game";
        isCompleted = false;
        _isLearningMode = LessonManager.Instance.IsLearningMode();
        
        if (_isLearningMode)
        {   
            youtubeHelpUrl = LessonManager.Instance.GetCurrentLesson().videoSupportLink;
            _currentQuestion = LessonManager.Instance.GetCurrentQuestion();
            // Set giá trị cho câu hỏi từ LessonManager
            _selectedAnswer = _currentQuestion.expectedValue.ToString();
            Debug.Log("Comparing Game Initialized");
            countingArea1.SetActive(true);
            countingArea2.SetActive(true);
            answerArea.SetActive(true);
            answerButtonArea.SetActive(true);
            GenerateOperators();
            GenerateCountingElements(operators[0], countingArea1);
            GenerateCountingElements(operators[1], countingArea2);
            GenerateAnswers();
            AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
            
            _wrongAttempts = 0;
            LessonManager.Instance.StartTimer();
        }
        else
        {
            _selectedAnswer = "-3";
            Debug.Log("Comparing Game Initialized");
            countingArea1.SetActive(true);
            countingArea2.SetActive(true);
            answerArea.SetActive(true);
            answerButtonArea.SetActive(true);
            GenerateOperators();
            GenerateCountingElements(operators[0], countingArea1);
            GenerateCountingElements(operators[1], countingArea2);
            GenerateAnswers();
            AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
        }
    }

    public override void StartGame()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        InitializeGame();
    }
    
    private void Update()
    {
        if (_count1 >= operators[0] && !_isResetElementsScheduledInArea1)
        {   
            _isResetElementsScheduledInArea1 = true;
            Invoke(nameof(ResetCountingElements), 5f);
        }
        if (_count2 >= operators[1] && !_isResetElementsScheduledInArea2)
        {   
            _isResetElementsScheduledInArea2 = true;
            Invoke(nameof(ResetCountingElements), 5f);
        }
        if (_isNextGame)
        {   
            MoveBackground();
        }
    }
    
    private void GenerateOperators()
    {   
        if (_isLearningMode && _currentQuestion.isFixedValue)
        {
            int comparisonResult;
            do
            {
                operators[0] = Random.Range(valueRange[0], valueRange[1] + 1);
                operators[1] = Random.Range(valueRange[0], valueRange[1] + 1);
                
                // Calculate the comparison result
                if (operators[0] > operators[1])
                    comparisonResult = -2;  // Greater than
                else if (operators[0] == operators[1])
                    comparisonResult = 0;   // Equal
                else
                    comparisonResult = -1;  // Less than
                    
            } while (comparisonResult != _currentQuestion.expectedValue);
        }
        else
        {
            // Practice mode - random normally
            operators[0] = Random.Range(valueRange[0], valueRange[1] + 1);
            operators[1] = Random.Range(valueRange[0], valueRange[1] + 1);
        }

        operatorButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = operators[0].ToString();
        operatorButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = operators[1].ToString();
        
        // Calculate expectedValue for the current question
        expectedValue = operators[0] > operators[1] ? -2 : 
                       operators[0] == operators[1] ? 0 : -1;

        foreach (var operatorBtn in operatorButtons)
        {
            operatorBtn.SetActive(true);
        }

        // Animation fade in cho các nút
        for (int i = 0; i < operatorButtons.Length; i++)
        {
            CanvasGroup canvasGroup = operatorButtons[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.5f;
        }
    }
    
     private void GenerateCountingElements(int numberOfElements, GameObject countingArea)
    {
     
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
    }
     
    private void GenerateAnswers()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {   
            AnswerButton buttonScript = answerButtons[i].GetComponent<AnswerButton>();
            buttonScript.SetValue(answers[i].ToString());
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

     private void ResetCountingElements()
    { 
        ResetCount();
        if (_isResetElementsScheduledInArea1)
        {
            _count1 = 0;
            _isResetElementsScheduledInArea1 = false;
            foreach (Object element in countingArea1.transform)
            {
                element.GetComponent<CountingElement>().Reset();
            } 
        }
        else if(_isResetElementsScheduledInArea2)
        {
            _count2 = 0;
            _isResetElementsScheduledInArea2 = false;
            foreach (Object element in countingArea2.transform)
            {
                element.GetComponent<CountingElement>().Reset();
            }
        }
    }
    public override bool CheckWin()
    {
        if (_selectedAnswer == expectedValue.ToString())
        {
            isCompleted = true;
            DOVirtual.DelayedCall(0.2f, () => AudioManager.Instance.PlaySound("Kids Cheering And Laughing"));
            DOVirtual.DelayedCall(2f, () => AudioManager.Instance.PlaySound(expectedValue.ToString()));
            DOVirtual.DelayedCall(3.2f, () => AudioManager.Instance.PlaySound("Task complete whistel sound"));
            DOVirtual.DelayedCall(3.3f, () => AudioManager.Instance.PlayMusic("Task Complete enjoy music"));
            DOVirtual.DelayedCall(3.35f, SpawnParticles);
           
            var text = _selectedAnswer == "0" ? "=" : _selectedAnswer == "-1" ? "<" : ">";
            answerBox.GetComponent<AnswerBox>().SetText(text);
            answerButtons.ForEach(button => button.GameObject().SetActive(false));
            
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
        ResetCountingElements();
        countingArea1.GameObject().SetActive(false);
        countingArea2.GameObject().SetActive(false);
        answerArea.GameObject().SetActive(false);
        answerButtonArea.GameObject().SetActive(false);
        foreach (var operatorBtn in operatorButtons)
        {
            operatorBtn.GameObject().SetActive(false);
        }
        nextButton.GameObject().SetActive(false);
        board.GetComponent<RectTransform>().anchoredPosition = new Vector2(2011, 0);
        DestroyParticles();
        _isNextGame = true;
    }
    public void Reset()
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
            Reset();
        }
    }
    private void SpawnParticles()
    {
        foreach (var prefab in particlePrefab)
        {
            GameObject particle = Instantiate(prefab, spawnPoint.transform.position, prefab.transform.rotation);
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
        // if countingElement is countingArea1 child then
        if (countingElement != null && countingElement.transform.IsChildOf(countingArea1.transform))
        {
            _count1++;
            Count = _count1;
        }
        else
        {
            _count2++;
            Count = _count2;
        }
        AudioManager.Instance.PlaySound(Count.ToString());
    }

    public override void CloseGame()
    {   
        AudioManager.Instance.StopMusic();
        board.GameObject().SetActive(false);
        nextButton.GameObject().SetActive(false);
        DestroyParticles();
    }
    public override void EndGame()
    {   
        LessonManager.Instance.StopTimer();
        LessonManager.Instance.ProcessLessonResult();
        
        // Hiển thị kết quả
        _scoreStatusText.text = LessonManager.Instance.GetScoreText();
        _timeText.text = LessonManager.Instance.GetCurrentTime();
        _scoreText.text = LessonManager.Instance.GetCurrentScore();
        _diamondText.text = LessonManager.Instance.GetCurrentDiamond().ToString();
        
        _endGamePanel.GameObject().SetActive(true);
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
            
            // Đảm bảo handGuide nằm trong cùng Canvas với answerButtons
            handGuide.transform.SetParent(correctAnswerButton.transform.parent);
            RectTransform handGuideRect = handGuide.GetComponent<RectTransform>();
            RectTransform startRect = correctAnswerButton.GetComponent<RectTransform>();
            Vector2 endRect = new Vector2(-550, -300);
            Debug.Log("Start Rect: " + startRect.anchoredPosition);
            Debug.Log("End Rect: " + endRect);
            // Set vị trí ban đầu giống như trong AnswerButton
            handGuideRect.anchoredPosition = startRect.anchoredPosition;
            handGuide.SetActive(true);
            
            // Tạo animation di chuyển sử dụng anchoredPosition lặp lại 2 lần
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

    public void ResetScene()
    {
        // Reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
