using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ShowActionTooltip : GameEvent
    {
        string Text;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ShowActionTooltip";

            Text = string.Empty;
            if (node["Text"] != null)
                Text = node["Text"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            UIParameters.NullAction();
            UIParameters.SetAction(new List<ActionButtonInfo>() { ActionButtonInfo.Create("Continue").SetCallback(End) }, GEM.instance, text: Text);
            UIM.OpenMenu("DialogueMenu");
        }

        #region static
        public static ShowActionTooltip Create(string text)
        {
            ShowActionTooltip temp = new ShowActionTooltip();
            temp.ID = "ShowActionTooltip";
            temp.Text = text;

            return temp;
        }

        public static ShowActionTooltip CreateAddAction(string text)
        {
            ShowActionTooltip temp = new ShowActionTooltip();
            temp.ID = "ShowActionTooltip";

            temp.Text = text;

            return temp;
        }
        #endregion
    }
}