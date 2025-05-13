using System;
using UnityEngine;
using SQLite4Unity3d;

    [Serializable]
    public class ActivityLog
    {
        [PrimaryKey]
        public string LogID { get; set; }
        
        public string StudentID { get; set; }
        public string ActivityName { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public int CorrectProblems { get; set; }
        public int TotalProblems { get; set; }
        public float TimeTaken { get; set; }
        public string ActivityType { get; set; }
        public int SyncStatus { get; set; }
    }
