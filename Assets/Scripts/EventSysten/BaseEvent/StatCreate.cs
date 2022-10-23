using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class StatCreate : GameEvent
    {
        List<StatTemplate> templates;
        bool canActive;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "StatCreate";
            this.canActive = true;

            if (node["ID"] != null)
            {
                this.templates = new List<StatTemplate>();
                if(node["ID"].Value.Equals("list"))
                {
                    JSONArray arr = node["ID"].AsArray;
                    for(int i = 0; i < arr.Count; i++)
                    {
                        this.templates.Add(StatTemplate.Create(arr[i]));
                    }
                }
                else
                {
                    this.templates.Add(StatTemplate.Create(node));
                }
            }
            else
                this.canActive = false;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);

        }

        public override bool CanActive()
        {
            return this.canActive == false? false : base.CanActive();
        }

        public override void Start()
        {
            foreach(var stat in this.templates)
                Stat.Create(stat.ObjectID, stat.represent, stat._icon, stat.mainStat, stat._curVal, stat._maxVal, stat.negative, stat.intFlag, stat._isProd);

            End();
        }


        #region static
        public static StatCreate Create(string ID, Represent.Type type, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true, bool isProd = false, List<StatTemplate> templat = null)
        {
            StatCreate temp = new StatCreate();
            temp.ID = "StatCreate";
            temp.templates = new List<StatTemplate>();

            if (templat == null)
            {
                StatTemplate stat = new StatTemplate();
                stat.ObjectID = ID;
                stat.represent = type;
                stat._icon = icon;
                stat.mainStat = mainStat;
                stat._curVal = curVal;
                stat._maxVal = maxVal;
                stat.negative = neg;
                stat.intFlag = intF;
                stat._isProd = isProd;
            }
            else
                temp.templates.AddRange(templat);


            return temp;
        }
        #endregion
    }
}