using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MoveIcon : IconGameEvent
    {
        string MoveID;
        string ObjectID;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MoveIcon";

            this.MoveID = "Remove";
            if (node["To"] != null)
                this.MoveID = node["To"].Value;

            this.ObjectID = "self";
            if (node["ID"] != null)
                this.ObjectID = node["ID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            IconObject iconObject;

            if (this.ObjectID.IsNullOrEmpty() || this.ObjectID.Equals("self"))
                iconObject = this.IconObject;
            else
                iconObject = GM.GetIcon(this.ObjectID);

            if(this.MoveID.Equals("Remove"))
            {
                iconObject.RemoveIcon();
                iconObject.Visible = false;
            }
            else
            {
                SceneObject temp = GetObject(this.MoveID) as SceneObject;
                if(temp != null)
                    temp.AddIcon(iconObject);
            }

            End();
        }

        #region static

        public static MoveIcon Create(string iconID, string To = "remove")
        {
            MoveIcon temp = new MoveIcon();
            temp.ID = "MoveIcon";

            temp.MoveID = To;
            temp.ObjectID = iconID;

            return temp;
        }

        #endregion
    }
}