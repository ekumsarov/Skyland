using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class PlayerInfection : GameEvent
    {

        Subscriber subscriber;

        float CurrentInfection;
        bool firstInfection;

        public override void Init()
        {
            this.ID = "LoyaltyQuest";

            Simple = false;

            subscriber = Subscriber.Create(this);

            this.CurrentInfection = 10f;

            this.firstInfection = true;

            Stat.Create("PlayerInfection", Represent.Type.Percent, curVal: 10f, maxVal: 100f);

            Object.Activity.PushPack("FirstInfection", new List<GameEvent>()
                {
                    PerpareCutSene.Create(),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Tootip, Text:"FirstInfection1"),
                    ShowTooltip.Create(UIM.RandomScreenPoint, "", 1.5f, timeMode:TooltipTimeMode.Tootip, Text:"FirstInfection2"),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Tootip, Text:"FirstInfection3"),
                    ShowTooltip.Create(UIM.RandomScreenPoint, "", 1.5f, timeMode:TooltipTimeMode.Tootip, Text:"FirstInfection4"),
                    CompleteCutScene.Create()
                });

            subscriber.AddEvent(SM.Stats["PlayerInfection"].CallFunction);

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

        public void PlayerDead()
        {
            SM.Stats["PlayerInfection"].Count += 5;
            Object.Group.RestoreCurHP("Group");
        }

        public void PlayerInfectionChanged()
        {
            this.CurrentInfection = SM.Stats["PlayerInfection"].Count;

            if(this.firstInfection)
            {
                Object.Activity.callActivityPack("FirstInfection");
                this.firstInfection = false;
                return;
            }

            this.InfectionLevel();
            ////
            /// Maybe Add some more logic
        }

        public void InfectionLevel()
        {
            
            if(this.CurrentInfection>=25)
            {
                GM.Player.Group.AddSkills("Player", SkillObject.Make("stamina", 5, 8));
                GM.Player.Group.AddAction("Player", "BlackHeal");
            }
            
        }
    }
}