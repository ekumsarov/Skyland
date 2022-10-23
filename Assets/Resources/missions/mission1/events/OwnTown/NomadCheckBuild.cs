using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class NomadCheckBuild : GameEvent
    {

        Subscriber subscriber;

        // Quest Type
        // 0 - nomad will attack
        // 1 - nomad will offer service
        // 2 - nomad will come for skystone
        int QuestType = 0; 

        public override void Init()
        {
            this.ID = "NomadCheckBuild";

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

            End();
        }

        public void NomadAngry()
        {
            if (subscriber == null)
                subscriber = Subscriber.Create(Object);

            QuestType = 0;
            subscriber.AddEvent(TriggerType.FlagChanged.ToString(), "FirstStepComplete");
        }

        public void NomadBuyout()
        {
            QuestType = 2;
            subscriber.AddEvent(TriggerType.FlagChanged.ToString(), "FirstStepComplete");
        }

        public void NomadService()
        {
            QuestType = 1;
            subscriber.AddEvent(TriggerType.FlagChanged.ToString(), "FirstStepComplete");
        }

        public void FlagChanged()
        {
            if(QuestType == 0)
                ExpiredDay.ExpiredAfterDay(DayInfo.Create(UnityEngine.Random.Range(3, 8)), packID: "NomadNephewAttack");
            else if (QuestType == 1)
                ExpiredDay.ExpiredAfterDay(DayInfo.Create(UnityEngine.Random.Range(2, 5)), packID: "NomadNephewService");
            else if (QuestType == 2)
                ExpiredDay.ExpiredAfterDay(DayInfo.Create(UnityEngine.Random.Range(3, 8)), packID: "NomadNephewBuyoutCheck");
        }
    }
}
