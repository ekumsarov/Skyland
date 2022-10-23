using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class WolfQuestEvent : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "WolfQuestEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            initialized = false;

            Object.MainEvent = "WolfAction";

            Object.Group.AddNewHero("Wolf");

            Object.AddAction(SkillCheckAction.Make("TryToCatchWolf", "CatchWolfAgain", "CatchWolfAgainFail",
                ResultID.Create()
                    .SetSuccessCallback(CaughtWolf)
                    .SetFailID("FailCaughtWolf"),
                new List<SkillCheckObject>()
                {
                    SkillCheckObject.Create("dexterity", 7, 2),
                    SkillCheckObject.Create("intelligence", 5, 1)
                }
                ));

            Actions act = Actions.Get("Context");
            act.ID = "WolfAction";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("CatchWolf").SetCallData("CatchWolfAgain").SetAvailableCondition(LootCondition.Make("WolfTrap")));
            act.list.Add(ActionButtonInfo.Create("KillWolf").SetCallData("KillWolf").SetType(ActionType.Pack));
            act.list.Add(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

            this.Object.Activity.PushPack("FailCaughtWolf", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "Wolf", Text:"FailCaughtWolf"),
                BattleEvent.Create("Player", "Wolf", ResultID.Create().SetSuccessCallback(WolfDead).SetFailCallback(PlayerDead))
            });

            this.Object.Activity.PushPack("KillWolf", new List<GameEvent>() {
                BattleEvent.Create("Player", "Wolf", ResultID.Create().SetSuccessCallback(WolfDead).SetFailCallback(PlayerDead))
            });
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

        public void CaughtWolf()
        {
            QS.CompleteQuest("WolfQuest");
            IconObject temp = Object as IconObject;
            if(temp != null)
            {
                temp.Lock = true;
                temp.Visible = false;
                temp.RemoveIcon();
            }

            MapLocationIcon sanIcon = GM.GetIcon("SanctuaryMainIcon") as MapLocationIcon;
            if(sanIcon != null)
            {
                sanIcon.AddActionChoice("StandartAction", ActionButtonInfo.Create("WolfTrainChoice").SetCallData("WolfTrainAction"));
                Actions act = Actions.Create("Context", "WolfTrainAction")
                    .AddChoice(
                        ActionButtonInfo.Create("WolfFeed1").SetCallback(FirstFeed).SetAvailableCondition(StatCondition.Make("Food", 1, StatCondition.StatConType.More)))
                    .AddChoice(
                        ActionButtonInfo.Create("WolfFeed2").SetCallback(SecondFeed).SetAvailableCondition(StatCondition.Make("Food", 20, StatCondition.StatConType.More)))
                    .AddChoice(
                        ActionButtonInfo.Create("WolfFeed3").SetCallback(ThirdFeed).SetAvailableCondition(StatCondition.Make("Food", 50, StatCondition.StatConType.More)))
                    .AddChoice(
                        ActionButtonInfo.Create("WolfFeed4").SetCallback(ThirdFeed).SetAvailableCondition(StatCondition.Make("Food", 100, StatCondition.StatConType.More)))
                    .AddChoice(
                        ActionButtonInfo.Create("MoveBack").SetType(ActionType.Action))
                        ;
                sanIcon.AddAction(act);
            }

            End();
        }

        public void WolfDead()
        {
            QS.CompleteQuest("WolfQuest");
            IconObject temp = Object as IconObject;
            if (temp != null)
            {
                temp.Lock = true;
                temp.Visible = false;
                temp.RemoveIcon();
            }
            SM.Stats["Food"].Count += 20;
        }

        public void PlayerDead()
        {
            Object.Group.RestoreCurHP("Wolf");
            SM.Stats["PlayerInfection"].Count += 5;
        }

        public void FirstFeed()
        {
            GM.GetIcon("SanctuaryMainIcon").RepmoveActionChoice("WolfTrainAction", "WolfFeed1");
            SM.SetFlag("WolfSpeech");

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Instantly, TooltipObject.UI, "Теперь волк будет помогать тебе в диалогах. Но пока нет");

            if (SM.Stats["Food"].Count >= 1)
                SM.AddStat(-1, "Food");

            End();
        }

        public void SecondFeed()
        {
            GM.GetIcon("SanctuaryMainIcon").RepmoveActionChoice("WolfTrainAction", "WolfFeed2");
            SM.SetFlag("WolfHuntOnLeming");

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "Теперь волк будет приносить по одной тушке леминга в день");

            if (SM.Stats["Food"].Count >= 20)
            {
                SM.AddStat(-20, "Food");
                ExpiredDay.ExpiredAfterDay(DayInfo.Create(1), act: LemingSkin);
                Object.Activity.PushPack("WolfHuntPack", new List<GameEvent>()
                {
                    ShowTooltip.Create(UIM.ScreenCenter,null, Text:"Wuf"),
                    ShowTooltip.Create(UIM.ScreenCenter, null, Text:"GoodBoy")
                }) ;
            }
      

            End();
        }

        public void LemingSkin()
        {
            SM.AddStat(1, "AnimalSkin");
            Object.Activity.callActivityPack("WolfHuntPack");
            ExpiredDay.ExpiredAfterDay(DayInfo.Create(1), act: LemingSkin);
        }

        public void ThirdFeed()
        {
            GM.GetIcon("SanctuaryMainIcon").RepmoveActionChoice("WolfTrainAction", "WolfFeed3");
            GM.GetIcon("SanctuaryMainIcon").RepmoveActionChoice("StandartAction", "WolfTrainChoice");
            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "WolfTakeMapLoot");

        }

        public void FourthFeed()
        {
            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "YouKillWolf");
            MapLocationIcon sanIcon = GM.GetIcon("SanctuaryMainIcon") as MapLocationIcon;
            if (sanIcon != null)
            {
                sanIcon.RepmoveAction("WolfTrainAction");
                sanIcon.RepmoveActionChoice("StandartAction", "WolfTrainChoice");
            }

        }
    }
}