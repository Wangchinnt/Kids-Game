using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;

public class FirebaseDataUploadTest : MonoBehaviour
{
    public string testUserId = "PeeDcyjsT7XM6TFvXWxwWBH0Obn1";

    public void Start()
    {
        UploadUserMockData();
    }

    public async void UploadUserMockData()
    {
        var db = FirebaseFirestore.DefaultInstance;
        var badges = new List<Dictionary<string, object>>();
        var toys = new List<Dictionary<string, object>>();
        var learningProgress = new Dictionary<string, object>();
        var activityLogs = new List<Dictionary<string, object>>();
        // add mock data for badges
        badges.Add(new Dictionary<string, object>
        {
            { "BadgeID", "FirstSteps" },
            { "Name", "First Steps" },
            { "Description", "Hoàn thành bài học toán đầu tiên." }
        });
        // another badge
        badges.Add(new Dictionary<string, object>
        {
            { "BadgeID", "DailyMathematician" },
            { "Name", "Daily Mathematician" },
            { "Description", "Học toán ít nhất 3 ngày liên tiếp." }
        });
        // add mock data for toys
        toys.Add(new Dictionary<string, object>
        {
            { "toy_id", "toy1" },
            { "name", "Toy 1" },
            { "description", "Toy 1 description" }
        });
        // another toy
        toys.Add(new Dictionary<string, object>
        {
            { "toy_id", "toy2" },
            { "name", "Toy 2" },
            { "description", "Toy 2 description" }
        });
        // another learning progress
        learningProgress.Add("chapter", "Chapte r 2");
        learningProgress.Add("lesson", "Lesson 2");
        learningProgress.Add("status", "Completed");
        learningProgress.Add("error_detail", "No errors");
        learningProgress.Add("sync_status", 1);
        // add mock data for activity logs
        activityLogs.Add(new Dictionary<string, object>
        {
            // activity log 1
            { "log_id", "log1" },
            { "student_id", testUserId },
            { "activity_name", "Activity 1" },
            { "date", DateTime.Now },
            { "level", 1 },
            { "correct_problems", 10 },
            { "total_problems", 10 },
            { "time_taken", 10 },
            { "activity_type", "Activity 1" },
            { "sync_status", 1 }
        });
        // another activity log
        activityLogs.Add(new Dictionary<string, object>
        {
            { "log_id", "log2" },
            { "student_id", testUserId },
            { "activity_name", "Activity 2" },
            { "date", DateTime.Now },
            { "level", 1 },
            { "correct_problems", 10 },
            { "total_problems", 10 },
            { "time_taken", 10 },
            { "activity_type", "Activity 2" },
            { "sync_status", 1 }
        });

        var userData = new Dictionary<string, object>
        {
            { "email", "testuser@email.com" },
            { "user_name", "testuser" },
            { "full_name", "Test User" },
            { "DOB", "2015-01-01" },
            { "phone", "0123456789" },
            { "address", "123 Test Street" },
            { "gender", "male" },
            { "role", 0 },
            { "createdAt", DateTime.Now },
            { "lastLogin", DateTime.Now },
            { "notification_preferences", new Dictionary<string, object>
                {
                    { "progress_updates", true },
                    { "achievements", true },
                    { "class_announcements", true }
                }
            },
            { "game_settings", new Dictionary<string, object>
                {
                    { "difficulty", "easy" },
                    { "sound_enabled", true },
                    { "music_enabled", true },
                    { "language", "vi" },
                    { "notifications_enabled", true }
                }
            },
            { "student_role", new Dictionary<string, object>
                {
                    { "parent_id", "" },
                    { "class_id", "" },
                    { "level", 1 },
                    { "exp_points", 0 },
                    { "diamonds", 0 },
                    { "total_learning_time", 0 },
                    { "total_practice_time", 0 },
                    { "badges", badges },
                    { "toys", toys },
                    { "learning_progress", learningProgress },
                    { "activity_logs", activityLogs }
                }
            }
        };

        await db.Collection("users").Document(testUserId).SetAsync(userData);

        Debug.Log("Test user data uploaded to Firestore for user: " + testUserId);
    }
}