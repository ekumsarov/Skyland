using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;
using EventPackSystem;


namespace GameEvents1
{
    public class BokenShipQuestSet : GameEvent
    {
        bool GetEngine;
        bool FirstMeet;
        bool NoResForFood;

        int NeedFood;

        public override void Init()
        {
            this.ID = "BokenShipQuestSet";

            Simple = false;

            GetEngine = false;
            FirstMeet = true;
            NoResForFood = true;

            NeedFood = 25;

            Object.Activity.PushPack("BrokenShipMeet", new List<GameEvent>
            {
                ActivateLocationPanel.Create("BrokenShip", false),
                ShowTooltip.Create(Vector3.zero, "BrokenShipObject", Text:"BrokenShipFind1"),
                ShowTooltip.Create(Vector3.zero, "BrokenShipObject", Text:"BrokenShipFind2"),
                ShowTooltip.Create(Vector3.zero, "BrokenShipObject", Text:"BrokenShipFind3"),
                ShowTooltip.Create(Vector3.zero, "BrokenShipObject", Text:"BrokenShipFind4"),
                QuestWork.Create("BrokenShip", quest: MainQuest.Create("BrokenShipQuest").SetIcon("oprions").SetDescription("BrokenShipQuestDes")
                    .AddNode(
                        QuestNode.Create("RepaierShipBoard").SetDescription("RepaierShipBoardDes").SetHideOnComplete(true).SetIcon("oprions")
                    )
                    .AddNode(
                        QuestNode.Create("RepaierShipEngine").SetDescription("RepaierShipEngineDes").SetHideOnComplete(true).SetIcon("oprions")
                    )
                    .AddNode(
                        QuestNode.Create("RepaierShipGenerator").SetDescription("RepaierShipGeneratorDes").SetHideOnComplete(true).SetIcon("oprions")
                    )),
                ActivateLocationPanel.Create("BrokenShip", true)
            });

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
    }
}