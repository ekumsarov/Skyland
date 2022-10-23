using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class Nomads : GameEvent
    {
        bool firstRedearch;

        int daysTicks;

        float FoodGrab;
        float WoodGrab;
        float SkystoneGrab;

        string NomaAttackText;
        string NomadGiveText;
        string NomadAttackWin;
        string NomadAttackLose;

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "Nomads";

            Simple = false;

            subscriber = Subscriber.Create(this);

            daysTicks = 10;
            FoodGrab = 0.3f;
            WoodGrab = 0.2f;
            SkystoneGrab = 0.1f;

            NomaAttackText = "NomaAttackText";
            NomadGiveText = "NomadGiveText";
            NomadAttackWin = "NomadAttackWin";
            NomadAttackLose = "NomadAttackLose1";

            firstRedearch = true;

            ActionSet();

            //QuestButton.Create("NomadThreat", Object, "war", "NomadThreat");

            ExpiredDay.ExpiredAfterDay(DayInfo.Create(2), everyCall : StartAttack);

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

            Object.CallAction("NomadVillage");

            if(firstRedearch)
            {
                Object.ChangeActionText("NomadVillage", "");
                firstRedearch = false;
            }

            End();
        }

        void StartAttack()
        {

        }

        void ActionSet()
        {
            /*
             * Nomad interact
             */
            Actions act = Actions.Get("");
            act.Text = LocalizationManager.Get("NomadVillage1");
            act.ID = "NomadVillage";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("NomadVillageStartTalk").SetCallData("NomadVillageTalk"));
            act.list.Add(ActionButtonInfo.Create("QuitTask").SetType(ActionType.Close));

            /*
             * Nomad talk
             */
            act = Actions.Get("");
            act.Text = LocalizationManager.Get("NomadVillageTalk1");
            act.ID = "NomadVillageTalk";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("NomadVillageAsk1").SetCallData("NomadVillageAsk2"));
            act.list.Add(ActionButtonInfo.Create("FightTask").SetCallData("NomadVillageTalk"));
            act.list.Add(ActionButtonInfo.Create("QuitTask").SetType(ActionType.Close));

            /*
             * Nomad talk 2
             */
            act = Actions.Get("");
            act.Text = LocalizationManager.Get("NomadVillageTalk2");
            act.ID = "NomadVillageTalk2";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("NomadVillageAsk2").SetCallData("NomadVillageAsk3"));
            act.list.Add(ActionButtonInfo.Create("NomadVillageAsk3").SetType(ActionType.Close));

            /*
             * Nomad talk 3
             */
            act = Actions.Get("");
            act.Text = LocalizationManager.Get("NomadVillageTalk3");
            act.ID = "NomadVillageTalk3";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("NomadVillageAsk4").SetCallData("NomadVillageTalk"));
            act.list.Add(ActionButtonInfo.Create("NomadVillageAsk3").SetType(ActionType.Close));

            /*
             * Nomad quest
             */
            act = Actions.Get("quest");
            act.Text = LocalizationManager.Get("NomadThreat1");
            act.ID = "NomadThreat";
            Object.AddAction(act);


        }
    }
}