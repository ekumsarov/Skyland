using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class LockObject : GameEvent
    {
        string objectID;
        bool activate;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "LockObject";

            this.objectID = "self";
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
                temp.Lock = this.activate;
            }

            End();
        }


        #region static
        public static LockObject Create(string ID, bool activate)
        {
            LockObject temp = new LockObject();
            temp.ID = "LockObject";
            temp.objectID = ID;
            temp.activate = activate;

            return temp;
        }
        #endregion
    }
}