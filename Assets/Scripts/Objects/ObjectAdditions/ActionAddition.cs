using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Lodkod;



public enum ActionsSet
{
    baement,
    dialogue,
    quest,
    Context,
    build,
    Map,
    SkillCheck
}

public class ActionAddition
{
    protected Dictionary<string, Actions> events;
    public SkyObject parent;

    public ActionAddition()
    {
        if (parent != null && parent.ID.Equals("GameEventManager"))
            Debug.LogError("Init Quest Events");

        events = new Dictionary<string, Actions>();
    }

    public void AddAction(string ID, string text, List<ActionButtonInfo> list, bool OnClear = true, bool restore = false)
    {
        if (parent != null && parent.ID.Equals("GameEventManager"))
            Debug.LogError("Add Action");

        Actions act = new Actions();
        act.ID = ID;
        act.Text = text;
        act.OnClear = OnClear;
        act.restore = restore;

        act.list = new List<ActionButtonInfo>();
        act.list.AddRange(list);
        this.AddAction(act); 
    }

    public virtual void CallAction(string action)
    {

        if (!this.events.ContainsKey(action))
        { Debug.LogError("There is no action ID: " + action); return; }

        this.events[action].CallAction(parent);
    }

    public virtual Actions GetAction(string action)
    {
        if(!this.events.ContainsKey(action))
        { Debug.LogError("There is no action ID: " + action); return null; }

        return this.events[action];
    }

    public virtual List<ActionButtonInfo> GetActionList(string action)
    {

        if (!this.events.ContainsKey(action))
        { Debug.LogError("There is no action ID: " + action); return null; }

        return this.events[action].GetActionList();
    }

    public virtual void SetupActionParameters(string action)
    {

        if (!this.events.ContainsKey(action))
        { Debug.LogError("There is no action ID: " + action); return; }

        this.events[action].SetupActionParameters();
    }

    public void AddAction(Actions act)
    {

        if (this.events.ContainsKey(act.ID))
        { Debug.LogError("Action ID already exist: " + act.ID); return; }

        act.parent = this;
        this.events.Add(act.ID, act);
    }
    
    public void ReplaceAction(Actions act)
    {

        this.events.Remove(act.ID);
        act.parent = this;
        this.events.Add(act.ID, act);
    }

    public void RemoveAction(string ID)
    {

        this.events.Remove(ID);
    }

    public void RepmoveAllActionChoice(string ID)
    {

        this.events[ID].list.Clear();
    }

    public void ChangeActionText(string action, string txt)
    {

        this.events[action].Text = txt;
    }

    public void AddActionChoice(string act, ActionButtonInfo info)
    {

        if (!this.events.ContainsKey(act))
        { Debug.LogError("Action not exist: " + act); return; }

        bool found = false;
        foreach(var choice in this.events[act].list)
        {
            if(choice.Compare(info))
            {
                found = true;
                break;
            }
        }
        
        if (found)
        { Debug.LogError("Choice already exist: " + info.ID); return; }
        else
        {
            if(info.ID == "CloseDialogeu")
                this.events[act].list.Insert(this.events[act].list.Count, info);
            else
                this.events[act].list.Insert(0, info);
        }
            
    }

    public void ReplaceActionChoice(string act, string actID, ActionButtonInfo info)
    {

        if (!this.events.ContainsKey(act))
        { Debug.LogError("Action not exist: " + act); return; }

        ActionButtonInfo inf = null;
        foreach (var choice in this.events[act].list)
        {
            if (choice.ID == actID)
            {
                inf = choice;
                break;
            }
        }

        if(inf == null)
        {
            Debug.LogError("No ActID: " + actID);
            return;
        }

        int index = this.events[act].list.IndexOf(inf);
        if (inf != null)
            this.events[act].list.Remove(inf);

        this.events[act].list.Insert(index, info);
    }

    public void RemoveActionChoice(string act, string actID)
    {

        if (!this.events.ContainsKey(act))
        { Debug.LogError("Action not exist: " + act); return; }

        ActionButtonInfo inf = null;
        foreach (var choice in this.events[act].list)
        {
            if (choice.ID == actID)
            {
                inf = choice;
                break;
            }
        }

        if (inf == null)
        {
            Debug.LogError("No ActID: " + actID);
            return;
        }

        if (inf != null)
            this.events[act].list.Remove(inf);
    }
}

public class Actions : ObjectID
{
    string ObjectID;
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    public ActionAddition parent;

    public string Text;
    public List<ActionButtonInfo> list;

    public bool OnClear;
    public bool restore;

    public ActionsSet type;

    public Actions()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.baement;
        OnClear = true;
        restore = true;
        Text = "AIPangramm";
    }

    public static Actions Create(string id, bool onClear = true, bool _restore = true, string text = "MissionPangramm")
    {
        Actions temp = new Actions
        {
            ID = id,
            OnClear = onClear,
            restore = _restore
        };
        temp.Text = text;

        return temp;
    }

    public static Actions Get(string type, string ID = "")
    {
        Actions temp;

        if (type.Equals("dialogue"))
            temp = new DialogueAction();
        else if (type.Equals("quest"))
            temp = new QuestAction();
        else if (type.Equals("Context"))
            temp = new ContextAction();
        else if (type.Equals("build"))
            temp = new BuildAction();
        else if (type.Equals("Map"))
            temp = new MapAction();
        else
            temp = new Actions();

        if (!ID.Equals(""))
            temp.ID = ID;

        return temp;
    }

    public virtual void CallAction(SkyObject parent)
    {
        UIParameters.SetAction(list, parent, text: Text);
        UIM.OpenMenu("ActionMenu");
    }

    public virtual void SetupActionParameters()
    {
        UIParameters.SetAction(list, parent.parent, text: Text);
        UIM.OpenMenu("ActionMenu");
    }

    public virtual List<ActionButtonInfo> GetActionList()
    {
        return list;
    }

    public SkyObject Parent
    {
        get { return parent.parent; }
    }

    #region Fake action builder

    public static Actions Create(string type, string ID)
    {
        Actions temp;

        if (type.Equals("dialogue"))
            temp = new DialogueAction();
        else if (type.Equals("quest"))
            temp = new QuestAction();
        else if (type.Equals("Context"))
            temp = new ContextAction();
        else if (type.Equals("build"))
            temp = new BuildAction();
        else if (type.Equals("Map"))
            temp = new MapAction();
        else
            temp = new Actions();

        if (!ID.Equals(""))
        {
            temp.ID = ID;
            temp.Text = ID;
        }
            

        return temp;
    }

    public Actions SetText(string text)
    {
        this.Text = text;
        return this;
    }

    public Actions AddChoice(ActionButtonInfo but)
    {
        this.list.Add(but);
        return this;
    }

    #endregion

    #region EventBuilder

    public class ActionsEventBuilder
    {
        Actions data;

        public static ActionsEventBuilder Create(string type, string id)
        {
            ActionsEventBuilder temp = new ActionsEventBuilder();
            temp.data = Actions.Get(type, id);
            return temp;
        }

        public void SetText(string text)
        {
            this.data.SetText(text);
        }

        public void AddChoice(ActionButtonInfo choice)
        {
            this.data.AddChoice(choice);
        }
    }
    #endregion
}

