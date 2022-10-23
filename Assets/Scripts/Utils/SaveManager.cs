using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using SimpleJSON;

public class SaveManager
{
    private static SaveManager instance = null;
    public static void NewGame()
    {
        if (SaveManager.instance != null)
            SaveManager.instance = null;

        SaveManager.instance = new SaveManager();
        SaveManager.instance._saveDate = new JSONClass();
    }

    JSONNode _saveDate;

    public static void AddSaveObject(object str, string key)
    {
        bool HasKey = false;

        foreach(var _key in SaveManager.instance._saveDate.Keys)
        {
            if(_key == key)
            {
                HasKey = true;
                break;
            }
        }

        if(!HasKey)
        {
            JSONArray ar = new JSONArray();
            SaveManager.instance._saveDate.Add(key, ar);
        }

        var node = new JSONClass();

        SaveManager.instance._saveDate[key].Add(JSON.Parse(JsonUtility.ToJson(str, true)));


    }

    public static void SaveFile()
    {
        File.WriteAllText(Application.dataPath + "/Resources/missions/Slots/startInfo.json", SaveManager.instance._saveDate.ToString());
    }
}
