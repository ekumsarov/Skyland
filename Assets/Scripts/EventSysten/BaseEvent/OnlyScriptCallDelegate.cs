using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class OnlyScriptCallDelegate : GameEvent
    {
        Action<Action> callback;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "OnlyScriptCallDelegate";

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (callback == null)
                return false;

            return base.CanActive();
        }


        public override void Start()
        {
            callback?.Invoke(End);
        }


        #region static
        public static OnlyScriptCallDelegate Create(Action<Action> callback)
        {
            OnlyScriptCallDelegate temp = new OnlyScriptCallDelegate();
            temp.ID = "OnlyScriptCallDelegate";

            temp.callback = callback;

            return temp;
        }
        #endregion
    }
}