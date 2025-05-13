using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[System.Serializable]
public class BadgeDefinition
{
    public string BadgeID;
    public string Name;
    public string Description;
}

[System.Serializable]
public class BadgeDefinitionList
{
    public List<BadgeDefinition> badges;
}
