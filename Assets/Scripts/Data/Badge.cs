using System;
using UnityEngine;
using SQLite4Unity3d;


    [Serializable]
    public class Badge
    {
        [PrimaryKey]
        public string BadgeID { get; set; }
        public string StudentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ObtainedAt { get; set; }
        public int SyncStatus { get; set; }
    }
