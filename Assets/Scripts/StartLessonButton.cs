using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartLessonButton : MonoBehaviour
{
    // get button
    [SerializeField] private Button _startLessonButton;
    // get lesson id
    [SerializeField] private int _lessonId;
    // get is learning mode
    
    private void Start()
    {
        _startLessonButton = GetComponent<Button>();
        _lessonId = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1));
        _startLessonButton.onClick.AddListener(OnClick);
    }
    // on click
    private void OnClick()
    {
        LessonManager.Instance.StartLesson(_lessonId);
    }
    
}
