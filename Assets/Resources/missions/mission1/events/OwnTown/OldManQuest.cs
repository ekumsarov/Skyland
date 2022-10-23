using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class OldManQuest : GameEvent
    {
        bool firstMeet = true;

        public override void Init()
        {
            this.ID = "OldManQuest";

            Simple = false;
            initialized = false;

            this.Object.Activity.PushPack("ForesterGreetFounded", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Tootip, Text: "BuildHunterHut"),
                MoveObject.Create("OldMan", Vector3.zero, "forest_14")
            });

            this.Object.Activity.PushPack("ReadyBuildHunterHut", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Tootip, Text: "BuildHunterHut"),
                AddAvaliableBuild.Create("Sanctuary", "Lumberjack"),
                AddStat.Create("Unit", 2),
                ReactLock.Create("LonelyHut")

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

            if (firstMeet)
            {
                if (SM.CheckFlag("FoundNephew") || SM.CheckFlag("LostNephew"))
                    FoundNephew();
                else
                    GoFindNephew();

                firstMeet = false;
            }

            this.Object.CallAction("ForesterTalk");

            End();
        }

        void FoundNephew()
        {
            string textKey = "OldManFoundHasQuest";
            if (!QS.HasQuest("FindNephew"))
                textKey = "OldManFoundNoQuest";

            this.Object.AddAction(Actions.Create("Context", "ForesterTalk").SetText(textKey)
                    .AddChoice(ActionButtonInfo.Create("OldManHelp").SetCallData("CompleteOldManQuest").SetType(ActionType.Pack).SetAppearCondition(AvaliableQuestCondition.Make("FindNephew")))
                    .AddChoice(ActionButtonInfo.Create("OldManHelp").SetCallData("LostNephewQuest").SetType(ActionType.Pack).SetAppearCondition(AvaliableQuestCondition.Make("FindNephew", false)))
                    .AddChoice(ActionButtonInfo.Create("KnowSecret").SetCallData("OldManNotYourBuiseness").SetAppearCondition(FlagCondition.Make("KnowSecret")))
                    );
        }

        void GoFindNephew()
        {
            SM.SetFlag("FirstFoundForester");

            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("ForesterGreeting");
            act.ID = "ForesterTalk";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("TryRecruitForester").SetCallData("GoFindNephew"));

            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("GoFindNephew");
            act.ID = "GoFindNephew";
            Object.AddAction(act);

            act.list.Add(ActionButtonInfo.Create("GoFindNephew").SetText("AgreeFindNephew").SetCallData("AgreeFindNephew").SetType(ActionType.Pack));


            this.Object.Activity.PushPack("AgreeFindNephew", new List<GameEvent>() {
                QuestWork.Create("FindNephew", quest: MainQuest.Create("FindNephew").SetTitle("FindNephew").SetDescription("FindNephewDes")),
                ActionWork.Create("ForesterTalk", "OldMan", new List<ActionButtonInfo> () {
                    ActionButtonInfo.Create("ReadyFindNephew")
                        .SetCallData("ForesterGreetFounded")
                        .SetType(ActionType.Pack)
                        .SetAvailableCondition(FlagCondition.Make("FoundNephew")),
                    ActionButtonInfo.Create("NotYetFindNephew").SetType(ActionType.Close)
                    },
                    _WorkType:"Replace",
                    text:"DidFindNephew")
            });
        }
    }
}