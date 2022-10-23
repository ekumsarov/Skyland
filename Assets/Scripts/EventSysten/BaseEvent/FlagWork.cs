using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class FlagWork : GameEvent
    {
        string flag;
        string ActType;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "FlagWork";

            ActType = "Add";
            if (node["WorkType"] != null)
                ActType = node["WorkType"].Value;

            flag = null;
            if (node["flag"] != null)
                flag = node["flag"].Value;

            if (node["FlagWorkID"] != null)
                flag = node["FlagWorkID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }
        
        public override void Start()
        {
            if(flag != null)
            {
                if (ActType.Equals("Add"))
                    SM.AddFlag(flag);
                else if (ActType.Equals("Remove"))
                    SM.RemoveFlag(flag);
                else if (ActType.Equals("On"))
                    SM.SetFlag(flag);
                else if (ActType.Equals("Off"))
                    SM.SetFlag(flag, false);
            }

            End();
        }


        #region static
        public static FlagWork Create(string flag, string actType)
        {
            FlagWork temp = new FlagWork();
            temp.ID = "FlagWork";

            temp.ActType = actType;
            temp.flag = flag;

            return temp;
        }
        #endregion
    }
}