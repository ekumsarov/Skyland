using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lodkod;
using GameEvents;
using EventPackSystem;
using SimpleJSON;

public class MainQuest : ObjectID
{
    string ObjectID = "nil";
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    public List<QuestNode> _nodes;
    private string _description;
    public string Description
    {
        get { return this._description; }
    }

    private string _title = string.Empty;
    public string Title
    {
        get { return this._title; }
    }

    public string MainQuestIcon = "quest";

    public static MainQuest Create(string ID)
    {
        return new MainQuest()
        {
            ID = ID,
            _title = ID,
            _nodes = new List<QuestNode>()
        };
    }

    public MainQuest SetDescription(string text)
    {
        this._description = text;
        return this;
    }

    public MainQuest SetTitle(string text)
    {
        this._title = text;
        return this;
    }

    public MainQuest SetIcon(string icon)
    {
        this.MainQuestIcon = icon;
        return this;
    }

    public MainQuest AddNode(QuestNode quest)
    {
        this._nodes.Add(quest);
        return this;
    }

    #region Builder for event

    public static MainQuest Create(string id, JSONNode data)
    {
        MainQuest temp = MainQuest.Create(id);

        if (data["Icon"] != null)
            temp.MainQuestIcon = data["Icon"].Value;

        if (data["Description"] != null)
            temp._description = data["Description"].Value;

        if (data["Title"] != null)
            temp._title = data["Title"].Value;

        if(data["Nodes"] != null)
        {
            JSONArray ar = data["Nodes"].AsArray;
            for(int i = 0; i < ar.Count; i++)
            {
                temp._nodes.Add(QuestNode.Create(ar[i]["Title"].Value, ar[i]));
            }
        }

        return temp;
    }
    #endregion
}
