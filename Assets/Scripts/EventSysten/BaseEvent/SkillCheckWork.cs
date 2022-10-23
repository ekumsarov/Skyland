using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using EventPackSystem;

namespace GameEvents
{
    public class SkillCheckWork : GameEvent
    {
        string ActID;
        string WorkType;
        string To;
        string ActOn;
        string Text;
        string FailText;

        List<string> LootHelp;

        List<SkillCheckObject> SuccessCheck;
        List<SkillCheckObject> BadCheck;

        ResultID result;

        SkyObject parent;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SkillCheckWork";

            this.ActID = "SkillCheck";
            if (node["ActionID"] != null)
                ActID = node["ActionID"].Value;

            WorkType = "Add";
            if (node["WorkType"] != null)
                WorkType = node["WorkType"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            ActOn = "self";
            if (node["ActOn"] != null)
                ActOn = node["ActOn"].Value;

            Text = "MissionPangramm";
            if (node["Text"] != null)
                Text = node["Text"].Value;

            FailText = "MissionPangramm";
            if (node["FailText"] != null)
                FailText = node["FailText"].Value;

            JSONArray arr = null;
            if (node["SuccessCheck"] != null)
                arr = node["SuccessCheck"].AsArray;
            if (arr != null)
            {
                SuccessCheck = new List<SkillCheckObject>();

                for (int i = 0; i < arr.Count; i++)
                    SuccessCheck.Add(SkillCheckObject.Create(arr[i]));
            }

            arr = null;
            if (node["BadCheck"] != null)
                arr = node["BadCheck"].AsArray;
            if (arr != null)
            {
                BadCheck = new List<SkillCheckObject>();

                for (int i = 0; i < arr.Count; i++)
                    BadCheck.Add(SkillCheckObject.Create(arr[i]));
            }

            LootHelp = null;
            arr = null;
            if (node["LootHelp"] != null)
                arr = node["LootHelp"].AsArray;
            if (arr != null)
            {
                LootHelp = new List<string>();

                for (int i = 0; i < arr.Count; i++)
                    LootHelp.Add(arr[i].Value);
            }

            result = ResultID.Create(node);
        }

        public override bool CanActive()
        {
            if (this.SuccessCheck == null)
                return false;

            return true;
        }

        public override void Start()
        {
            parent = GetObject(To);

            if (WorkType.Equals("Remove"))
                parent.RepmoveAction(ActID);
            else if (WorkType.Equals("ChangeText"))
            {
                parent.ChangeActionText(ActID, Text);
            }
            else
            {
                Actions act = SkillCheckAction.Make(Text, ActID, FailText, result, SuccessCheck,
                    LootHelp, ActOn, BadCheck);

                if (WorkType.Equals("Replace"))
                    parent.ReplaceAction(act);
                else
                    parent.AddAction(act);
            }

            End();
        }

        #region static
        public static SkillCheckWork Create(string ActID, string text, string fText, ResultID res,
        List<SkillCheckObject> successCheck, 
        List<string> lootHelp = null,
        string workType = "Add",
        string to = "self",
        string ActOn = "self",
        List<SkillCheckObject> badCheck = null)
        {
            SkillCheckWork temp = new SkillCheckWork();
            temp.ID = "SkillCheckWork";

            temp.ActID = ActID;
            temp.To = to;
            temp.WorkType = workType;
            temp.LootHelp = lootHelp;
            temp.Text = text;
            temp.FailText = fText;
            temp.SuccessCheck = successCheck;
            temp.BadCheck = badCheck;
            temp.result = res;

            return temp;
        }
        #endregion
    }
}