using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class IslandState : GameEvent
    {
        Island obj;

        Island.iState state;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "IslandState";

            if (node["ID"] != null)
                obj = GM.GetObject(node["ID"].Value).GetComponent<Island>();

            state = Island.iState.Active;
            if (node["state"] != null)
                state = (Island.iState)Enum.Parse(typeof(Island.iState), node["state"].Value);

            if (obj == null)
                Debug.LogError("NOT SET CAMERA TARGET");
        }

        public override bool CanActive()
        {
            if (obj == null)
                return false;

            return true;
        }

        public override void Start()
        {
            obj.State = state;
            End();
        }
    }
}
