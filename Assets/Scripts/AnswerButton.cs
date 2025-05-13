using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class AnswerButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string _value;
    private Vector3 _initialPosition; // Save the initial position
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Button _button;
    public bool isClickable = true;
    [SerializeField] private float tolerance = 2f; 
    [SerializeField] private List<GameObject> answerBoxs;
    private MiniGame _currentGame;
    
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    Direction VectorToDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return Direction.Up;
        if (dir == Vector2Int.down) return Direction.Down;
        if (dir == Vector2Int.left) return Direction.Left;
        if (dir == Vector2Int.right) return Direction.Right;

        throw new Exception("Hướng không hợp lệ");
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>(); 
        _button = GetComponent<Button>();
        _initialPosition = _rectTransform.anchoredPosition;
        _currentGame = FindObjectOfType<MiniGame>();
        // get all answer boxs in the scene
        answerBoxs = new List<GameObject>(GameObject.FindGameObjectsWithTag("AnswerBox"));
        transform.DOShakeScale(1, 0.1f, 1, 0, false, ShakeRandomnessMode.Harmonic).SetLoops(-1);
        // Add a listener to the button
        _button.onClick.AddListener(OnButtonClick);     
    }

    private void OnButtonClick()
    {
        // If the button is not dragging 
        if (_rectTransform.anchoredPosition == (Vector2) _initialPosition && isClickable)
        {   
            Debug.Log($"Button {_value} clicked");
            // shake the rotation of the button
            
            transform.DOShakeRotation(0.8f, new Vector3(0, 0, 10), 10, 90, true, ShakeRandomnessMode.Harmonic);
            
            AudioManager.Instance.PlaySound(_value);
            isClickable = false;
            Invoke(nameof(ResetClickable), 1f);
        }
       
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag started");
        AudioManager.Instance.PlaySound(_value);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null) return;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        GameObject closestAnswerBox = null;
        float minDistance = float.MaxValue;

        foreach (var t in answerBoxs)
        {
            var answerBoxPosition = t.GetComponent<RectTransform>().position;
            float distance = Vector2.Distance(transform.position, answerBoxPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestAnswerBox = t;
            }
        }

        // Đặt tất cả AnswerBox về màu trắng
        foreach (var t in answerBoxs)
        {
            t.GetComponentInChildren<Image>().color = Color.white;
        }

        if (closestAnswerBox != null && minDistance <= tolerance)
        {
            Debug.Log("Dragging close enough to the closest AnswerBox!");
            closestAnswerBox.GetComponentInChildren<Image>().color = Color.red;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //Find the closest AnswerBox
        GameObject closestAnswerBox = null;
        float minDistance = float.MaxValue;

        foreach (var t in answerBoxs)
        {
            var answerBoxPosition = t.GetComponent<RectTransform>().position;
            float distance = Vector2.Distance(transform.position, answerBoxPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestAnswerBox = t;
            }
        }

        // Check if the closest AnswerBox is within the tolerance
        if (closestAnswerBox != null && minDistance <= tolerance)
        {
            Debug.Log("Dropped close enough to AnswerBox!");
            if (_currentGame != null)
            {
                Debug.Log("Setting selected answer");
                _currentGame.SetSelectedAnswer(_value, closestAnswerBox);
            }
            else
            {
                Debug.Log("Current game is null");
            }
        }
        else
        {
            Debug.Log("Dropped too far. Resetting position.");
        }

        ResetPosition();
        transform.DOShakeRotation(0.8f, new Vector3(0, 0, 10), 10, 90, true, ShakeRandomnessMode.Harmonic);

        foreach (var t in answerBoxs)
        {
            t.GetComponentInChildren<Image>().color = Color.white;
        }
    }
    
    
    // Display the value on the button
    public void SetText(string text)
    {
        var buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }
    private void ResetPosition()
    {
        _rectTransform.anchoredPosition = _initialPosition;
    }
    
    private void ResetClickable()
    {
        isClickable = true;
    }
    
    public void SetValue(string value)
    {
        _value = value;
    }
    public string GetValues()
    {
        return _value;
    }
    public void SetAnswerBoxs(List<GameObject> answerBoxs)
    {
        this.answerBoxs = answerBoxs;
    }
}