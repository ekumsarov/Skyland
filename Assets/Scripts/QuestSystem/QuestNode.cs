using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class QuestNode : ObjectID
{
    string ObjectID = "nil";
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    private string _title = string.Empty;
    public string Title
    {
        get { return this._title; }
    }

    private string _description = string.Empty;
    public string Description
    {
        get { return this._description; }
    }

    private bool _complete = false;
    public bool Complete
    {
        get { return this._complete; }
        set
        {
            this._complete = value;
            ES.NotifySubscribers("QuestCompleted", this.ID);
        }
    }

    public string QuestIcon = "quest";
    public string BindNode = string.Empty;
    public bool Visible = true;
    public bool HideOnComplete = false;
    public bool ShowCompleteMark = true;


    public static QuestNode Create(string ID)
    {
        return new QuestNode()
        {
            ID = ID,
            _title = ID
        };
    }

    public QuestNode Create(string ID, string text)
    {
        return new QuestNode()
        {
            ID = ID,
            _description = text
        };
    }

    public QuestNode SetTitle(string text)
    {
        this._title = text;
        return this;
    }

    public QuestNode SetIcon(string icon)
    {
        this.QuestIcon = icon;
        return this;
    }

    public QuestNode SetDescription(string text)
    {
        this._description = text;
        return this;
    }

    public QuestNode SetBindNode(string nodeID)
    {
        this.BindNode = nodeID;
        this.Visible = false;
        return this;
    }

    public QuestNode SetHideOnComplete(bool hide)
    {
        this.HideOnComplete = hide;
        return this;
    }

    public QuestNode SetMarkVisibility(bool mark)
    {
        this.ShowCompleteMark = mark;
        return this;
    }

    #region Builder for event

    public static QuestNode Create(string id, JSONNode data)
    {
        QuestNode temp = QuestNode.Create(id);

        if (data["Icon"] != null)
            temp.QuestIcon = data["Icon"].Value;

        if (data["Description"] != null)
            temp._description = data["Description"].Value;

        if (data["Title"] != null)
            temp._title = data["Title"].Value;

        if (data["BindNode"] != null)
        {
            temp.BindNode = data["BindNode"].Value;
            temp.Visible = false;
        }

        if (data["HideOnComplete"] != null)
            temp.HideOnComplete = data["HideOnComplete"].AsBool;

        if (data["SetMarkVisibility"] != null)
            temp.ShowCompleteMark = data["SetMarkVisibility"].AsBool;


        return temp;
    }
    #endregion
}
