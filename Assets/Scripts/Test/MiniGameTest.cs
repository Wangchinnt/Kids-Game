using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Tests
{
    public class MiniGameTest
    {
        private CountingGame countingGame;
        private LessonManager lessonManager;

        [SetUp]
        public void Setup()
        {
            countingGame = new CountingGame();
            lessonManager = new LessonManager();
        }

        [Test]
        public void PlayGame_AllCorrectAnswers_ShouldGetPerfectScore()
        {
            // Arrange
            int correctAnswers = 0;
            int totalQuestions = 7;

            // Act
            for (int i = 0; i < totalQuestions; i++)
            {
                countingGame.SetSelectedAnswer("correct_answer", null);
                if (countingGame.CheckWin())
                    correctAnswers++;
            }

            // Assert
            Assert.AreEqual(totalQuestions, correctAnswers, "Không đạt điểm tối đa");
            Assert.AreEqual("Hoàn hảo", lessonManager.GetScoreText(), "Không nhận được đánh giá Hoàn hảo");
        }

        [Test]
        public void PlayGame_FirstWrongAnswer_ShouldShowHint()
        {
            // Arrange
            bool hintShown = false;

            // Act
            countingGame.SetSelectedAnswer("wrong_answer", null);
            countingGame.CheckWin();

            // Assert
            Assert.IsTrue(hintShown, "Không hiển thị gợi ý sau lần sai đầu tiên");
        }

        [Test]
        public void PlayGame_SecondWrongAnswer_ShouldShowVideo()
        {
            // Arrange
            bool videoShown = false;

            // Act
            countingGame.SetSelectedAnswer("wrong_answer", null);
            countingGame.CheckWin();
            countingGame.SetSelectedAnswer("wrong_answer", null);
            countingGame.CheckWin();

            // Assert
            Assert.IsTrue(videoShown, "Không hiển thị video sau lần sai thứ hai");
        }
    }
} 