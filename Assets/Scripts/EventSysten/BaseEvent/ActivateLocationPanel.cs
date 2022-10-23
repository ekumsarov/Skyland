using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActivateLocationPanel : GameEvent
    {
        string objectID;
        bool activate;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActivateLocationPanel";

            this.objectID = "no";
            if (node["ID"] != null)
                this.objectID = node["ID"].Value;

            this.activate = true;
            if (node["Activate"] != null)
                this.activate = node["Activate"].AsBool;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            SceneObject temp = GetObject(this.objectID) as SceneObject;
            if (temp != null)
            {
                temp.LockLocation(this.activate);
            }

            End();
        }


        #region static
        public static ActivateLocationPanel Create(string ID, bool activate)
        {
            ActivateLocationPanel temp = new ActivateLocationPanel();
            temp.ID = "ActivateLocationPanel";
            temp.objectID = ID;
            temp.activate = activate;

            return temp;
        }
        #endregion
    }
}