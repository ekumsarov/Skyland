using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class OwnTown : GameEvent
    {

        Subscriber subscriber;

        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "OwnTown";

            Simple = false;

            subscriber = Subscriber.Create(this);

            initialized = false;

//            RM.GetObject("forest14").AddIcon(IconObject.Create("HunterTalk", "BaseIcon", IconInteractType.Object));
//            RM.GetObject("forest14").LockLocation();


//            BM.Mains[14].ClearAllAvaliable();

            BM.Mains[14].ReplaceActionChoice("BuiltAction", "OpenMenu",
                ActionButtonInfo.Create("OpenMenu")
                .SetText("BuildAction")
                .SetCallData("OpenMenu")
                .SetType(ActionType.Special)
                .SetAvailableCondition(StatCondition.Make(iStat.Create(StatType.Unit, 3)))
                .SetAppearCondition(FlagCondition.Make("BuildHunterHut"))
                );

            BM.Mains[14].ReplaceActionChoice("BuiltAction", "CloseDialogue",
                ActionButtonInfo.Create("CloseDialogue")
                .SetType(ActionType.Close)
                .SetAppearCondition(FlagCondition.Make("AfterFirstDialogue"))
                );

            BM.Mains[14].AddActionChoice("BuiltAction",
                ActionButtonInfo.Create("FirstStep")
                .SetCallData("AFirstStep")
                .SetType(ActionType.Pack)
                );

            BM.Mains[14].AddActionChoice("BuiltAction",
                ActionButtonInfo.Create("TryEditor")
                .SetCallData("Action2")
                .SetType(ActionType.Action)
                );

            Object.Activity.PushPack("AFirstStep", new List<GameEvent>() {
                FlagWork.Create("AfterFirstDialogue", "On"),
                ActChoiceWork.Create("Sanctuary", "BuiltAction", "FirstStep", _ActType: "Remove"),
                ShowTooltip.Create(Vector3.zero, "Sanctuary", exTime: 2.0f, timeMode: TooltipTimeMode.Click, Text: "AFirstStep"),
                ActionWork.Create("BuiltAction", "Sanctuary", null, _WorkType: "ChangeText", text:"BFirstStep"),
                QuestWork.Create("RepaierBoat", quest:
                    MainQuest.Create("RepaierBoat")
                        .SetDescription("RepaierBoatDes")
                        .AddNode(QuestNode.Create("ForesterTalk").SetDescription("ForesterTalkDes"))
                        .AddNode(QuestNode.Create("FindUnits").SetDescription("FindUnitsDes"))),
                SetupIconMainEvent.Create("ForesterOwn", "ForesterPlug")
            });

            SkyObject tempIsl = GetObject("island_15");

            if (tempIsl != null)
                tempIsl.Activity.PushPack("Nephew", new List<GameEvent>()
                {
                    CallEvent.Create("NephewDanger", to:"LonelyNomad")
                });
            else
                Debug.LogError("EROOOOOOOOOOOOOOR");
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