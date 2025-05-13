using System;
using UnityEngine;
using SQLite4Unity3d;


    [Serializable]
    public class LearningProgress
    {
        [PrimaryKey]
        public string ProgressID { get; set; }
        
        public string StudentID { get; set; }
        public string Chapter { get; set; }
        public string Lesson { get; set; }
        public int Status { get; set; } // 0: Not started, 1: In progress, 2: Completed
        public string ErrorDetail { get; set; }
        public int SyncStatus { get; set; }
    }
