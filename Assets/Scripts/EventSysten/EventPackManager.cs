using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using SimpleJSON;
using UnityEngine;
using System;
using EventPackSystem;
using Lodkod;

public class EventPackManager
{
    private static Dictionary<string, Type> assets;
    public Dictionary<string, EventPack> packs;
    private Dictionary<string, List<EventPack>> editorPacks;
    private Dictionary<string, List<EventPack>> editorPacksOnlyPush;
    private Dictionary<string, Dictionary<string, EventPack>> editorPacksDelayed;

    public void initAllPacks()
    {
        assets = new Dictionary<string, Type>();
        packs = new Dictionary<string, EventPack>();
        editorPacks = new Dictionary<string, List<EventPack>>();
        editorPacksDelayed = new Dictionary<string, Dictionary<string, EventPack>>();

        getAllAssets();

        EventPack res = null;

        foreach (var key in assets.Keys)
        {
            try
            {
                res = Activator.CreateInstance(assets[key]) as EventPack;
                res.ID = key;
                res.Create();
                packs.Add(key, res);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading script " + key);
                Debug.LogError(e.Message);
            }
        }

        /*DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/missions/mission1/events/");


        FileInfo[] fileInfo = dir.GetFiles("*.cs*", SearchOption.AllDirectories);

        foreach (var file in fileInfo)
        {
            if (file.Name.Contains(".meta"))
                continue;

            string eventPackName = file.Name.Remove(file.Name.LastIndexOf("."));
            Debug.Log(eventPackName);
            Debug.LogError(Type.GetType("EventPackSystem."+eventPackName));

            
        }

        //List<string> res = new List<string>();*/
    }

    public EventPack loadEvent(string id)
    {
        EventPack  res = null;
        if(assets == null)
        {
            getAllAssets();
        }

        if (packs.ContainsKey(id))
        {
            res = packs[id];
            return res;
            
        }

        if(assets.ContainsKey(id))
        {
            try
            {
                res = Activator.CreateInstance(assets[id]) as EventPack;
                res.ID = id;
                res.Create();
                packs.Add(id, res);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading script " + id);
                Debug.LogError(e.Message);
            }
        }

        if(packs.ContainsKey(id))
            res = packs[id];

        if (res == null)
        {
            Debug.LogError("Failed to load event! ID: " + id);
        }

        return res;
    }

    public EventPack DelaydPacks(string objID, string id)
    {
        EventPack res = null;
        if (this.editorPacksDelayed.ContainsKey(objID) && this.editorPacksDelayed[objID].ContainsKey(id))
            res = this.editorPacksDelayed[objID][id];

        return res;
    }

    public void LoadAllEditorPacks()
    {
        foreach(var gameObject in this.editorPacks)
        {
            SkyObject temp = GM.GetObject(gameObject.Key);

            if (temp == null)
            {
                if (!this.editorPacksDelayed.ContainsKey(gameObject.Key))
                    this.editorPacksDelayed.Add(gameObject.Key, new Dictionary<string, EventPack>());

                foreach (var packi in gameObject.Value)
                {
                    this.editorPacksDelayed[gameObject.Key].Add(packi.ID, packi);
                }
                continue;
            }
            else if (temp.ID.Equals("Manager") || temp.ID.Equals("manager"))
                continue;

            foreach(var packi in gameObject.Value)
            {
                temp.Activity.pushPack(packi);
                temp.Activity.callActivityPack(packi.ID, true);
            }
        }

        foreach (var gameObject in this.editorPacksOnlyPush)
        {
            SkyObject temp = GM.GetObject(gameObject.Key);

            if (temp == null)
            {
                if (!this.editorPacksDelayed.ContainsKey(gameObject.Key))
                    this.editorPacksDelayed.Add(gameObject.Key, new Dictionary<string, EventPack>());

                foreach (var packi in gameObject.Value)
                {
                    this.editorPacksDelayed[gameObject.Key].Add(packi.ID, packi);
                }
                continue;
            }
            else if (temp.ID.Equals("Manager") || temp.ID.Equals("manager"))
                continue;

            foreach (var packi in gameObject.Value)
            {
                temp.Activity.pushPack(packi);
            }
        }
    }

    void getAllAssets()
    {
        Assembly[] asset = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();
        foreach (var a in asset)
            foreach (var t in a.GetExportedTypes())
                if (t.IsSubclassOf(typeof(EventPackSystem.EventPack)))
                {
                    string typeName = t.ToString();
                    string shortName;
                    if (typeName.IndexOf("EventPackSystem.") == 0)
                    {
                        shortName = typeName.Substring("EventPackSystem.".Length);
                        Debug.Log(shortName);
                        assets[shortName] = t;
                    }
                    else if(typeName.IndexOf("EventPackSystem" + GM.mission + ".") == 0)
                    {
                        shortName = typeName.Substring(string.Concat("EventPackSystem.", GM.mission).Length);
                        Debug.Log(shortName);
                        assets[shortName] = t;
                    }
                    else if (typeName.IndexOf("EventPackSystemAi.") == 0)
                    {
                        shortName = typeName.Substring("EventPackSystemAi.".Length);
                        Debug.Log(shortName);
                        assets[shortName] = t;
                    }
                }

        TextAsset[] allTexts = Resources.LoadAll<TextAsset>("missions/mission" + GM.mission + "/EditorQuest");
        for(int i = 0; i < allTexts.Length; i++)
        {
            if (allTexts[i].text.IsNullOrEmpty())
                continue;

            EventPack events = new EventPack();
            events.Create();
            
            JSONNode Data = JSON.Parse(allTexts[i].text);

            string objectBame = Data["Data"]["ObjectID"].Value;
            if (!this.editorPacks.ContainsKey(objectBame))
                this.editorPacks.Add(objectBame, new List<EventPack>());

            if(Data["SetupNode"] != null)
            {
                JSONNode iconData = Data["SetupNode"]["NodeData"];

                SceneObject parent = null;
                if (!iconData["ObjectID"].Value.IsNullOrEmpty())
                    parent = GM.GetObject(iconData["ObjectID"].Value) as SceneObject;

                IconObject.Create(iconData["ID"].Value,
                    iconData["IconID"].Value,
                    (IconInteractType)Enum.Parse(typeof(IconInteractType), iconData["Type"]),
                    (IconInteractType)Enum.Parse(typeof(IconInteractType), iconData["Layout"]),
                    parent);

                if(iconData["MainEvent"] != null)
                {
                    IconObject tempIcon = GM.GetIcon(iconData["ID"].Value);
                    tempIcon.MainEvent = iconData["MainEvent"].Value;
                    if (iconData["IsActive"] != null)
                        tempIcon.Visible = iconData["IsActive"].AsBool;
                }
            }

            JSONArray array = Data["Nodes"].AsArray;
            for(int j = 0; j < array.Count; j++)
            {
                if(array[j]["Node"]["NodeType"].Value.Equals("Pack"))
                {
                    if(this.editorPacksOnlyPush == null)
                    {
                        this.editorPacksOnlyPush = new Dictionary<string, List<EventPack>>();
                        this.editorPacksOnlyPush.Add(objectBame, new List<EventPack>());
                    }

                    if(!this.editorPacksOnlyPush.ContainsKey(objectBame))
                        this.editorPacksOnlyPush.Add(objectBame, new List<EventPack>());

                    JSONArray packArray = array[j]["NodeData"]["Events"].AsArray;
                    EventPack packEvent = new EventPack();
                    packEvent.Create();
                    packEvent.ID = array[j]["NodeData"]["ActionID"].Value;
                    for (int k = 0; k < packArray.Count; k++)
                    {
                        packEvent.events.Add(packArray[k]);
                    }
                    this.editorPacksOnlyPush[objectBame].Add(packEvent);
                }
                else if(!array[j]["Node"]["NodeType"].Value.Equals("Event") && !array[j]["Node"]["NodeType"].Value.Equals("Pack"))
                    events.events.Add(array[j]["NodeData"]);
            }

            events.ID = allTexts[i].name;

            this.editorPacks[objectBame].Add(events);
        }
    }

}