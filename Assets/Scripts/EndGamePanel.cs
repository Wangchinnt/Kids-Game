using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _continueLearningButton;
    [SerializeField] private Button _homeButton;

    [SerializeField] private Button _closeButton;

    private void Start()
    {   
        if(_replayButton == null){  
            _replayButton = transform.Find("ReplayButton").GetComponent<Button>();
        }
        if(_continueLearningButton == null){
            _continueLearningButton = transform.Find("ContinueLearningButton").GetComponent<Button>();
        }
        if(_homeButton == null){
            _homeButton = transform.Find("HomeButton").GetComponent<Button>();
        }
        if(_closeButton == null){
            _closeButton = transform.Find("CloseButton").GetComponent<Button>();
        }
        _replayButton.onClick.AddListener(OnReplayButtonClick);
        _continueLearningButton.onClick.AddListener(OnContinueLearningButtonClick);
        _homeButton.onClick.AddListener(OnHomeButtonClick);
        _closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnReplayButtonClick()
    {
        LessonManager.Instance.StartLesson(LessonManager.Instance.GetCurrentLesson().lessonId);
        gameObject.SetActive(false);
    }
    private void OnContinueLearningButtonClick()
    {
        LessonManager.Instance.StartLesson(LessonManager.Instance.GetCurrentLesson().lessonId + 1);
        gameObject.SetActive(false);
    }
    private void OnHomeButtonClick()
    {
        SceneManager.Instance.LoadSceneWithLoadingScene("MainMenuScene");
        gameObject.SetActive(false);
    }
    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }
}
