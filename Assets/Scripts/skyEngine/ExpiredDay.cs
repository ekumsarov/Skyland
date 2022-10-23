using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using System;
using GameEvents;

public class ExpiredDay : DayInfo
{
    public Action act;
    public Action everyCall;
    public string even;
    public EventPackSystem.EventPack pack;
    public string parent;
    public string PackID;

    public ExpiredDay()
    {
        even = null;
        act = null;
        everyCall = null;
        pack = null;
        parent = null;
        PackID = null;
        Day = 0;
        DayPart = Lodkod.DayPart.Afternoon;
        DayTick = 0;
        _sub = Subscriber.Create(this);
    }

	public static void ExpiredAfterTicks(int ticks, bool visual = false, Action everyCall = null, Action act = null, string ev = null, EventPackSystem.EventPack pack = null, string parent = null, string packID = null)
    {
        ExpiredDay temp = new ExpiredDay();

        DayInfo dd = TM.GetExpieredDay(ticks);
        temp.Day = dd.Day;
        temp.DayPart = dd.DayPart;
        temp.DayTick = dd.DayTick;
        temp.act = act;
        temp.even = ev;
        temp.pack = pack;
        temp.parent = parent;
        temp.PackID = packID;
        temp.everyCall = everyCall;
        temp._sub.AddEvent(TriggerType.ProductTick.ToString());

        TM.AddThreat(temp, visual);
    }

    public static void ExpiredAfterDay(DayInfo day, bool visual = false, Action everyCall = null, Action act = null, string ev = null, EventPackSystem.EventPack pack = null, string parent = null, string packID = null)
    {
        ExpiredDay temp = new ExpiredDay();

        DayInfo dd = TM.GetExpieredDay(day);
        temp.Day = dd.Day;
        temp.DayPart = dd.DayPart;
        temp.DayTick = dd.DayTick;
        temp.act = act;
        temp.even = ev;
        temp.pack = pack;
        temp.parent = parent;
        temp.PackID = packID;
        temp.everyCall = everyCall;
        temp._sub.AddEvent(TriggerType.ProductTick.ToString());

        TM.AddThreat(temp, visual);
    }

    public void ProductTick()
    {
        if (everyCall != null)
            everyCall();

        if(TM.Expired(this))
        {
            if (even != null)
                GEM.Execute(even);

            if (pack != null)
                GEM.Execute(pack);

            if(act != null)
            {
                act();
                act = null;
            }

            if(PackID != null && parent != null)
            {
                if (parent.Equals("manager") || parent.Equals("Manager"))
                    GEM.Execute(PackID);
                else
                    GM.GetObject(parent).Actioned(PackID);
            }

            everyCall = null;
            _sub.RemoveEvent(TriggerType.ProductTick.ToString());
            TM.RemoveThreat(this);
        }
    }
}
