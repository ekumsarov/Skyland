using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class SafeCallFunction : GameEvent
    {
        string function;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SafeCallFunction";

            function = string.Empty;
            if (node["Function"] != null)
                function = node["Function"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            return function.IsNullOrEmpty()? false : base.CanActive();
        }

        public override void Start()
        {

            SkyObject temp = GetObject(To);
            if(temp == null)
            {
                Debug.LogError("Can't find object ID: " + To);
                End();
                return;
            }

            temp.SafeCall(function, End);
        }

        #region static
        public static SafeCallFunction Create(string function, string to)
        {
            SafeCallFunction temp = new SafeCallFunction();
            temp.ID = "SafeCallFunction";
            temp.To = to;
            temp.function = function;

            return temp;
        }
        #endregion
    }
}