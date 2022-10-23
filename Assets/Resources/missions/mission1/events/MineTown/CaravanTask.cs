using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class CaravanTask : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "CaravanTask";

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
            if (TaskType == 3)
            {

            }
        }

    }
}