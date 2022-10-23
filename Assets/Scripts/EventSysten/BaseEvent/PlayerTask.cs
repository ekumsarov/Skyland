using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class PlayerTask : GameEvent
    {
        string To;
        Lodkod.PlayerTask Task;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "PlayerTask";

            To = null;
            if (node["To"] != null)
                To = node["To"].Value;

            Task = Lodkod.PlayerTask.GoToObject;
            if (node["Task"] != null)
                Task = (Lodkod.PlayerTask)Enum.Parse(typeof(Lodkod.PlayerTask), node["Task"].Value);

        }

        public override bool CanActive()
        {
            if (To == null)
                return false;

            return true;
        }

        public override void Start()
        {
            SceneObject temp = GetObject(To) as SceneObject;

            //if (temp != null)
            //    GM.Player.setPlayerTask(Task, temp);

            End();
        }
    }
}