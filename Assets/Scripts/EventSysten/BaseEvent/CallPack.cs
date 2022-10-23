using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CallPack : GameEvent
    {
        string EventsID;
        SkyObject parent;
        string To;
        bool fromObject;
        DayInfo day;
        int ticks;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CallPack";

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            fromObject = false;
            if (node["To"] != null)
            {
                fromObject = true;
                To = node["To"].Value;

            }

            day = null;
            if (node["DayDelay"] != null)
                day = DayInfo.Create(node["DayDelay"]["day"].AsInt, (DayPart)Enum.Parse(typeof(DayPart), node["DayDelay"]["part"].Value));

            ticks = -1;
            if (node["Ticks"] != null)
                ticks = node["Ticks"].AsInt;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            if (fromObject)
            {
                if(day!=null)
                {
                    ExpiredDay.ExpiredAfterDay(day, packID: EventsID, parent: To);
                    End();
                    return;
                }
                if(ticks > 0)
                {
                    ExpiredDay.ExpiredAfterTicks(ticks, packID: EventsID, parent: To);
                    End();
                    return;
                }

                parent = GetObject(To);
                parent.Activity.callActivityPack(EventsID);
            }
            else
                GEM.Execute(GM.Pack(EventsID), day, ticks);

            End();
        }

        #region static
        public static CallPack Create(string ActionID, string To = null, DayInfo day = null, int ticks = -1)
        {
            CallPack temp = new CallPack();
            temp.ID = "CallPack";

            temp.EventsID = ActionID;
            temp.day = day;
            temp.ticks = ticks;

            if (To == null)
                temp.fromObject = false;
            else
            {
                temp.fromObject = true;
                temp.To = To;
            }

            return temp;
        }
        #endregion
    }
}
