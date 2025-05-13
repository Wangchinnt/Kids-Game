using System;
using UnityEngine;
using SQLite4Unity3d;

    [Serializable]
    public class NotificationPreferences
    {
        [PrimaryKey]
        public string StudentID { get; set; }
        
        public bool ProgressUpdates { get; set; } = true;
        public bool Achievements { get; set; } = true;
        public bool ClassAnnouncements { get; set; } = true;
        public int SyncStatus { get; set; }
    }