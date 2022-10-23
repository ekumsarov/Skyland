using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class WolfTrain : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "WolfTrain";

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

        public void FirstFeed()
        {
            SM.SetFlag("FWolfFeed");
            SM.SetFlag("WolfSpeech");

            if(SM.Stats["Food"].Count >= 1)
                SM.AddStat(-1, "Food");

            End();
        }

        public void SecondFeed()
        {
            SM.SetFlag("SWolfFeed");

            End();
        }

        public void ThirdFeed()
        {
            SM.SetFlag("TWolfFeed");

            End();
        }
    }
}