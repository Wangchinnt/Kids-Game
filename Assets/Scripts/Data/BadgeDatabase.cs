using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class BadgeDatabase
{
    public static List<BadgeDefinition> BadgeDefinitions;
    static BadgeDatabase()
    {
        BadgeDefinitions = new List<BadgeDefinition>();
    }
    public static void LoadBadgeDefinitions()
    {
        string json = Resources.Load<TextAsset>("badges").text;
        BadgeDefinitionList wrapper = JsonUtility.FromJson<BadgeDefinitionList>(json);
        BadgeDefinitions = wrapper.badges;
    }
    public static BadgeDefinition GetBadgeDefinition(string badgeID)
    {
        return BadgeDefinitions.FirstOrDefault(x => x.BadgeID == badgeID);
    }
    
}
