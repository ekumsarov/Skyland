using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class FadeScreen : GameEvent
    {

        Image FadeImage;
        bool fast = false;
        bool FadeIn = false;


        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "FadeScreen";

            if (node["fast"] != null)
                fast = node["fast"].AsBool;

            if (node["Fade"] != null)
            {
                if (node["Fade"].Value == "in")
                    FadeIn = true;
                else if (node["Fade"].Value == "out")
                    FadeIn = false;
            }

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (fast)
            {
                UIM.FastFade(FadeIn);
                End();
                return;
            }

            UIM.Fade(FadeIn, this);
        }

        public static FadeScreen Create(bool fadein, bool fast)
        {
            FadeScreen temp = new FadeScreen();

            temp.ID = "FadeScreen";
            temp.fast = fast;
            temp.FadeIn = fadein;

            return temp;
        }
    }
}