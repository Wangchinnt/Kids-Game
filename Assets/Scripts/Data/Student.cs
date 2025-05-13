using System;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;


    [Serializable]
    public class Student
    {
        [PrimaryKey]
        public string StudentID { get; set; }
        public string FullName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string ParentName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public int Level { get; set; } = 1;
        public int ExpPoints { get; set; } = 0;
        public int Diamonds { get; set; } = 0;
        public int Streak { get; set; } = 0;
        public float TotalLearningTime { get; set; } = 0;
        public float TotalPracticeTime { get; set; } = 0;
        public string DeviceID { get; set; }
        public int SyncStatus { get; set; }
    }
