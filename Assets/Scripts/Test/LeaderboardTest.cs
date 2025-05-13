using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class LeaderboardTest
{
    private LeaderboardManager leaderboardManager;
    private ProgressManager progressManager;

    [SetUp]
    public void Setup()
    {
        leaderboardManager = new LeaderboardManager();
        progressManager = new ProgressManager();
    }

    [Test]
    public void CompleteLessons_ShouldUpdateLeaderboard()
    {
        // Arrange
        string studentId = "test_student";
        int initialRank = leaderboardManager.GetStudentRank(studentId);

        // Act
        progressManager.CompleteLesson(studentId, "lesson1", 100);
        progressManager.CompleteLesson(studentId, "lesson2", 90);
        leaderboardManager.UpdateLeaderboard();

        // Assert
        int newRank = leaderboardManager.GetStudentRank(studentId);
        Assert.Less(newRank, initialRank, "Xếp hạng không được cập nhật");
    }

    [Test]
    public void ViewProgress_ShouldShowCorrectData()
    {
        // Arrange
        string studentId = "test_student";
        progressManager.CompleteLesson(studentId, "lesson1", 100);

        // Act
        StudentProgress progress = progressManager.GetStudentProgress(studentId);

        // Assert
        Assert.IsNotNull(progress, "Không tìm thấy tiến độ học tập");
        Assert.AreEqual(100, progress.GetLessonScore("lesson1"), "Điểm số không chính xác");
    }
}