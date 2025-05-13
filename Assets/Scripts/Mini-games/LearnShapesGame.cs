using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LearnShapesGame : MiniGame
{   
    private string[] shapeCategories = new string[] {"Circle", "Square", "Triangle", "Rectangle", "Rectangular Prism", "Cube"};
    private string selectedShape;
    private string[] answers = new string[9];
    private string expectedAnswer;
    private List<GameObject> correctAnswers;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject ground;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject shapeRequiredAreaObject;
    [SerializeField] private GameObject[] shapeRequiredPrefabs;
    [SerializeField] private GameObject shapeChoosingAreaObject;
    [SerializeField] private GameObject[] shapeAnswersPrefabs;
    [SerializeField] private GameObject nextButton;
    private string _selectedAnswer = "-1"; 
    private float _delay = 0f;
    private bool _isNextGame = false;
    private List<GameObject> _particles = new List<GameObject>();
    
    public override void InitializeGame()
    {
        gameName = "Learn Shapes Game";
        isCompleted = false;
        _selectedAnswer = "-1"; 
        Debug.Log($"{gameName} Initialized");
        // random select a shape from the shapes array 
        board.GameObject().SetActive(true);
        expectedAnswer =  shapeCategories[Random.Range(0, shapeCategories.Length)];
        var answerInVn = expectedAnswer switch
        {
            "Circle" => "Hình tròn",
            "Square" => "Hình vuông",
            "Triangle" => "Hình tam giác",
            "Rectangle" => "Hình chữ nhật",
            "Rectangular Prism" => "Hình hộp chữ nhật",
            "Cube" => "Hình lập phương",
            _ => "Hình không xác định"
        };
        questionText.text = $"Hãy chọn các vật có dạng hình {answerInVn}";
        GenerateAnswers();
        GenerateShapeRequired();
        AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
    }
    
    private void Start()
    {
        InitializeGame();
    }
    private void Update()
    {
        if (_isNextGame)
        {
            MoveBackground();
        }
    }
    public override void StartGame()
    {
        throw new System.NotImplementedException();
    }
    
    private void GenerateShapeRequired()
    {
        // Generate the shape required
        Debug.Log($"The shape required is {expectedAnswer}");
        // add the shape having name equal expectedAnswer to the shapeRequiredAreaObject
        foreach (var shape in shapeRequiredPrefabs)
        {
            if (shape.name == expectedAnswer)
            {
                var obj = Instantiate(shape, shapeRequiredAreaObject.transform);
                CanvasGroup canvasGroup =  obj.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart((() => {
                    AudioManager.Instance.PlaySound("Object Appear Sound");
                }));
                _delay += 0.2f;
            }
        }
        shapeRequiredAreaObject.GameObject().GetComponent<OperatorButton>().SetValue(expectedAnswer);
    }
    
    private void GenerateAnswers()
    {   
        correctAnswers = shapeAnswersPrefabs
            .Where(prefab => prefab.name.StartsWith(expectedAnswer))
            .Take(3)
            .ToList();

        List<GameObject> remainingShapes = shapeAnswersPrefabs
            .Where(prefab => !prefab.name.StartsWith(expectedAnswer))
            .ToList();

        List<GameObject> wrongAnswers = new List<GameObject>();
        HashSet<string> usedCategories = new HashSet<string>();

        // get 6 wrong answers
        foreach (var category in shapeCategories)
        {
            var wrongCandidates = remainingShapes.Where(prefab => prefab.name.StartsWith(category)).ToList();
            if (wrongCandidates.Count > 0)
            {
                var selected = wrongCandidates[Random.Range(0, wrongCandidates.Count)];
                wrongAnswers.Add(selected);
                usedCategories.Add(category);
            }
            if (wrongAnswers.Count == 6) break;
        }

        // if not enough 6 wrong answers, get random from the remaining list
        while (wrongAnswers.Count < 6)
        {
            var randomShape = remainingShapes[Random.Range(0, remainingShapes.Count)];
            if (!wrongAnswers.Contains(randomShape))
            {
                wrongAnswers.Add(randomShape);
            }
        }

        // combine the correct answers and wrong answers and shuffle
        List<GameObject> allAnswers = correctAnswers.Concat(wrongAnswers).ToList();
        allAnswers = allAnswers.OrderBy(a => Random.value).ToList();
        
        correctAnswers.Clear();
        foreach (var answer in allAnswers)
        {
            var obj = Instantiate(answer, shapeChoosingAreaObject.transform);
            // Animation for the obj
            CanvasGroup canvasGroup =  obj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart((() => {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            }));
            _delay += 0.2f;
        }
    }
    
    private bool CheckCorrectAnswer(GameObject answerButton)
    {   
        // if the selected answer is equal to any of the answer box value
        if (_selectedAnswer != expectedAnswer)
        {   
            Debug.Log("Wrong Answer!");
            AudioManager.Instance.PlaySound("lion no no no no");
            answerButton.transform.GetChild(3).gameObject.SetActive(true);
            return false;
        }
        AudioManager.Instance.PlaySound("Object Done Right place");
        // Instantiate correct effect
        Debug.Log("Correct!");
        answerButton.GetComponent<Button>().interactable = false;
        // active Correct image in child
        answerButton.transform.GetChild(2).gameObject.SetActive(true);
        correctAnswers.Add(answerButton);
        CheckWin();
        return true;
    }
    public override bool CheckWin()
    {    
        if (correctAnswers.Any(element => element.GetComponent<Button>().interactable) || correctAnswers.Count < 3) return false;
        AudioManager.Instance.PlaySound("Task complete whistel sound");
        // Instantiate particle effect
        SpawnParticles();
        AudioManager.Instance.PlaySound("Kids Cheering And Laughing");
        AudioManager.Instance.PlayMusic("Task Complete enjoy music");
        nextButton.SetActive(true);
        return true;
    }
    public override void SetSelectedAnswer(string answer, GameObject answerButton)
    {
        _selectedAnswer = answer;
        CheckCorrectAnswer(answerButton);
    }
    
    public void NextGame()
    {
        Debug.Log("Next Game!");
        board.GameObject().gameObject.SetActive(false);
        shapeRequiredAreaObject.GameObject().SetActive(false);
        board.GetComponent<RectTransform>().anchoredPosition = new Vector2(2011, 0);
        DestroyParticles();
        _isNextGame = true;
    }

    public void ResetScene()
    { 
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
        Debug.Log("endGame");
        
    }
}
