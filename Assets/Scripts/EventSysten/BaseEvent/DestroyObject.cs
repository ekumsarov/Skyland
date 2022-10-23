using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class DestroyObject : GameEvent
    {
        string ObjectName;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "DestroyObject";

            if (node["ID"] != null)
                ObjectName = node["ID"].Value;

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (!GM.Uniqs.ContainsKey(ObjectName))
            {
                Debug.LogError("No Such Object In Mission Folder");
                return;
            }


            SkyObject temp = GM.Uniqs[ObjectName];
            temp.gameObject.SetActive(false);
            End();
        }

        #region static
        public static DestroyObject Create(string _id)
        {
            DestroyObject temp = new DestroyObject();
            temp.ID = "DestroyObject";

            temp.ObjectName = _id;

            return temp;
        }
        #endregion
    }
}