using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;

public class BattleUnitInfo
{
    public float HP;

    public string Name;
    public string Icon;
    public string Description;

    // Buy Info
    public List<iStat> Cost;
    public float BuildTime;
    public Dictionary<string, SkillObject> skills;

    public static BattleUnitInfo Make(string name, JSONNode data)
    {
        BattleUnitInfo temp = new BattleUnitInfo();

        temp.HP = data["HPAm"].AsFloat;

        temp.Name = name;
        temp.Icon = data["Icon"].Value;
        temp.Description = data["Description"].Value;
        temp.BuildTime = data["BuildTime"].AsFloat;

        temp.Cost = new List<iStat>();
        if(data["Cost"] != null)
            temp.Cost = iStat.createResList(data["Cost"]);

        temp.skills = new Dictionary<string, SkillObject>();
        JSONNode skill = data["Skills"];
        for (int i = 0; i < skill.Count; i++)
        {
            if(skill[i]["id"] == null)
            {
                Debug.LogError("Not set id for skill");
                continue;
            }

            temp.skills.Add(skill[i]["id"].Value, SkillObject.Make(skill[i]));
        }

        return temp;
    }

    public void CopyInfo(BuildInfo info)
    {
        Debug.LogError("Cannot copy BuildInfo");
    }
}
