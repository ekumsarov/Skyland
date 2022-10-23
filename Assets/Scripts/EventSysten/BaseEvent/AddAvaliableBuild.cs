using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddAvaliableBuild : GameEvent
    {
        string MainID;
        string BuildID;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddAvaliableBuild";

            this.MainID = "no";
            if (node["MainID"] != null)
                this.MainID = node["MainID"].Value;

            this.BuildID = "no";
            if (node["BuildID"] != null)
                this.MainID = node["BuildID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (this.MainID.Equals("no"))
                return false;

            return base.CanActive();
        }

        public override void Start()
        {
            MainBuild temp = GetObject(this.MainID) as MainBuild;
            if(temp == null)
            {
                End();
                return;
            }

            temp.AddAvaliable(this.BuildID);

            End();
        }


        #region static
        public static AddAvaliableBuild Create(string mainID, string buildID)
        {
            AddAvaliableBuild temp = new AddAvaliableBuild();
            temp.ID = "AddAvaliableBuild";
            temp.MainID = mainID;
            temp.BuildID = buildID;

            return temp;
        }
        #endregion
    }
}