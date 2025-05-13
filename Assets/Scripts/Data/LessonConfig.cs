namespace GameData
{
    [System.Serializable]
    public class QuestionConfig
    {
        public int expectedValue;  // Giá trị cố định cho câu hỏi
        public bool isFixedValue;  // true nếu là giá trị cố định, false nếu random trong range
        public int diamondWillEarn;
    }

    [System.Serializable]
    public class LessonConfig
    {
        public int lessonId;
        public int chapterId;
        public string chapterName;
        public string lessonName;
        public string lessonDescription;
        public int[] valueRange;
        public int numberOfQuestions;
        public QuestionConfig[] questions;  // Thêm mảng questions
        public int[] fixedValues;  // Các giá trị cố định cho các câu hỏi đầu
        public int score;
        public double time;
        public string errorDetail;
        public int expWillEarn;
        public string videoSupportLink;
        public string miniGame;
    }

    [System.Serializable]
    public class LessonConfigList
    {
        public LessonConfig[] lessons;
    }
}