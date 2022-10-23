using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;
using UnityEditor;

namespace GameEvents1
{
    public class NewQuest : GameEvent
    {

        public override void Init()
        {
            this.ID = "NewQuest";
            Simple = false;
            initialized = false;

            ////////////////
            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("ToraldHi");
            act.ID = "FirstDialogue";
            GetObject("Cave").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("AnswerTorald1").SetText("AnswerToraldHi").SetCallData("ToraldQuest"));
            act.list.Add(ActionButtonInfo.Create("AnswerTorald2").SetText("AnswerToraldWTF").SetType(ActionType.Close));
            act.list.Add(ActionButtonInfo.Create("ToraldSkillCheck").SetText("AnswerToraldWTF").SetCallData("ToraldSkillCheck"));


            ////////////////// ToraldQuest
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("ToraldHi");
            act.ID = "ToraldQuest";
            GetObject("Cave").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("AnswerTorald2").SetText("AnswerToraldWTF").SetType(ActionType.Close));


            /////////////////////// SkillCheck
            ///
            GetObject("Cave").AddAction(SkillCheckAction.Make("GHfjh", 
                "ToraldSkillCheck", 
                "gdfg",
                ResultID.Create().SetSuccesID("WeWinTorald").SetFailID("ToraldWin"),
                new List<SkillCheckObject>()
                {
                    SkillCheckObject.Create("strenght", 6, 2)
                }
                ));

            //////////////////////////
            ///
            GetObject("Cave").Activity.PushPack("WeWinTorald", new List<GameEvent>()
            {
                AddStat.Create("Food", 5),
                ShowTooltip.Create(Vector3.zero, "Cave", 2f, timeMode:TooltipTimeMode.Dialog, Text:"ToraldCongrat"),
                ReactLock.Create("Cave")
            });

            ////////////////// ToraldWin
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("ToraldWorry");
            act.ID = "ToraldWin";
            GetObject("Cave").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("ToraldSkillCheck").SetText("AnswerToraldWTF").SetCallData("ToraldSkillCheck"));
            act.list.Add(ActionButtonInfo.Create("AnswerTorald2").SetText("Okaaay").SetType(ActionType.Close));
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
