using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class Dialogue : GameEvent
    {
        string IconImage;
        string Text;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "Dialogue";

            this.IconImage = "asist_icon";
            if (node["IconImage"] != null)
                this.IconImage = node["IconImage"].Value;

            this.Text = "MissionPangramm";
            if (node["text"] != null)
                this.Text = node["text"].Value;

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIParameters.SetDialogue(this.IconImage, this.Text, this);
            UIM.OpenMenu("DialogueMenu");
        }

        #region static
        public static Dialogue Create(string text, string icoImage)
        {
            Dialogue temp = new Dialogue();
            temp.ID = "Dialogue";

            temp.Text = text;
            temp.IconImage = icoImage;

            return temp;
        }
        #endregion
    }
}