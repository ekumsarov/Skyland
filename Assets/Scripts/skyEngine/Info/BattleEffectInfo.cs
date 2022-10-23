using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;

public class BattleEffectInfo
{
    private string _id;
    public string ID
    {
        get { return this._id; }
    }

    private string _icon;
    public string Icon
    {
        get { return this._icon; }
    }

    private string _description;
    public string Description
    {
        get { return this._description; }
    }

    private int _turns;
    public int Turns
    {
        get { return this._turns; }
    }

    public List<string> actToCancel;

    // Buy Info
    public JSONNode OtherData = null;

    public static BattleEffectInfo Make(string name, JSONNode data)
    {
        BattleEffectInfo temp = new BattleEffectInfo();

        temp._id = name;

        if (data["Icon"] != null)
            temp._icon = data["Icon"].Value;
        else
            temp._icon = "info"
;
        if (data["Description"] != null)
            temp._description = data["Description"].Value;
        else
            temp._description = "MissionPangramm";

        if (data["Turns"] != null)
            temp._turns = data["Turns"].AsInt;
        else
            temp._turns = -100;

        if (data["OtherData"] != null)
            temp.OtherData = data["OtherData"];
        else
            temp.OtherData = null;

        if (data["ActionsCancel"] != null)
        {
            temp.actToCancel = new List<string>();
            JSONArray arr = data["ActionsCancel"].AsArray;
            for (int i = 0; i < arr.Count; i++)
                temp.actToCancel.Add(arr[i].Value);
        }
        else
            temp.actToCancel = new List<string>();

        return temp;
    }

    public void CopyInfo(BuildInfo info)
    {
        Debug.LogError("Cannot copy BuildInfo");
    }
}