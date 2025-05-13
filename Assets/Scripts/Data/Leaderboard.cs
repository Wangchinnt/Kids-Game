using System;
using UnityEngine;
using SQLite4Unity3d;

[Serializable]
public class Leaderboard
{
    [PrimaryKey]
    public string LeaderboardID { get; set; }
    
    public string StudentID { get; set; }
    public int Level { get; set; }
    public int Diamonds { get; set; }
    public float TotalLearningTime { get; set; }
    public int SyncStatus { get; set; }
} 