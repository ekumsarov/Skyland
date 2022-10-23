using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class ForestWolf : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "ForestWolf";

            Simple = false;

            subscriber = Subscriber.Create(this);

            initialized = false;

            Object.AddAction(SkillCheckAction.Make("TryToCatchWolf", "TryToCatchWolf", "TryToCatchWolfFail",
                ResultID.Create()
                    .SetSuccesID("CatchWolfSuccess")
                    .SetFailID("CatchWolfAgain"),
                new List<SkillCheckObject>()
                    {
                        SkillCheckObject.Create("dexterity", 9, 2),
                        SkillCheckObject.Create("intelligence", 6, 1)
                    }
                ));

            Object.AddAction(SkillCheckAction.Make("TryToCatchWolf", "CatchWolfAgain", "CatchWolfAgainFail",
                ResultID.Create()
                    .SetSuccesID("CatchWolfSuccess")
                    .SetFailID("KillWolf"),
                new List<SkillCheckObject>()
                {
                    SkillCheckObject.Create("dexterity", 7, 2),
                    SkillCheckObject.Create("intelligence", 5, 1)
                }
                ));

            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("HunterTalk");
            act.ID = "HunterTalk";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("CatchWolf").SetCallData("TryToCatchWolf").SetAvailableCondition(LootCondition.Make("WolfTrap")));
            //act.list.Add(ActionButtonInfo.Create("CatchWolf").SetCallData("TryToCatchWolf").SetAvailableCondition(LootCondition.Make("WolfTrap")));
            act.list.Add(ActionButtonInfo.Create("KillWolf").SetCallData("KillWolf").SetType(ActionType.Pack));
            act.list.Add(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

            this.Object.Activity.PushPack("CatchWolfSuccess", new List<GameEvent>() {
                AddEvent.Create("WolfTrain", "MainShip"),
                ActChoiceWork.Create("MainShip", "MainAction", "WolfFeed", _ActType: "Add", _actionsChoice: ActionButtonInfo.Create("WolfFeed").SetCallData("WolfFeedAct")),
                ActionWork.Create("WolfFeedAct", "MainShip", new List<ActionButtonInfo> () {
                    ActionButtonInfo.Create("WolfPartnerFeed1")
                        .SetCallData("{ 'Event':'WolfTrain', 'Action':'FirstFeed' }")
                        .SetType(ActionType.Event)
                        .SetAppearCondition(FlagCondition.Make("FWolfFeed", false))
                        .SetAvailableCondition(StatCondition.Make("Food", 1)),
                    ActionButtonInfo.Create("WolfPartnerFeed2")
                        .SetCallData("{ 'Event':'WolfTrain', 'Action':'SecondFeed' }")
                        .SetType(ActionType.Event)
                        .SetAppearCondition(FlagCondition.Make(new Dictionary<string, bool>() { { "FWolfFeed", true }, { "SWolfFeed", false } }))
                        .SetAvailableCondition(StatCondition.Make("Food", 25)),
                    ActionButtonInfo.Create("WolfPartnerFeed3")
                        .SetCallData("{ 'Event':'WolfTrain', 'Action':'ThirdFeed' }")
                        .SetType(ActionType.Event)
                        .SetAppearCondition(FlagCondition.Make(new Dictionary<string, bool>() { { "FWolfFeed", true }, { "TWolfFeed", false } }))
                        .SetAvailableCondition(StatCondition.Make("Food", 50)),
                    ActionButtonInfo.Create("Back")
                        .SetText("BackButton")
                        .SetCallData("MainAction")
                    },
                    text:null),
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Tootip, Text: "CatchedWolf"),
                AddStat.Create("Food", 1),
                ActiveStat.Create("Food"),
                ReactLock.Create(this.Object.ID)
            }); 

            this.Object.Activity.PushPack("KillWolf", new List<GameEvent>() {
                BattleEvent.Create("Player", "Setup", ResultID.Create().SetSuccesID("KilledWolf").SetFailID("KilledWolf"), "island_14", enemyStack: new List<string>() { "wolf" })
            });

            this.Object.Activity.PushPack("KilledWolf", new List<GameEvent>()
            {
                AddStat.Create("Food", 10),
                ActiveStat.Create("Food"),
                ShowTooltip.Create(Vector3.zero, this.Object.ID, timeMode: TooltipTimeMode.Click, Text: "KillWolfReward"),
                ReactLock.Create(this.Object.ID)
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
    }
}