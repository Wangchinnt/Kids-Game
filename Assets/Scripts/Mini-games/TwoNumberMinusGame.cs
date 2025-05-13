using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class TwoNumberMinusGame : MiniGame
{
    [SerializeField] private int[] operationRange = new int[2] { 1, 10 };
    [SerializeField] private int[] addends = new int[2] { 0, 0 };
    [SerializeField] private List<string> answers;
    [SerializeField] private GameObject answerAreaGameObject;
    [SerializeField] private GameObject answerButtonArea;
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private List<GameObject> answerAreas;
    [SerializeField] private List<GameObject> answerBoxList;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject[] particlePrefab;
    [SerializeField] private GameObject operatorPrefab;
    [SerializeField] private GameObject answerAreaPrefab;
    [SerializeField] private GameObject ground;
    
    private List<GameObject> _answerBoxs = new List<GameObject>();
    private string _selectedAnswer = "w";
    private float _delay = 0;
    
    private bool _isNextGame = false;
    private List<GameObject> _particles = new List<GameObject>();
    
    public override void InitializeGame()
    {
        gameName = "TwoNumberMinusGame";
        isCompleted = false;
        Debug.Log($"{gameName} initialized");
        answerAreaGameObject.SetActive(true);
        answerButtonArea.SetActive(true);
        GenerateOperations();
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
    private int GenerateTwoDigitNonBorrow()
    {
        int tens = Random.Range(1, 10);  // Hàng chục từ 1 đến 9
        int units = Random.Range(0, 10); // Hàng đơn vị từ 0 đến 9
        return tens * 10 + units;
    }
    private bool IsNonBorrow(int a, int b)
    {
        return (a % 10 >= b % 10); // Kiểm tra hàng đơn vị của số bị trừ lớn hơn hoặc bằng số trừ
    }

    private void GenerateOperations()
    {
        foreach (var answerArea in answerAreas)
        {
            int format = Random.Range(0, 3); 
            int minuend, subtrahend, answerValue;

            do
            {
                minuend = GenerateTwoDigitNonBorrow();
                subtrahend = GenerateTwoDigitNonBorrow();
                answerValue = minuend - subtrahend;
            } while (answerValue <=10 || !IsNonBorrow(minuend, subtrahend)); // Đảm bảo không mượn và không âm và là số có 2 chữ số

            addends[0] = minuend;
            addends[1] = subtrahend;

            switch (format)
            {
                case 0: 
                    // minuend - subtrahend = ?
                    break;

                case 1: 
                    // minuend - ? = answerValue (tìm subtrahend)
                    do
                    {
                        subtrahend = minuend - answerValue;
                    } while (subtrahend < 10 || subtrahend >= 100 || !IsNonBorrow(minuend, subtrahend));
                    addends[1] = subtrahend;
                    break;

                case 2: 
                    // ? - subtrahend = answerValue (tìm minuend)
                    do
                    {
                        minuend = answerValue + subtrahend;
                    } while (minuend < 10 || minuend >= 100 || !IsNonBorrow(minuend, subtrahend));
                    addends[0] = minuend;
                    break;
            }

            switch (format) 
            {
                case 0:
                    // addends[0] + addends[1] = ?
                    GameObject addend1 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    addend1.GetComponent<OperatorButton>().SetValue(addends[0].ToString());
                    addend1.transform.SetParent(answerArea.transform);
                    addend1.transform.localScale = Vector3.one;
                    addend1.transform.localPosition = Vector3.zero;
                    
                    GameObject plus = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    plus.GetComponent<OperatorButton>().SetValue("-");
                    plus.transform.SetParent(answerArea.transform);
                    plus.transform.localScale = Vector3.one;
                    plus.transform.localPosition = Vector3.zero;

                    GameObject addend2 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    addend2.GetComponent<OperatorButton>().SetValue(addends[1].ToString());
                    addend2.transform.SetParent(answerArea.transform);
                    addend2.transform.localScale = Vector3.one;
                    addend2.transform.localPosition = Vector3.zero;

                    GameObject equal = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    equal.GetComponent<OperatorButton>().SetValue("=");
                    equal.transform.SetParent(answerArea.transform);
                    equal.transform.localScale = Vector3.one;
                    equal.transform.localPosition = Vector3.zero;

                    GameObject answer = Instantiate(answerAreaPrefab, answerAreaPrefab.transform.position, Quaternion.identity);
                    answer.GetComponent<AnswerBox>().SetValue(answerValue.ToString());
                    answer.transform.SetParent(answerArea.transform);
                    answer.transform.localScale = Vector3.one;
                    answer.transform.localPosition = Vector3.zero;
                    answers.Add(answerValue.ToString());
                    break;

                case 1:
                        // addends[0] + ? = answerValue
                    GameObject addend1_case1 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    addend1_case1.GetComponent<OperatorButton>().SetValue(addends[0].ToString());
                    addend1_case1.transform.SetParent(answerArea.transform);
                    addend1_case1.transform.localScale = Vector3.one;
                    addend1_case1.transform.localPosition = Vector3.zero;
                        
                    GameObject plus_case1 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    plus_case1.GetComponent<OperatorButton>().SetValue("-");
                    plus_case1.transform.SetParent(answerArea.transform);
                    plus_case1.transform.localScale = Vector3.one;
                    plus_case1.transform.localPosition = Vector3.zero;
                        
                    GameObject unknown1 = Instantiate(answerAreaPrefab, answerAreaPrefab.transform.position, Quaternion.identity);
                    unknown1.GetComponent<AnswerBox>().SetValue(addends[1].ToString());
                    unknown1.transform.SetParent(answerArea.transform);
                    unknown1.transform.localScale = Vector3.one;
                    unknown1.transform.localPosition = Vector3.zero;
                        
                    GameObject equal_case1 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    equal_case1.GetComponent<OperatorButton>().SetValue("=");
                    equal_case1.transform.SetParent(answerArea.transform);
                    equal_case1.transform.localScale = Vector3.one;
                    equal_case1.transform.localPosition = Vector3.zero;
                        
                    GameObject answer_case1 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    answer_case1.GetComponent<OperatorButton>().SetValue(answerValue.ToString());
                    answer_case1.transform.SetParent(answerArea.transform);
                    answer_case1.transform.localScale = Vector3.one;
                    answer_case1.transform.localPosition = Vector3.zero;
                    answers.Add(addends[1].ToString());
                    break;

                case 2:
                    // ? + addends[1] = answerValue
                    GameObject unknown2 = Instantiate(answerAreaPrefab, answerAreaPrefab.transform.position, Quaternion.identity);
                    unknown2.GetComponent<AnswerBox>().SetValue(addends[0].ToString());
                    unknown2.transform.SetParent(answerArea.transform);
                    unknown2.transform.localScale = Vector3.one;
                    unknown2.transform.localPosition = Vector3.zero;
                    
                    GameObject plus_case2 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    plus_case2.GetComponent<OperatorButton>().SetValue("-");
                    plus_case2.transform.SetParent(answerArea.transform);
                    plus_case2.transform.localScale = Vector3.one;
                    plus_case2.transform.localPosition = Vector3.zero;
                    
                    GameObject addend2_case2 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    addend2_case2.GetComponent<OperatorButton>().SetValue(addends[1].ToString());
                    addend2_case2.transform.SetParent(answerArea.transform);
                    addend2_case2.transform.localScale = Vector3.one;
                    addend2_case2.transform.localPosition = Vector3.zero;
                    
                    GameObject equal_case2 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    equal_case2.GetComponent<OperatorButton>().SetValue("=");
                    equal_case2.transform.SetParent(answerArea.transform);
                    equal_case2.transform.localScale = Vector3.one;
                    equal_case2.transform.localPosition = Vector3.zero;
                    
                    GameObject answer_case2 = Instantiate(operatorPrefab, operatorPrefab.transform.position, Quaternion.identity);
                    answer_case2.GetComponent<OperatorButton>().SetValue(answerValue.ToString());
                    answer_case2.transform.SetParent(answerArea.transform);
                    answer_case2.transform.localScale = Vector3.one;
                    answer_case2.transform.localPosition = Vector3.zero;
                    
                    answers.Add(addends[0].ToString());
                    break;
            }

            CanvasGroup canvasGroup = answerArea.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart(() =>
            {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            });
            _delay += 0.2f;
           
        }
        answers.Add(Random.Range(operationRange[0], operationRange[1]).ToString());
    }
    
    private void GenerateAnswers()
    {   
        var rand = new System.Random();
        var suffledAnswers = answers.OrderBy(x => rand.Next()).ToList();
        _answerBoxs = new List<GameObject>(GameObject.FindGameObjectsWithTag("AnswerBox"));
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = suffledAnswers[i];
            answerButtons[i].GetComponent<AnswerButton>().SetValue(suffledAnswers[i]);
            answerButtons[i].GetComponent<AnswerButton>().SetAnswerBoxs(_answerBoxs);
            CanvasGroup canvasGroup =  answerButtons[i].GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.5f).SetDelay(_delay).OnStart((() => {
                AudioManager.Instance.PlaySound("Object Appear Sound");
            }));
            _delay += 0.2f;
        }
    }

    private void SufferAnswers()
    {
        var rand = new System.Random();
        var suffledAnswers = answers.OrderBy(x => rand.Next()).ToList();
        Sequence sequence = DOTween.Sequence(); // Tạo một chuỗi animation


        for (int i = 0; i < answerButtons.Count; i++)
        {
            CanvasGroup canvasGroup = answerButtons[i].GetComponent<CanvasGroup>();

            sequence.Join(answerButtons[i].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
            sequence.Join(canvasGroup.DOFade(0, 0.5f));
        }
        
        sequence.OnComplete(() =>
        {   
            DOVirtual.DelayedCall(0.5f, () => // Delay 5 giây sau khi hoàn thành
            {   
                
                AudioManager.Instance.PlaySound("Object Appear Sound");
                // Code hiển thị lại các button sau 5s
                for (int i = 0; i < answerButtons.Count; i++)
                {
                    CanvasGroup canvasGroup = answerButtons[i].GetComponent<CanvasGroup>();

                    answerButtons[i].SetActive(true);
                    answerButtons[i].transform.DOKill(true);
                    answerButtons[i].transform.localScale = Vector3.zero;
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = suffledAnswers[i];
                    answerButtons[i].GetComponent<AnswerButton>().SetValue(suffledAnswers[i]);
                    canvasGroup.alpha = 1;
                    answerButtons[i].transform.DOScale(Vector3.one * 1.5f, 0.5f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            // Sau khi xuất hiện xong, kích hoạt hiệu ứng rung
                            answerButtons[i].transform.DOShakeScale(1, 0.1f, 1, 0, false, ShakeRandomnessMode.Harmonic).SetLoops(-1);
                        });
                }
            });
        });
    }
    private bool CheckCorrectAnswer(GameObject answerBox)
    {   
        // if the selected answer is equal to any of the answer box value
        if (_selectedAnswer == answerBox.GetComponent<AnswerBox>().GetValue())
        {
            AudioManager.Instance.PlaySound("Object Done Right place");
            // Instantiate particle effect
            
            // Setting the text of the answer box to the selected answer
            var textMeshPro = answerBox.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = _selectedAnswer;
            Color32 currentColor = textMeshPro.color; // Lấy màu hiện tại
            textMeshPro.color = new Color32(currentColor.r, currentColor.g, currentColor.b, 255);
            // just suffle the answers again when still have answer box with text = "?"
            if (answerAreas.Any(area 
                => area.GetComponentInChildren<AnswerBox>().GetText() == "?"))  SufferAnswers();
            return true;
        }
        
        AudioManager.Instance.PlaySound("lion no no no no");
        return false;
    }
    
    public override bool CheckWin()
    {
        if (answerAreas.Any(answerArea 
                => answerArea.GetComponentInChildren<AnswerBox>().GetText() == "?")) return false;
        AudioManager.Instance.PlaySound("Task complete whistel sound");
        // Instantiate particle effect
        SpawnParticles();
        AudioManager.Instance.PlaySound("Kids Cheering And Laughing");
        AudioManager.Instance.PlayMusic("Task Complete enjoy music");
        answerButtonArea.GameObject().SetActive(false);
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
        nextButton.GameObject().SetActive(false);
        answerAreaGameObject.SetActive(false);
        answerButtonArea.SetActive(false);
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
