using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class Woodman : GameEvent
    {

        public override void Init()
        {
            this.ID = "Woodman";
            Simple = false;
            initialized = false;


            ////////
            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Yooman");
            act.ID = "FirstDialogue";
            GetObject("Torald").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("YoomanAnswer")
                .SetText("Yoomantext")
                .SetCallData("YoomanQuest")
                .SetType(ActionType.Pack)
                .SetAppearCondition(FlagCondition.Make("WoodmanTalk", false)));

            act.list.Add(ActionButtonInfo.Create("WoodGet")
                .SetText("Yoomantext1")
                .SetCallData("QuestEnd")
                .SetType(ActionType.Pack)
                .SetAvailableCondition(StatCondition.Make("Wood", 5, StatCondition.StatConType.More))
                .SetAppearCondition(FlagCondition.Make("WoodmanTalk"))
                );

            act.list.Add(ActionButtonInfo.Create("SadmanAnswer").SetText("Sadmantext").SetType(ActionType.Close));

            
            GetObject("Torald").Activity.PushPack("QuestEnd", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "Torald", timeMode:TooltipTimeMode.Click, Text:"QuestEnd"),
                ReactLock.Create("Torald")
            });

            GetObject("Torald").Activity.PushPack("YoomanQuest", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero,  "Torald", timeMode:TooltipTimeMode.Click, Text:"Youmansay"),
                FlagWork.Create("WoodmanTalk", "On"),
                ActionWork.Create("FirstDialogue", "Torald", null, _WorkType:"ChangeText", text:"ToraldAskWood")
            });

            //////yoomanquest
            GetObject("forest14").AddAction(SkillCheckAction.Make("TYK", "YoomanQuest", "Proval",
                ResultID.Create().SetSuccesID("Woodhave"),
                new List<SkillCheckObject> ()
                {
                    SkillCheckObject.Create("strenght", 6, 2 )
                }
               ));

            //////wood
            GetObject("forest14").Activity.PushPack("Woodhave", new List<GameEvent>()
            {
                AddStat.Create("Wood", 5)
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