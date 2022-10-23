using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class SentryGuardsEvent : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "SentryGuardsEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            MapLocationIcon townIcon = GetObject("TownMapIcon") as MapLocationIcon;
            if(townIcon != null)
            {
                townIcon.AfterZoomPack = "NeedPass";
                townIcon.Activity.PushPack("NeedPass", new List<GameEvent>()
                {
                    ShowTooltip.Create(Vector3.zero, "SentryGuard", Text:"SentuGuardFirstMeet"),
                    MapIconAdditionalEvent.Create("TownMapIcon", null)
                });
            }

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

        public void CompleteQuest()
        {
            UIM.HideAllMenu();

            UIM.FastFade();

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.ButtonClick, TooltipFillMode.Instantly, TooltipObject.UI, "YouWin");
        }
    }
}