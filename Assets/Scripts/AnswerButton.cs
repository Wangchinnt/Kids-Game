using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value; // Value of the button
    private Vector3 _initialPosition; // Save the initial position
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CountingGame _countingGame;
    private Vector3 _answerBoxPosition;
    [SerializeField] private float tolerance = 2f; 
  
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>(); 
        _initialPosition = _rectTransform.anchoredPosition;
        _countingGame = FindObjectOfType<CountingGame>();
        _answerBoxPosition = _countingGame.answerBox.GetComponent<RectTransform>().position;
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag started");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null) return;

        // Update the position of the button relative to the canvas
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        
        
        
        // Event when dragging close to the answer box
        if (Vector3.Distance(transform.position, _answerBoxPosition) <= tolerance)
        {
            Debug.Log("Dragging close enough to AnswerBox!");
            _countingGame.answerBox.gameObject.GetComponentInChildren<Image>().color = Color.red;
        }
        else
        {
            _countingGame.answerBox.gameObject.GetComponentInChildren<Image>().color = Color.white;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        // Get the position of the answer box, it has rect transform component
        // Event when dropped close to the answer box
        if (Vector3.Distance(transform.position, _answerBoxPosition) <= tolerance)
        {
            Debug.Log("Dropped close enough to AnswerBox!");
            if (_countingGame != null)
            {
                _countingGame.SetSelectedAnswer(value); // Xử lý đáp án
              
            }

            Vector3 answerBoxAnchoredPosition = new Vector3(748, 271, 0);
            _rectTransform.anchoredPosition = _countingGame.CheckWin() ? answerBoxAnchoredPosition : _initialPosition;
        }
        else
        {
            Debug.Log("Dropped too far. Resetting position.");
            ResetPosition();
        }
        _countingGame.answerBox.gameObject.GetComponentInChildren<Image>().color = Color.white;
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
    
}