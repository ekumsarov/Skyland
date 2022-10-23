using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class IconTestPack : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "IconTestPack";

            Simple = false;

            subscriber = Subscriber.Create(this);

            initialized = false;

            Object.AddAction(Actions.Create("Context", "Greeting")
                .AddChoice(
                    ActionButtonInfo.Create("CreateObject")
                        .SetAppearCondition(FlagCondition.Make("MakeObject", false))
                        .SetCallData("MakeObject")
                        .SetType(ActionType.Pack))
                .AddChoice(
                    ActionButtonInfo.Create("AddIconObject")
                        .SetAppearCondition(FlagCondition.Make("MakeIconObject", false))
                        .SetCallData("MakeIconObject")
                        .SetType(ActionType.Pack))
                .AddChoice(
                    ActionButtonInfo.Create("AddIconAction")
                        .SetAppearCondition(FlagCondition.Make("MakeIconAction", false))
                        .SetCallData("MakeIconAction")
                        .SetType(ActionType.Pack))
                .AddChoice(
                    ActionButtonInfo.Create("MoveIconAction")
                        .SetAppearCondition(FlagCondition.Make("MoveIconAction", false))
                        .SetCallData("MoveIconAction")
                        .SetType(ActionType.Pack))
                .AddChoice(
                    ActionButtonInfo.Create("MoveBowIconAction")
                        .SetAppearCondition(FlagCondition.Make("MoveBowIconAction", false))
                        .SetCallData("MoveBowIconAction")
                        .SetType(ActionType.Pack))
                .AddChoice(
                    ActionButtonInfo.Create("Close")
                        .SetType(ActionType.Close))
                );

            Object.Activity.PushPack("MakeObject", new List<GameEvent>()
            {
                FlagWork.Create("MakeObject", "On"),
                CreateObject.Create("LonelyHut")
            });

            Object.Activity.PushPack("MakeIconObject", new List<GameEvent>()
            {
                FlagWork.Create("MakeIconObject", "On"),
                CreateIcon.Create("logic", "gerold", IconInteractType.Object, IconInteractType.SubLocation, "LonelyHut")
            });

            Object.Activity.PushPack("MoveIconAction", new List<GameEvent>()
            {
                FlagWork.Create("MoveIconAction", "On"),
                MoveIcon.Create("asist", "LonelyHut"),
                CallAction.Create("Greeting")
            });

            Object.Activity.PushPack("MakeIconAction", new List<GameEvent>()
            {
                FlagWork.Create("MakeIconAction", "On"),
                SetupMainEvent.Create("logic", "Hey"),
                ActionWork.CreateAddAction("logic", Actions.Create("Context", "Hey").AddChoice(
                    ActionButtonInfo.Create("Close")
                        .SetType(ActionType.Close)))
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