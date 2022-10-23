using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class Oldman : GameEvent
    {

        public override void Init()
        {
            this.ID = "Oldman";

            Simple = false;

            initialized = false;

            Object.AddAction(Actions.Create("Context", "Oldmenmeet")
                .AddChoice(
                    ActionButtonInfo.Create("Oldtruble").SetCallData("Questoldmen"))
                );
            Object.AddAction(Actions.Create("Context", "Questoldmen")
            .AddChoice(
                ActionButtonInfo.Create("").SetCallData("Okkillwolf").SetType(ActionType.Pack)
               )
            .AddChoice(
                ActionButtonInfo.Create("IWillTry").SetCallData("Ipoyamay").SetType(ActionType.Pack))
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