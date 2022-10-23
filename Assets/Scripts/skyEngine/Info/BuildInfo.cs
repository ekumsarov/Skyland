using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;
using System;

public class BuildInfo
{
    public string Name;
    public string Icon;
    public string Prefab; 
    public string Description;
    public int HP;
    public int BuildTime;
    public int Level;
    public BuildType type;
    public List<iStat> Cost;
    public List<iStat> Consumtion;
    public JSONNode Special;

    public static BuildInfo Make(string name, int level, JSONNode data)
    {
        BuildInfo temp = new BuildInfo();
        
        if (data["Icon"] == null)
            Debug.LogError("Icon not foud in BuildInfo");
        else if (data["Prefab"] == null)
            Debug.LogError("Prefab not foud in BuildInfo");
        else if (data["Description"] == null)
            Debug.LogError("Description not foud in BuildInfo");
        else if (data["HP"] == null)
            Debug.LogError("HP not foud in BuildInfo");
        else if (data["BuildTime"] == null)
            Debug.LogError("BuildTime not foud in BuildInfo");
        else if (data["Cost"] == null)
            Debug.LogError("Cost not foud in BuildInfo");
        else if (data["Special"] == null)
            Debug.LogError("Special not foud in BuildInfo");

        temp.Name = name;
        temp.Icon = data["Icon"].Value;
        temp.Prefab = data["Prefab"].Value;
        temp.Description = data["Description"].Value;
        temp.HP = data["HP"].AsInt;
        temp.BuildTime = data["BuildTime"].AsInt;
        temp.Level = level;
        temp.type = (BuildType)Enum.Parse(typeof(BuildType), data["BuildType"].Value);
        temp.Cost = iStat.createResList(data["Cost"]);
        temp.Consumtion = iStat.createResList(data["Consumtion"]);
        temp.Special = data["Special"];

        return temp;
    }

    public void CopyInfo(BuildInfo info)
    {
        Debug.LogError("Cannot copy BuildInfo");
    }
}
