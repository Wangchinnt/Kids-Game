using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FunnyMinusGame : MiniGame, IHaveCountingElement
{
    [SerializeField]  GameObject answerBox;

    [SerializeField] private int expectedValue;
    [SerializeField] private int[] valueRange = new int[2] { 1, 10 };
    [SerializeField] private int[] answers = new int[3];
    [SerializeField] private int[] operators = new int[2];
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private GameObject countingArea1;
    [SerializeField] private GameObject operatorArea;
    [SerializeField] private GameObject[] operatorButtons;
    [SerializeField] private GameObject answerArea;
    [SerializeField] private GameObject answerButtonArea;
    [SerializeField] private GameObject[] gridElements;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject spawnPoint;
    private string _selectedAnswer = "-1";
    private float _delay = 0f;
    public int Count { get; set; }
    private bool _isNextGame = false;
    private bool _isResetElementsScheduled = false;
    
    private List<GameObject> _particles = new List<GameObject>();

    public override void InitializeGame()
    {
        gameName = "Funny Minus Game";
        isCompleted = false;
        _selectedAnswer = "-1";
        Debug.Log("Funny Minus Game Initialized");
        countingArea1.GameObject().SetActive(true);
        answerArea.GameObject().SetActive(true);
        operatorArea.GameObject().SetActive(true);
        answerButtonArea.GameObject().SetActive(true);
        GenerateOperators();
        GenerateAnswers();
        AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
        ResetCount();
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
    private void GenerateOperators()
    {
        operators[0] = Random.Range(valueRange[0], valueRange[1] + 1);
        // for minus operator
        while (operators[1] == operators[0] || operators[1] > operators[0] || expectedValue == 0)
        {
            operators[1] = Random.Range(valueRange[0], operators[0] + 1);
            expectedValue = operators[0] - operators[1];
        }
        GenerateCountingElements(expectedValue, countingArea1);
        operatorButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = operators[0].ToString();
        operatorButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = operators[1].ToString();
        for (int i = 0; i < operatorButtons.Length; i++)
        {
            
            CanvasGroup canvasGroup = operatorButtons[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            // make fade in effect for newElement
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.5f;
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
        Shuffle(answers);
        
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
            _delay += 0.5f;
        }
    }

    private void Shuffle(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    // generate counting elements on the counting area 1 based on the operators 0
    private void GenerateCountingElements(int numberOfElements, GameObject countingArea)
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

        // Clear any existing children (optional)
     
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
            _delay += 0.5f;
        }

        Debug.Log($"Generated {numberOfElements} grid elements with cell size: {grid.cellSize} on {countingArea.name}");
    }
    
    private void ResetCountingElements()
    { 
        ResetCount();
        _isResetElementsScheduled = false;
        foreach (Object element in countingArea1.transform)
        {
            element.GetComponent<CountingElement>().Reset();
        }
    }
    public override bool CheckWin()
    {
        if (_selectedAnswer == expectedValue.ToString())
        {
            isCompleted = true;
            AudioManager.Instance.PlaySound("Task complete whistel sound");
            AudioManager.Instance.PlaySound("Kids Cheering And Laughing");
            SpawnParticles();
            // delay 1s to play the correct sound
            AudioManager.Instance.PlaySound("Object Done Right place");
            AudioManager.Instance.PlayMusic("Task Complete enjoy music");
            nextButton.GameObject().SetActive(true);
            answerBox.GetComponent<AnswerBox>().SetText(expectedValue.ToString());
            answerButtons.ForEach(button => button.GameObject().SetActive(false));
            // answerBox.GetComponent<RectTransform>().DOMoveX(0, 1);
            // countingArea.GetComponent<RectTransform>().DOMoveX(0, 1);
            AudioManager.Instance.PlaySound(expectedValue.ToString());
            return true;
        }
        else
        {
            Debug.Log("Incorrect Answer. Try Again!");
            AudioManager.Instance.PlaySound("lion no no no no");
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
        answerArea.GameObject().SetActive(false);
        operatorArea.GameObject().SetActive(false);
        answerButtonArea.GameObject().SetActive(false);
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

    public void IncreaseCountAndPlaySoundCount(GameObject countElement)
    {
        Count++;
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
        Debug.Log("endGame");
        
    }
}
