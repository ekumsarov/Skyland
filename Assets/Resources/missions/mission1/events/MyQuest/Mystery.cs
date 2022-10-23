using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class Mystery : GameEvent
    {

        public override void Init()
        {
            this.ID = "Mystery";
            Simple = false;
            initialized = false;

            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Darktower");
            act.ID = "FirstDialogue";
            GetObject("tower").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Buttower").SetText("WTFsecret").SetCallData("Questenigma"));
            act.list.Add(ActionButtonInfo.Create("Buttower2").SetText("horrible").SetType(ActionType.Close));
            act.list.Add(ActionButtonInfo.Create("Buttower3").SetText("Itneresting").SetCallData("Somequestions"));

            ////////answer
            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("enigma");
            act.ID = "Questenigma";
            GetObject("tower").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Firstanswer").SetText("Nottrue1").SetType(ActionType.Close));
            act.list.Add(ActionButtonInfo.Create("Secondanswer").SetText("Nottrue2").SetType(ActionType.Close));
            act.list.Add(ActionButtonInfo.Create("Thirdanswer").SetText("Right").SetCallData("YRA").SetType(ActionType.Pack));

            GetObject("tower").Activity.PushPack("YRA", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "tower", timeMode:TooltipTimeMode.Click, Text:"Gongat"),
                LootWork.Create("Rubin", LS.LootType.Amulet, "Player"),
                ReactLock.Create("tower"),
                TimerCall.Create("tower", ticks:3, callback:NewDay)
            });

            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("questions");
            act.ID = "Somequestions";
            GetObject("tower").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Whathapen").SetText("Whathapen1").SetCallData("Thegame").SetType(ActionType.Pack));
            act.list.Add(ActionButtonInfo.Create("Ok").SetText("Iagree").SetCallData("Questenigma"));

            GetObject("tower").Activity.PushPack("Thegame", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "tower", timeMode:TooltipTimeMode.Click, Text:"QuestEnd"),
                ReactLock.Create("tower")
            });

            GetObject("tower").Activity.PushPack("NewDay", new List<GameEvent>()
            {
                ReactLock.Create("tower", false)
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

        public void NewDay()
        {
            SceneObject tower = GetObject("tower") as SceneObject;
            tower.LockLocation(true);
        }


    }
} 
