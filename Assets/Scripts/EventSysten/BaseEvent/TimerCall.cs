using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class TimerCall : GameEvent
    {
        string To;
        string eve;
        string pack;
        int ticks;
        int days;
        Action callback;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "TimerCall";

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            eve = null;
            if (node["event"] != null)
                eve = node["event"].Value;

            pack = null;
            if (node["pack"] != null)
                pack = node["pack"].Value;

            ticks = -1;
            if (node["ticks"] != null)
                ticks = node["ticks"].AsInt;

            days = -1;
            if (node["days"] != null)
                days = node["days"].AsInt;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (ticks != -1)
                ExpiredDay.ExpiredAfterTicks(ticks, act:callback, ev:eve, parent:To, packID:pack);
            else if (days != -1)
                ExpiredDay.ExpiredAfterDay(DayInfo.Create(days), act: callback, ev: eve, parent: To, packID: pack);

            End();
        }


        #region static
        public static TimerCall Create(string to, int days = -1, int ticks = -1, string eve = null, string packid = null, string _to = null, Action callback = null)
        {
            TimerCall temp = new TimerCall();
            temp.ID = "TimerCall";

            temp.To = to;
            temp.eve = eve;
            temp.pack = packid;
            temp.To = _to;
            temp.callback = callback;
            temp.days = days;
            temp.ticks = ticks;

            return temp;
        }
        #endregion
    }
}