using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class StrangeNote : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "StrangeNote";

            Simple = false;

            subscriber = Subscriber.Create(this);

            Stat.Create("Note", Represent.Type.Simple, maxVal:1);

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

    }
}