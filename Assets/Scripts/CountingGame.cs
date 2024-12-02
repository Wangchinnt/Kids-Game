using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CountingGame : MiniGame
{   
    public GameObject answerBox; 
    
    [SerializeField] private int expectedValue; 
    [SerializeField] private int[] answers = new int[3]; 
    [SerializeField] private List<GameObject> answerButtons;
    [SerializeField] private GameObject countingArea;
    [SerializeField] private GameObject[] gridElements;
    private int _selectedAnswer = -1; 

    public override void InitializeGame()
    {
        gameName = "Counting Game";
        isCompleted = false;
        _selectedAnswer = -1; 
        Debug.Log("Counting Game Initialized");

        
        expectedValue = Random.Range(1, 11);

        
        GenerateAnswers();
        GenerateGridElements(expectedValue);
        
        
        
    }

    public override void StartGame()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void GenerateAnswers()
    {
        // Đặt đáp án đúng vào vị trí ngẫu nhiên
        int correctIndex = Random.Range(0, 3);
        answers[correctIndex] = expectedValue;

        // Sinh 2 giá trị sai nhưng không được trùng với expectedValue
        for (int i = 0; i < answers.Length; i++)
        {
            if (i != correctIndex)
            {
                int wrongAnswer;
                do
                {
                    wrongAnswer = Random.Range(1, 11); // Sinh giá trị sai trong khoảng (0,10)
                } while (wrongAnswer == expectedValue || System.Array.Exists(answers, a => a == wrongAnswer));

                answers[i] = wrongAnswer;
            }
        }
        
        for (int i = 0; i < answerButtons.Count; i++)
        {
            AnswerButton buttonScript = answerButtons[i].GetComponent<AnswerButton>();
            buttonScript.value = answers[i]; // Gán giá trị từ mảng answers
            buttonScript.SetText(answers[i].ToString()); // Hiển thị số lên nút
        }
    }

    private void GenerateGridElements(int numberOfElements)
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
        foreach (Transform child in countingArea.transform)
        {
            Destroy(child.gameObject);
        }
        var selectedElement = gridElements[Random.Range(0, gridElements.Length)];
        // Generate elements
        for (int i = 0; i < numberOfElements; i++)
        {
            var newElement = Instantiate(selectedElement, countingArea.transform);
            newElement.transform.localScale = Vector3.one; // Ensure correct scale
            newElement.transform.rotation = Quaternion.identity; // Reset rotation
        }

        Debug.Log($"Generated {numberOfElements} grid elements with cell size: {grid.cellSize}");
    }
    

    public override bool CheckWin()
    {
        if (_selectedAnswer == expectedValue)
        {
            isCompleted = true;
            EndGame();
            Debug.Log("Correct Answer!");
            return true;
        }
        else
        {
            Debug.Log("Incorrect Answer. Try Again!");
            return false;
        }
    }

    // Khi người chơi thả nút vào AnswerBox
    public void SetSelectedAnswer(int value)
    {
        _selectedAnswer = value;
        CheckWin();
    }
}
