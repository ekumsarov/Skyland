using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class Narkopip : GameEvent
    {

        public override void Init()
        {
            this.ID = "Narkopip";
            Simple = false;
            initialized = false;

            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Beginsuck");
            act.ID = "FirstDialogue";
            GetObject("tower").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("PIp")
                .SetText("Kopin")
                .SetCallData("Questnarko")
                );
            act.list.Add(ActionButtonInfo.Create("Bullpip")
                .SetText("TIK")
                .SetCallData("WTFman")
                );
            act.list.Add(ActionButtonInfo.Create("Ignorpip")
                .SetText("Ignor")
                .SetType(ActionType.Close)
                );

            //////bullpip

            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Questnarkopip");
            act.ID = "WTFman";
            GetObject("forest").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Bitthim").SetText("ARG").SetCallData("Fight"));
            act.list.Add(ActionButtonInfo.Create("Angryanswer").SetText("Answerang").SetCallData("Baddialog"));

            GetObject("forest14").AddAction(SkillCheckAction.Make("Ulka", "Fight", "Praval",
                ResultID.Create().SetSuccesID("Wewinpip").SetFailID("Pipwin"),
                new List<SkillCheckObject>()
                {
                    SkillCheckObject.Create("strenght", 6, 2)
                }
                ));

            //////after fight when we lose

            GetObject("forest14").Activity.PushPack("Pipwin", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "forest14", timeMode:TooltipTimeMode.Click, Text:"Welose"),
                ReactLock.Create("forest14")
            });
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Afterfight");
            act.ID = "Makl";
            GetObject("forest").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Ignoragain").SetText("Fuckyou").SetType(ActionType.Close));
            act.list.Add(ActionButtonInfo.Create("Letstalk").SetText("Talk").SetCallData("Questbadgood"));

            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("dialog");
            act.ID = "Questbadgood";
            GetObject("forest").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Goodwariaant").SetText("Yeeeeee").SetCallData("Questzele"));



            //////after fight when we win






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