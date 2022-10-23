using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lodkod;

[RequireComponent(typeof(UIItem))]
public class ActionChoiceController : UIController
{
    private UIItem _item;

    public IconText ShowedText;
    ActionChoiceData data;

    public override void SetupUI()
    {
        if (_item == null)
            this._item = GetComponent<UIItem>();

        this._item.ItemTag = "ActionChoice";
        this._item.Interactable = true;
        this._item._frameType = UIItem.FrameType.Selectable;
        this._item.Resize = UIeX.UIResize.ContentDependence;
    }

    public override void ApplyData(UIData parameters)
    {
        this.data = parameters as ActionChoiceData;
        this.ShowedText.Text(this.data.Text);
    }

    public override bool Visible
    {
        get { return this._item.Visible; }
        set
        {
            if (this.data.Avaliable())
                this._item.Visible = true;
        }
    }

    public override void Pressed()
    {
        
    }
}

public class ActionChoiceData : UIData
{
    private int OrderIndex = 0;
    private string _text;
    private Action Callback;
    private ActionType Type = ActionType.NotSet;
    private string CallID;
    private SimpleJSON.JSONNode Event;
    private int SelectID;

    List<Condition> _condition;
    List<Condition> _appearCondition;

    /// <summary>
    /// Old date. Think about to remove
    /// </summary>
    SimpleJSON.JSONNode data;
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

    public bool Avaliable()
    {
        if (this._appearCondition == null)
            return true;

        for (int i = 0; i < this._appearCondition.Count; i++)
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

    public ActionChoiceData(string id)
    {
        ID = id;
        CallID = null;
        Type = ActionType.NotSet;
        data = null;
        _text = LocalizationManager.Get(id);

        _condition = new List<Condition>();
        _appearCondition = new List<Condition>();
    }

    public static ActionChoiceData Create(string id)
    {
        return new ActionChoiceData(id);
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

    public bool Compare(ActionChoiceData comparor)
    {
        if (comparor.Text != this.Text)
            return false;

        return true;
    }

    public ActionChoiceData SetText(string txt)
    {
        this._text = LocalizationManager.Get(txt);
        return this;
    }

    public ActionChoiceData SetCallData(string action)
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

    public ActionChoiceData SetType(ActionType type)
    {
        if (this.Type == ActionType.Action && this.Type == type)
            return this;

        this.Type = type;
        if (this.Type == ActionType.Event && this.CallID != null && this.CallID.Equals("") != false)
        {
            Event = SimpleJSON.JSON.Parse(CallID.Replace("'", "\""));
        }

        return this;
    }

    public ActionChoiceData SetSelectID(int sID)
    {
        this.SelectID = sID;
        return this;
    }

    public ActionChoiceData SetCallback(Action callback)
    {
        this.Type = ActionType.Callback;
        this.Callback = callback;
        return this;
    }

    public ActionChoiceData SetAppearCondition(Condition _con)
    {
        if (this._appearCondition == null)
            this._appearCondition = new List<Condition>();

        this._appearCondition.Add(_con);
        return this;
    }

    public ActionChoiceData SetAvailableCondition(Condition _con)
    {
        if (this._condition == null)
            this._condition = new List<Condition>();

        this._condition.Add(_con);
        return this;
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
        else if (Type == ActionType.Action)
        {
            if (Object != null)
            {
                Actions temp = Object.GetAction(CallID);

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
                Object.SafeCall(CallID, SelectID);
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

    #region builder for evet
    public class ACDEventBuilder
    {
        ActionChoiceData data;

        public static ACDEventBuilder Create(string id)
        {
            ACDEventBuilder temp = new ACDEventBuilder();
            temp.data = new ActionChoiceData(id);
            return temp;
        }

        public void SetText(string text)
        {
            this.data._text = LocalizationManager.Get(text);
        }

        public void SetCallData(string action)
        {
            this.data.CallID = action;
            if (this.data.Type == ActionType.NotSet)
            {
                this.data.Type = ActionType.Action;
            }
            else if (this.data.Type == ActionType.Event)
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

        public ActionChoiceData GetButton()
        {
            return this.data;
        }
    }
    #endregion
}