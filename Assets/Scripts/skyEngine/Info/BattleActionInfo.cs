using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;



public class BattleActionInfo
{
    public string Name;
    public string Icon;
    public string Sound;
    public string Description;

    public JSONNode OtherData = null;

    public BAType BaseType;

    public static BattleActionInfo Make(string name, JSONNode data)
    {
        BattleActionInfo temp = new BattleActionInfo();
        

        temp.Name = name;

        temp.Icon = "info";
        if (data["Icon"]!=null)
            temp.Icon = data["Icon"].Value;

        temp.Sound = "info";
        if (data["Sound"] != null)
            temp.Sound = data["Sound"].Value;

        temp.Description = "info";
        if (data["Description"] != null)
            temp.Description = data["Description"].Value;

        if(data["OtherData"]!=null)
            temp.OtherData = data["OtherData"];

        return temp;
    }

    public void CopyInfo(BuildInfo info)
    {
        Debug.LogError("Cannot copy BuildInfo");
    }
}