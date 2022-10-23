using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class Context : GameEvent
    {
        string IconImage;

        List<ActionButtonInfo> rightButtons;

        string Text;

        bool restore;
        bool OnClear;
        SkyObject parent;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "Context";

            this.IconImage = "Base";
            if (node["IconImage"] != null)
                this.IconImage = node["IconImage"].Value;

            this.restore = false;
            if (node["restore"] != null)
                this.restore = node["restore"].AsBool;

            this.OnClear = false;
            if (node["OnClear"] != null)
                this.OnClear = node["OnClear"].AsBool;

            To = "";
            if (node["To"] != null)
                To = node["To"].Value;

            rightButtons = new List<ActionButtonInfo>();

            this.Text = null;
            if (node["Text"] != null)
            {
                this.Text = node["Text"].Value;
            }

            if (node["Actions"] != null)
            {
                JSONArray arr = node["Actions"].AsArray;
                for (int i = 0; i < arr.Count; i++)
                {
                    string aID = "action" + i;
                    if (arr[i]["Text"] != null)
                        aID = arr[i]["Text"].Value;

                    ActionButtonInfo.ABIEventBuilder button = ActionButtonInfo.ABIEventBuilder.Create(aID);
                    if (arr[i]["CallID"] != null)
                        button.SetCallData(arr[i]["CallID"].Value);

                    if (arr[i]["type"] != null)
                        button.SetType((ActionType)Enum.Parse(typeof(ActionType), arr[i]["type"].Value));

                    if (arr[i]["Condition"] != null)
                        button.SetAvailableCondition(MakeCondition(arr[i]["Condition"]));

                    rightButtons.Add(button.GetButton());
                }
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            if (parent == null)
                parent = GetObject(To);

            UIParameters.SetAction(rightButtons, parent, text: Text);
            UIM.OpenMenu("ContextMenu");
            End();
        }

        #region static
        public static Context Create(SkyObject par, List<ActionButtonInfo> buts, string text = null, string icoImage = null)
        {
            Context temp = new Context();
            temp.ID = "Context";

            temp.parent = par;
            temp.rightButtons = buts;
            temp.Text = text;
            temp.restore = false;
            temp.OnClear = false;

            return temp;
        }
        #endregion
    }
}