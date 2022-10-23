using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class ForesterSetup : GameEvent
    {

        public override void Init()
        {
            this.ID = "ForesterSetup";

            Simple = false;

            initialized = false;

            Object.Activity.PushPack("ForesterPlug", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, "ForesterOwn", exTime:2f, Text:"ForesterPlug"),
                LootWork.Create("WolfBow", LS.LootType.Weapon, "Player"),
                LootWork.Create("WolfTrap", LS.LootType.Extra, "Player"),
                ShowReward.Create(new List<RewardItemInfo>() { RewardItemInfo.Create("icon_bow"), RewardItemInfo.Create("lock") } ),
                QuestWork.Create("WolfCaught", nQuest:QuestNode.Create("WolfCaught").SetDescription("WolfCaughtDes").SetIcon("lock")),
                QuestWork.Create("WolfCaught", nQuest:QuestNode.Create("WolfKilled").SetDescription("WolfKilledDes").SetIcon("Sword")),
                QuestWork.Create("ForesterTalk", "Complete"),
                ReactLock.Create("forest14", false)
            });

            Object.AddAction(Actions.Create("Action", "ForesterWolfAsk")
                .AddChoice(ActionButtonInfo.Create("WolfCaught").SetCallData("WolfCaughtSuccess").SetAvailableCondition(QuestCondition.Make("WolfCaught")).SetType(ActionType.Pack))
                .AddChoice(ActionButtonInfo.Create("KilledWolfAsk").SetCallData("KillWolfSuccess").SetAvailableCondition(QuestCondition.Make("WolfKilled")).SetType(ActionType.Pack))
                .AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close))
            );
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