using System;
using UnityEngine;
using SQLite4Unity3d;


    [Serializable]
    public class Toy
    {
        [PrimaryKey]
        public string ToyID { get; set; }
        
        public string StudentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IsEquipped { get; set; }
        public DateTime ObtainedAt { get; set; }
        public int SyncStatus { get; set; }
    }
