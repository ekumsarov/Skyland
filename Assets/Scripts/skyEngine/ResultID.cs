using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using Lodkod;
using UnityEngine;
using EventPackSystem;

public class ResultID 
{
    EventPack SuccessPack = null;
    EventPack FailPack = null;

    string SuccessID = "nil";
    string FailID = "nil";

    Action SuccessCallback = null;
    Action FailCallback = null;

    bool HasSuccess = false;
    bool HasFail = false;

    string FailGlobalEventID = "nil";

    public static ResultID Create()
    {
        return new ResultID();
    }

    public static ResultID Create(JSONNode node)
    {
        ResultID temp = new ResultID();

        if (node["SuccessID"] != null)
        {
            temp.SuccessID = node["SuccessID"].Value;
            temp.HasSuccess = true;
        }
            

        if (node["FailID"] != null)
        {
            temp.FailID = node["FailID"].Value;
            temp.HasFail = true;
        }
            

        return temp;
    }

    public ResultID SetSuccesPack(EventPack pack)
    {
        if(HasSuccess)
        {
            Debug.LogError("Result alredy has success to call");
            return this;
        }

        this.HasSuccess = true;
        this.SuccessPack = pack;
        return this;
    }

    public ResultID SetFailPack(EventPack pack)
    {
        if (HasFail)
        {
            Debug.LogError("Result alredy has fail to call");
            return this;
        }

        this.HasFail = true;
        this.FailPack = pack;
        return this;
    }

    ///////////////////
    public ResultID SetSuccesID(string pack)
    {
        if (HasSuccess)
        {
            Debug.LogError("Result alredy has success to call");
            return this;
        }

        this.HasSuccess = true;
        this.SuccessID = pack;
        return this;
    }

    public ResultID SetFailID(string pack)
    {
        if (HasFail)
        {
            Debug.LogError("Result alredy has fail to call");
            return this;
        }

        this.HasFail = true;
        this.FailID = pack;
        return this;
    }

    public ResultID SetFailGlobalEventID(string eventID)
    {
        if (HasFail)
        {
            Debug.LogError("Result alredy has fail to call");
            return this;
        }

        this.HasFail = true;
        this.FailGlobalEventID = eventID;
        return this;
    }

    ///////////////////
    public ResultID SetSuccessCallback(Action callback)
    {
        if (HasSuccess)
        {
            Debug.LogError("Result alredy has success to call");
            return this;
        }

        this.HasSuccess = true;
        this.SuccessCallback = callback;
        return this;
    }

    public ResultID SetFailCallback(Action callback)
    {
        if (HasFail)
        {
            Debug.LogError("Result alredy has fail to call");
            return this;
        }

        this.HasFail = true;
        this.FailCallback = callback;
        return this;
    }

    public void CallResult(SkyObject parent, bool success)
    {
        if(!HasSuccess)
        {
            throw new InvalidOperationException();
        }

        if (parent != null)
        {
            if (this.SuccessPack != null)
            {
                parent.Activity.pushPack(this.SuccessPack);
                this.SuccessID = this.SuccessPack.ID;
            }

            if (this.FailPack != null)
            {
                parent.Activity.pushPack(this.FailPack);
                this.FailID = this.FailPack.ID;
            }
        }

        if (success)
        {
            if (SuccessCallback != null)
                SuccessCallback.Invoke();
            else
                parent.Actioned(SuccessID);
        }
        else
        {
            if (!HasFail)
                Debug.LogError("Notice: result has no fail");

            if (FailCallback != null)
                FailCallback.Invoke();
            else if (FailGlobalEventID.IsNullOrEmpty())
                GEM.Execute(FailGlobalEventID);
            else
                parent.Actioned(FailID);
        }
    }
}
