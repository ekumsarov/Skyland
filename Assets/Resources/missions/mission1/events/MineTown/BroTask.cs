using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class BroTask : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "BroTask";

            Simple = false;

            subscriber = Subscriber.Create(this);

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

            Object.CallAction("OTStartTown");

            End();
        }

        public void SetupTask()
        {
            if (TaskType == 1)
            {
//                string eve = "{ 'Event':'CaravanTask', 'param': { 'TaskType':'3', 'Action':'SetupTask' }, 'to':'Torald' }";
//                JSONNode nod = JSON.Parse(eve.Replace("'", "\""));
//                GetObject("Torald").Activity.CallEvent(nod);

                Object.Activity.PushPack("OTFindTownQuest", new List<GameEvent>
                {
                    QuestBut.Create("OTFindTwon", Object.ID, Text:"OTFindTownQuest"),
                    CallEvent.Create(Event:"{ 'Event':'CaravanTask', 'param': { 'TaskType':'3', 'Action':'SetupTask' }, 'to':'Torald' }", to:"Torald")
                });

                SceneObject par = GetObject("MainShip") as SceneObject;
                par.LockLocation();

                Actions act = Actions.Get("Context");
                act.Text = LocalizationManager.Get("OTStartTown");
                act.ID = "OTStartTown";
                Object.AddAction(act);

                act.list.Add(ActionButtonInfo.Create("CloseDialogue").SetCallData("OTFindTownQuest").SetType(ActionType.Pack));
            }
        }

    }
}
