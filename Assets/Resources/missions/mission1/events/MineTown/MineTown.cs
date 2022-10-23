using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class MineTown : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "MineTown";

            Simple = false;

            subscriber = Subscriber.Create(this);

            SetupStart();

            Stat.Create("MTQLoyaltyMine", curVal: 50, maxVal: 100);

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

            if (Math.Abs(SM.Stats["Note"].Count) < Double.Epsilon)
                Found();

            End();
        }

        void Found()
        {

        }

        void SetupStart()
        {
            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("MTQGreeting");
            act.ID = "MTQGreeting";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("MTQSimpleAnswer").SetCallData("MTQWhoDid"));
            act.list.Add(ActionButtonInfo.Create("MTQBlackAttack").SetCallData("MTQLaugh"));


            /////////////
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("MTQWhoDid");
            act.ID = "MTQWhoDid";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("MTQLie").SetCallData("MTQNeedHelp").SetType(ActionType.Pack));
            act.list.Add(ActionButtonInfo.Create("MTQBlackDid").SetCallData("MTQLaugh"));

            /////////////
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("MTQLaugh");
            act.ID = "MTQLaugh";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("MTQShowArm").SetCallData("MTQBroDef").SetType(ActionType.Pack));
            act.list.Add(ActionButtonInfo.Create("MTQResponse").SetCallData("MTQBeUseful"));
            act.list.Add(ActionButtonInfo.Create("MTQThreat").SetCallData("MTQIrritation").SetType(ActionType.Pack));

            ////////////
            ///
            Object.Activity.PushPack("MTQBeUseful", new List<GameEvent>
            {
                ShowTooltip.Create(Vector3.zero, "MineTown", Text:"MTQBeUseful"),
                CallPack.Create("MTQBroDef", "MineTown")
            });

            ////////////
            ///
            Object.Activity.PushPack("MTQBroDef", new List<GameEvent>
            {
                ShowTooltip.Create(Vector3.zero, "MineTown", Text:"MTQBroDef"),
                SetupStat.Create("MTQLoyaltyMine", -5),
                //Torald
                CreateNPC.Create("Bro1", "island_14"),
                AddEvent.Create("BroTask", "Bro1"),
                CallEvent.Create(Event:"{ 'Event':'BroTask', 'param': { 'TaskType':'1', 'Action':'SetupTask' }, 'to':'Bro1' }")
            });

            ////////////
            ///
            Object.Activity.PushPack("MTQIrritation", new List<GameEvent>
            {
                ShowTooltip.Create(Vector3.zero, "MineTown", Text:"MTQIrritation"),
                SetupStat.Create("MTQLoyaltyMine", -10),
                CallPack.Create("MTQBroDef", "MineTown")
            });

            ////////////
            ///
            Object.Activity.PushPack("MTQNeedHelp", new List<GameEvent>
            {
                ShowTooltip.Create(Vector3.zero, "MineTown", Text:"MTQNeedHelp"),
                //Torald
                CreateNPC.Create("Torald", "island_14"),
                AddEvent.Create("CaravanTask", "Torald"),
                CallEvent.Create(Event:"{ 'Event':'CaravanTask', 'param': { 'TaskType':'2', 'Action':'SetupTask' }, 'to':'Torald' }"),
                //SubQuest - MTQMineRepair
                CreateNPC.Create("Nomad", "island_14"),
                AddEvent.Create("MTQMineRepair", "Nomad")
            });
        }

    }
}
