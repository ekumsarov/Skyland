using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Lodkod;

public enum ActionType
{
    NotSet,
    Event,
    Pack,
    Action,
    SkillCheck,
    Context,
    Special,
    Callback,
    Close
}

public class ActionButtonInfo : ObjectID
{
    protected string ObjectID;
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    public ActionChoiceType AdditionalType = ActionChoiceType.Simple;
    private int OrderIndex = 0;
    protected string _text = string.Empty;
    protected string _addText = string.Empty;
    protected Action Callback;
    protected ActionType Type = ActionType.NotSet;
    public ActionType CallType
    {
        get { return this.Type; }
    }
    protected string CallID;
    protected SimpleJSON.JSONNode Event;
    protected int SelectID;

    protected List<Condition> _condition;
    protected List<Condition> _appearCondition;



    /// <summary>
    /// Old date. Think about to remove
    /// </summary>
    protected SimpleJSON.JSONNode data;
    bool initData = false;
    public SimpleJSON.JSONNode Data
    {
        get
        {
            if (!initData)
                this.ParseInfo();

            return this.data;
        }
    }

    public string Text
    {
        get
        {
            return this._text;
        }
    }

    public string AdditionalText
    {
        get
        {
            return this._addText;
        }
    }

    public bool Avaliable()
    {
        if (this._appearCondition == null)
            return true;

        for(int i = 0; i < this._appearCondition.Count; i++)
        {
            if (this._appearCondition[i].Available == false)
                return false;
        }

        return true;
    }

    public bool CanCall()
    {
        if (this._condition == null)
            return true;

        for (int i = 0; i < this._condition.Count; i++)
        {
            if (this._condition[i].Available == false)
                return false;
        }

        return true;
    }

    public string CantCallText()
    {
        for (int i = 0; i < this._condition.Count; i++)
        {
            if (this._condition[i].Available == false)
                return this._condition[i].Text;
        }

        return string.Empty;
    }

    public ActionButtonInfo(string id)
    {
        ID = id;
        CallID = null;
        Type = ActionType.NotSet;
        data = null;
        _text = LocalizationManager.Get(id);
        _addText = string.Empty;

        _condition = new List<Condition>();
        _appearCondition = new List<Condition>();
    }

    public static ActionButtonInfo Create(string id)
    {
        return new ActionButtonInfo(id);
    }

    public string SetEvent
    {
        set
        {
            Event = SimpleJSON.JSON.Parse(value.Replace("'", "\""));
        }
    }

    public void ParseInfo()
    {
        initData = true;
        data = new SimpleJSON.JSONClass();
        data.Add("Text", Text);
        data.Add("CallID", CallID);
        data.Add("Event", Event);
        data.Add("Type", Type.ToString());
        data.Add("SelectID", SelectID.ToString());
        data.Add("ID", ID);
    }

    public bool Compare(ActionButtonInfo comparor)
    {
        if (comparor.Text != this.Text)
            return false;

        return true;
    }

    public ActionButtonInfo SetText(string txt)
    {
        this._text = LocalizationManager.Get(txt);
        return this;
    }

    public ActionButtonInfo SetAdditionalText(string txt)
    {
        this._addText = LocalizationManager.Get(txt);
        return this;
    }

    public ActionButtonInfo SetCallData(string action)
    {
        this.CallID = action;
        if (Type == ActionType.NotSet)
        {
            this.Type = ActionType.Action;
            return this;
        }
        else
        {
            if (Type == ActionType.Event)
            {
                Event = SimpleJSON.JSON.Parse(CallID.Replace("'", "\""));
            }

            return this;
        }
    }

    public ActionButtonInfo SetType(ActionType type)
    {
        if (this.Type == ActionType.Action && this.Type == type)
            return this;

        if (type == ActionType.SkillCheck)
            this.AdditionalType = ActionChoiceType.Skillcheck;

        this.Type = type;
        if (this.Type == ActionType.Event && this.CallID != null && this.CallID.Equals("") != false)
        {
            Event = SimpleJSON.JSON.Parse(CallID.Replace("'", "\""));
        }

        return this;
    }

    public ActionButtonInfo SetSelectID(int sID)
    {
        this.SelectID = sID;
        return this;
    }

    public ActionButtonInfo SetAdditionalChoiceType(ActionChoiceType type)
    {
        this.AdditionalType = type;
        return this;
    }

    public ActionButtonInfo SetCallback(Action callback)
    {
        this.Type = ActionType.Callback;
        this.Callback = callback;
        return this;
    }

    public ActionButtonInfo SetAppearCondition(Condition _con)
    {
        if (this._appearCondition == null)
            this._appearCondition = new List<Condition>();

        this._appearCondition.Add(_con);
        return this;
    }

    public ActionButtonInfo SetAvailableCondition(Condition _con)
    {
        if (this._condition == null)
            this._condition = new List<Condition>();

        if (_con.ConditionType == ConditionType.Stat)
            this.AdditionalType = ActionChoiceType.Resource;

        this._condition.Add(_con);
        return this;
    }

    public string GetCallID()
    {
        return this.CallID;
    }

    public void CallAction(SkyObject Object)
    {
        if (Type == ActionType.Pack)
        {
            if (Object != null && Object.CanCallPack(CallID))
                GEM.AddEventsInQueue(Object.Activity.getActivity(CallID), true);
            else
                GEM.Execute(GM.Pack(CallID));
        }
        else if (Type == ActionType.Event)
        {
            if (Object != null)
            {
                GEM.AddEventInQueue(Object.Activity.GetEvent(Event));
            }
            else
                Debug.LogError("Parent not set to call Event!");
        }
        else if (Type == ActionType.Action || Type == ActionType.Context || Type == ActionType.SkillCheck)
        {
            if (Object != null)
            {
                Actions temp = Object.GetAction(CallID);

                if(temp == null)
                {
                    return;
                }

                if (temp.type == ActionsSet.Context)
                {
                    List<ActionButtonInfo> list = Object.GetActionList(CallID);
                    if (list == null || list.Count == 0)
                        return;
                    else
                    {
                        UIParameters.NullAction();
                        UIParameters.SetAction(list, Object, text: temp.Text);
                    }
                        
                }
                else
                {
                    
                    Object.CallAction(CallID);
                }
            }
            else
                Debug.LogError("Can't call Action: " + CallID + ". Or parent not set!");
        }
        else if (Type == ActionType.Special)
        {
            if (Object != null)
                Object.SafeCall(CallID, -1);
            else
                Debug.LogError("Can't call Speial Action: " + CallID + ". Or parent not set!");
        }
        else if (Type == ActionType.Callback)
        {
            this.Callback?.Invoke();
        }
        else if (Type == ActionType.Close)
        {
            if (CallID != null)
                GEM.Execute(GM.Pack(CallID));
        }
    }

    public List<iStat> GetStatList()
    {
        List<iStat> stats = new List<iStat>();

        for(int i = 0; i < _condition.Count; i++)
        {
            if(_condition[i].ConditionType == ConditionType.Stat)
            {
                StatCondition con = _condition[i] as StatCondition;
                if(con != null)
                {
                    for(int j = 0; j < con.GetStats.Count; j++)
                    {
                        if(stats.Any(stat => stat.type.Equals(con.GetStats[j].type)))
                        {
                            stats.First(stat => stat.type.Equals(con.GetStats[j].type)).amount += con.GetStats[j].amount;
                        }
                        else
                        {
                            stats.Add(con.GetStats[j]);
                        }
                    }
                }
            }
        }

        return stats;
    }

    #region builder for evet
    public class ABIEventBuilder
    {
        ActionButtonInfo data;

        public static ABIEventBuilder Create(string id)
        {
            ABIEventBuilder temp = new ABIEventBuilder();
            temp.data = new ActionButtonInfo(id);
            return temp;
        }

        public void SetText(string text)
        {
            if(!text.IsNullOrEmpty())
                this.data._text = LocalizationManager.Get(text);
        }

        public void SetCallData(string action)
        {
            this.data.CallID = action;
            if (this.data.Type == ActionType.NotSet)
            {
                this.data.Type = ActionType.Action;
            }
            else if(this.data.Type == ActionType.Event)
            {
                this.data.Event = SimpleJSON.JSON.Parse(this.data.CallID.Replace("'", "\""));
            }
        }

        public void SetType(ActionType type)
        {
            if (this.data.Type == ActionType.Action && this.data.Type == type)
                return;

            this.data.Type = type;
            if (this.data.Type == ActionType.Event && this.data.CallID != null && this.data.CallID.Equals("") != false)
            {
                this.data.Event = SimpleJSON.JSON.Parse(this.data.CallID.Replace("'", "\""));
            }
        }

        public void SetSelectID(int sID)
        {
            this.data.SelectID = sID;
        }

        public void SetCallback(Action callback)
        {
            this.data.Type = ActionType.Callback;
            this.data.Callback = callback;
        }

        public void SetAppearCondition(Condition _con)
        {
            if (this.data._appearCondition == null)
                this.data._appearCondition = new List<Condition>();

            this.data._appearCondition.Add(_con);
        }

        public void SetAvailableCondition(Condition _con)
        {
            if (this.data._condition == null)
                this.data._condition = new List<Condition>();

            this.data._condition.Add(_con);
        }

        public ActionButtonInfo GetButton()
        {
            return this.data;
        }
    }
    #endregion
}

