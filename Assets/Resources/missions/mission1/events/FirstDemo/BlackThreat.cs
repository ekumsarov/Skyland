using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class BlackThreat : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "BlackThreat";

            Simple = false;

            subscriber = Subscriber.Create(this);

            QS.AddQuest(MainQuest.Create("BlackThreat").SetDescription("BlackThreatDescription"));
            ExpiredDay.ExpiredAfterDay(DayInfo.Create(15, DayPart.Evening), act: LoseGame);

            initialized = false;
        }

        public void Play()
        {
            if (!initialized)
            {
                initialized = true;
                End();
                return;
            }

            End();
        }

        public void LoseGame()
        {
            UIM.HideAllMenu();

            UIM.FastFade();

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.ButtonClick, TooltipFillMode.Instantly, TooltipObject.UI, "YouLoose");
        }
    }
}