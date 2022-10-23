using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class Wait : GameEvent
    {
        float second;


        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "Wait";

            second = 1f;
            if (node["second"] != null)
                second = node["second"].AsFloat;

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            CM.WaitController.Wait(second, this);
        }

        #region static
        public static Wait Create(float time = 1.0f)
        {
            Wait temp = new Wait();
            temp.ID = "Wait";
            temp.second = time;

            return temp;
        }
        #endregion
    }
}