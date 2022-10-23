using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActivateObject : GameEvent
    {
        bool activated;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActivateObject";

            activated = true;
            if (node["Activate"] != null)
                activated = node["Activate"].AsBool;

            To = "self";
            if (node["ID"] != null)
                To = node["ID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            SceneObject parent = GetObject(To) as SceneObject;
            if (parent == null)
            {
                Debug.LogError("Cannot find object ID: " + To);
                End();
                return;
            }

            parent.Visible = activated;
            End();
        }

        public static ActivateObject Create(string ID, bool active = true)
        {
            ActivateObject temp = new ActivateObject();

            temp.ID = "ActivateObject";
            temp.To = ID;
            temp.activated = active;

            return temp;
        }
    }
}