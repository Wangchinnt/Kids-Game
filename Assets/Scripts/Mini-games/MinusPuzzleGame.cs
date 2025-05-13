using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MinusPuzzleGame : MiniGame
{
    [SerializeField] private string[] expectedAnswer;
    [SerializeField] private int[] operationRange = new int[2] { 1, 10 };
    [SerializeField] private string[] answers = new string [5];
    [SerializeField] private int[] addends = new int[2] { 0, 0 };
    [SerializeField] private GameObject answerButtonArea;
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private GameObject answerArea;
    [SerializeField] private List<GameObject> answerBoxList;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject ground;

    private string _selectedAnswer = "w";

    private float _delay = 0;
    
    private bool _isNextGame = false;
    private List<GameObject> _particles = new List<GameObject>();


    public override void InitializeGame()
    {
        gameName = "MinusPuzzleGame";
        isCompleted = false;
        Debug.Log($"{gameName} initialized");
        
        GenerateOperation();
        GenerateAnswers();
        AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");
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
        if (_isNextGame)
        {
            MoveBackground();
        }
        
    }

    private void GenerateOperation()
    {
        addends[0] = Random.Range(operationRange[0], operationRange[1]);
        addends[1] = Random.Range(operationRange[0], operationRange[1]);
        while (addends[0] < 2 || addends[1] >= addends[0])
        {
            addends[0] = Random.Range(operationRange[0], operationRange[1]);
            addends[1] = Random.Range(operationRange[0], operationRange[1]);
        }
        answers[0] = addends[0].ToString();
        answers[1] = "-";
        answers[2] = addends[1].ToString();
        answers[3] = "=";
        answers[4] = (addends[0] - addends[1]).ToString();

        for (int i = 0; i < answerBoxList.Count; i++)
        {
            answerBoxList[i].SetActive(true);
            answerBoxList[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];
            answerBoxList[i].GetComponent<AnswerBox>().SetValue(answers[i]);
            CanvasGroup canvasGroup =  answerBoxList[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.5f;
        }
    }
    private void GenerateAnswers()
    {   
        var rand = new System.Random();
        var suffledAnswers = answers.OrderBy(x => rand.Next()).ToList();
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].SetActive(true);
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = suffledAnswers[i];
            answerButtons[i].GetComponent<AnswerButton>().SetValue(suffledAnswers[i]);
            CanvasGroup canvasGroup =  answerButtons[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.5f;
        }
    }

    private bool CheckCorrectAnswer(GameObject answerBox)
    {   
        // if the selected answer is equal to any of the answer box value
        if (_selectedAnswer == answerBox.GetComponent<AnswerBox>().GetValue())
        {
            AudioManager.Instance.PlaySound("Object Done Right place");
            // Instantiate particle effect
            var textMeshPro = answerBox.GetComponentInChildren<TextMeshProUGUI>();
            Color32 currentColor = textMeshPro.color; // Lấy màu hiện tại
            textMeshPro.color = new Color32(currentColor.r, currentColor.g, currentColor.b, 255);
            // Inactive the answer button having _selectedAnswer
            foreach (var button in answerButtons)
            {
                if (button.GameObject().activeSelf && button.GetComponentInChildren<TextMeshProUGUI>().text == _selectedAnswer)
                {
                    button.GameObject().SetActive(false); // Ẩn nút
                    break; // Dừng vòng lặp sau khi tìm thấy
                }
            }
            return true;
        }
        AudioManager.Instance.PlaySound("lion no no no no");
        return false;
    }
    public override bool CheckWin()
    {
        // if all answer boxes are filled
        var isWin = answerButtons.All(button => !button.gameObject.activeSelf);
        if (!isWin) return false;
        // move all answer boxes x position to 0
        answerArea.GetComponent<RectTransform>().DOMoveY(0, 1);
        
        AudioManager.Instance.PlaySound("Task complete whistel sound");
        // Instantiate particle effect
        SpawnParticles();
        AudioManager.Instance.PlaySound("Kids Cheering And Laughing");
        AudioManager.Instance.PlayMusic("Task Complete enjoy music");
        nextButton.SetActive(true);
        
        return true;
    }

    public override void SetSelectedAnswer(string answer, GameObject answerBox)
    {   
        _selectedAnswer = answer;
        Debug.Log($"currentSelectedAnswer {_selectedAnswer}");
        CheckCorrectAnswer(answerBox);
        CheckWin();
    }
    
    public void NextGame()
    {
        Debug.Log("Next Game!");
        answerArea.GameObject().SetActive(false);
        answerButtonArea.GameObject().SetActive(false);
        nextButton.GameObject().SetActive(false);
        board.GetComponent<RectTransform>().anchoredPosition = new Vector2(2011, 0);
        DestroyParticles();
        _isNextGame = true;
    }
    private void ResetScene()
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
