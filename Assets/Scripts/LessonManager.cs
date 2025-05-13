using UnityEngine;
using System;
using GameData;

public class LessonManager : MonoBehaviour
{
    public static LessonManager Instance { get; private set; }
    
    private LessonConfig _currentLesson;
    private int _currentQuestionIndex = 0;
    private bool _isLearningMode = false;
    
    private int _score = 0;
    private float _time = 0;
    private int _diamond = 0;
    private int _exp = 0;
    private bool _isTimerRunning = false;
    private bool _isCurrentLessonFinished = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartLesson(4, true);
    }

    public void StartLesson(int lessonId, bool isLearningMode = true)
    {
        _isLearningMode = isLearningMode;
        _currentQuestionIndex = 0;
        _score = 0;
        _time = 0;
        _diamond = 0;
        _exp = 0;
        LoadLessonConfig(lessonId);
        SceneManager.Instance.LoadSceneWithLoadingScene(_currentLesson.miniGame + "Scene");
    }

    public QuestionConfig GetCurrentQuestion()
    {
        if (_currentLesson == null || _currentQuestionIndex >= _currentLesson.questions.Length)
            return null;
            
        return _currentLesson.questions[_currentQuestionIndex];
    }

    public void NextQuestion()
    {
        if (!_isLearningMode) return;

        _currentQuestionIndex++;
        if (_currentQuestionIndex >= _currentLesson.numberOfQuestions)
        {
            _isCurrentLessonFinished = true;
            _currentQuestionIndex = 0;
        }
    }

    public int GetCurrentQuestionIndex()
    {
        return _currentQuestionIndex;
    }

    public int GetTotalQuestions()
    {
        return _currentLesson?.numberOfQuestions ?? 0;
    }

    public bool IsLearningMode()
    {
        return _isLearningMode;
    }
    public LessonConfig GetCurrentLesson()
    {
        return _currentLesson;
    }
    public bool IsCurrentLessonFinished()
    {
        return _isCurrentLessonFinished;
    }

    public void ProcessLessonResult() {
        _currentLesson.score = _score;
        _currentLesson.time = _time;
        // Save the lesson result to the database
    }

    public void UpdateScore(int score)
    {
        _score += score;
    }
    public void UpdateDiamond()
    {
        _diamond += _currentLesson.questions[_currentQuestionIndex].diamondWillEarn;
    }
    public void UpdateExp()
    {
        _exp += _currentLesson.expWillEarn;
    }
    public string GetCurrentScore()
    {
        return $"{_score}/{_currentLesson.numberOfQuestions}";
    }
    public string GetScoreText()
    {
        float percentage = (float)_score / _currentLesson.numberOfQuestions;
        
        if (percentage == 1f) // 7/7
            return "Hoàn hảo";
        else if (percentage >= 0.857f) // 6/7
            return "Xuất sắc";
        else if (percentage >= 0.571f) // 4-5/7
            return "Tốt";
        else if (percentage >= 0.429f) // 3/7
            return "Khá";
        else if (percentage >= 0.143f) // 1-2/7
            return "Trung bình";
        else // 0/7
        return "Yếu";
    }   
    public int GetCurrentDiamond()
    {
        return _diamond;
    }
    // Counting time in seconds
    public void StartTimer()
    {
        _isTimerRunning = true;
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            _time += Time.deltaTime;
        }
    }
    public string GetCurrentTime()
    {
        int minutes = Mathf.FloorToInt(_time / 60f);
        int seconds = Mathf.FloorToInt(_time % 60f);
        return $"{minutes:D2}:{seconds:D2}";
    }
    public void ResetLessonManager()
    {
        _currentQuestionIndex = 0;
        _score = 0;
        _time = 0;
        _diamond = 0;
        _exp = 0;
        _isTimerRunning = false;
        _isCurrentLessonFinished = false;
        _isLearningMode = false;
    }

    private void LoadLessonConfig(int lessonId)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("LessonConfigs/lessons");
        if (jsonFile != null)
        {
            LessonConfigList configList = JsonUtility.FromJson<LessonConfigList>(jsonFile.text);
            _currentLesson = System.Array.Find(configList.lessons, lesson => lesson.lessonId == lessonId);
        }
    }
    public void SaveLessonResult()
    {
        Student student = LocalDataManager.Instance.LoadStudent(AuthManager.Instance.GetCurrentUserId());
        student.ExpPoints += _exp;
        student.Diamonds += _diamond;
        student.TotalLearningTime += _time;
        LocalDataManager.Instance.SaveStudent(student);

        // Activity log
        ActivityLog activityLog = new ActivityLog();
        activityLog.LogID = AuthManager.Instance.GetCurrentUserId() + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        activityLog.StudentID = AuthManager.Instance.GetCurrentUserId();
        activityLog.ActivityType = "Học tập";
        activityLog.ActivityName = activityLog.ActivityType + "Chương" + _currentLesson.chapterId + "-" +_currentLesson.lessonName;
        activityLog.Date = DateTime.Now.ToString("MMM dd, yyyy 'vào lúc' HH:mm", 
                      new System.Globalization.CultureInfo("vi-VN"));
        activityLog.Description = _currentLesson.lessonDescription;
        activityLog.CorrectProblems = _score;
        activityLog.TotalProblems = _currentLesson.numberOfQuestions;
        activityLog.TimeTaken = _time;
      
        LocalDataManager.Instance.SaveActivityLog(activityLog);

        // Save learning progress
        LearningProgress learningProgress = new LearningProgress();
        learningProgress.ProgressID = AuthManager.Instance.GetCurrentUserId() + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        learningProgress.StudentID = AuthManager.Instance.GetCurrentUserId();
        learningProgress.Chapter =  _currentLesson.chapterName;
        learningProgress.Lesson = _currentLesson.lessonName;
        learningProgress.Status = 2;
        learningProgress.ErrorDetail = _currentLesson.errorDetail;
        LocalDataManager.Instance.SaveLearningProgress(learningProgress);

        // Log the lesson result
        Debug.Log("Lesson result saved");
    }
}