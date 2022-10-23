using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents
{
    public class ReactLock : GameEvent
    {

        string To;
        bool Lock;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ReactLock";

            this.To = "";
            if (node["to"] != null)
                this.To = node["to"].Value;

            this.Lock = true;
            if (node["lock"] != null)
                this.Lock = node["lock"].AsBool;
        }

        public override bool CanActive()
        {
            if (To.Equals("nill"))
                return false;

            return true;
        }

        public override void Start()
        {
            SceneObject par = GetObject(To) as SceneObject;

            if (par == null)
            {
                Debug.LogError("No scene object in ReactButSetup id: " + To);
                End();
                return;
            }

            par.LockLocation(this.Lock);

            End();
        }

        #region static
        public static ReactLock Create(string to = "Manager", bool _lock = true)
        {
            ReactLock temp = new ReactLock();
            temp.ID = "ReactLock";

            temp.To = to;
            temp.Lock = _lock;

            return temp;
        }
        #endregion
    }
}