using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;

    public class LocalDataManager : MonoBehaviour
    {
        public static LocalDataManager Instance { get; private set; }
        private SQLiteConnection _db;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(Application.persistentDataPath, "kids_game.db");
            _db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            _db.CreateTable<Student>();
            _db.CreateTable<Badge>();
            _db.CreateTable<Toy>();
            _db.CreateTable<ActivityLog>();
            _db.CreateTable<LearningProgress>();
            _db.CreateTable<Leaderboard>();
            _db.CreateTable<NotificationPreferences>();
            // make json file for game settings
            string path = Path.Combine(Application.persistentDataPath, "gameSettings.json");
            if (!File.Exists(path))
            {
                File.Create(path);
            };
        }

        // --------- STUDENT ---------
        public void SaveStudent(Student student)
        {
            _db.InsertOrReplace(student);
        }

        public Student LoadStudent(string studentId)
        {
            return _db.Find<Student>(studentId);
        }

        public void DeleteStudent(string studentId)
        {
            _db.Delete<Student>(studentId);
        }

        // --------- NOTIFICATION PREFERENCES ---------
        public void SaveNotificationPreferences(NotificationPreferences preferences)
        {
            _db.InsertOrReplace(preferences);
        }

        public NotificationPreferences LoadNotificationPreferences(string studentId)
        {
            return _db.Find<NotificationPreferences>(studentId);
        }

        public void DeleteNotificationPreferences(string studentId)
        {
            _db.Delete<NotificationPreferences>(studentId);
        }

        // --------- BADGE ---------
        public void SaveBadge(Badge badge)
        {
            Debug.Log("SaveBadge: " + badge.BadgeID + " " + badge.Name + " " + badge.Description + " " + badge.ObtainedAt);
            _db.InsertOrReplace(badge);
        }

        public List<Badge> LoadBadgesByStudent(string studentId)
        {
            return _db.Table<Badge>().Where(b => b.StudentID == studentId).ToList();
        }

        // --------- TOY ---------
        public void SaveToy(Toy toy)
        {
            _db.InsertOrReplace(toy);
        }

        public List<Toy> LoadToysByStudent(string studentId)
        {
            return _db.Table<Toy>().Where(t => t.StudentID == studentId).ToList();
        }

        // --------- ACTIVITY LOG ---------
        public void SaveActivityLog(ActivityLog log)
        {
            _db.InsertOrReplace(log);
        }

        public List<ActivityLog> LoadActivityLogsByStudent(string studentId)
        {
            return _db.Table<ActivityLog>().Where(a => a.StudentID == studentId).ToList();
        }

        // --------- LEARNING PROGRESS ---------
        public void SaveLearningProgress(LearningProgress progress)
        {
            _db.InsertOrReplace(progress);
        }

        public LearningProgress LoadLearningProgress(string studentId)
        {
            return _db.Table<LearningProgress>().FirstOrDefault(p => p.StudentID == studentId);
        }

        // --------- LEADERBOARD ---------
        public void SaveLeaderboard(Leaderboard leaderboard)
        {
            _db.InsertOrReplace(leaderboard);
        }

        public Leaderboard LoadLeaderboard(string studentId)
        {
            return _db.Table<Leaderboard>().FirstOrDefault(l => l.StudentID == studentId);
        }

        // --------- GAME SETTINGS (JSON) ---------
        public void SaveGameSettings(GameSettings settings)
        {   
            string path = Path.Combine(Application.persistentDataPath, "gameSettings.json");
            string json = JsonUtility.ToJson(settings);
            File.WriteAllText(path, json);
        }

        public GameSettings LoadGameSettings()
        {
            string path = Path.Combine(Application.persistentDataPath, "gameSettings.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<GameSettings>(json);
            }
            return null;
        }

        // --------- SYNC METHODS (stub) ---------
        // Bạn có thể thêm các hàm sync dữ liệu từ remote về local tại đây
    }