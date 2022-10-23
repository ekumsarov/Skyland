using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class LonelyHut : GameEvent
    {
        public override void Init()
        {
            this.ID = "LonelyHut";

            Simple = false;
            initialized = false;

            SceneObject temp = this.Object as SceneObject;

            if(temp != null)
            {
                IconObject.Create("LonelyHut", "BaseIcon", IconInteractType.Object, temp);
            }

            this.Object.AddAction(Actions.Create("Context", "LonelyHut")
                .AddChoice(ActionButtonInfo.Create("InspectHut").SetCallData("StartInspectHut").SetType(ActionType.Pack))
                .AddChoice(ActionButtonInfo.Create("Quit").SetType(ActionType.Close))
            );

            this.Object.Activity.PushPack("StartInspectHut", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "OldMan", exTime: 1.0f, timeMode: TooltipTimeMode.Tootip, Text: "OldManAngry"),
                ShowTooltip.Create(Vector3.zero, "Player", exTime: 1.0f, timeMode: TooltipTimeMode.Tootip, Text: "AnswerOldMan"),
                LootWork.Create("Trap", LS.LootType.Extra, "Player"),
                ShowReward.Create(new List<RewardItemInfo>() { RewardItemInfo.Create("lock") } ),
                ActionWork.Create("LonelyHut", "LonelyHut", null, _WorkType:"Remove"),
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
            
            End();
        }
    }
}