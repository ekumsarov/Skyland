using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class MyTrollEvent : GameEvent
    {

        Subscriber subscriber;
        bool trollFirstClick = true;

        public override void Init()
        {
            this.ID = "MyTrollEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            Object.Activity.PushPack("EatMushrooms", new List<GameEvent>()
                {
                    PerpareCutSene.Create(),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"EatMushrooms1"),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"EatMushrooms2"),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"EatMushrooms3"),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"EatMushrooms4"),
                    CompleteCutScene.Create(),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"TrollRequestFarm"),
                });
            Object.Activity.PushPack("NotEatMushrooms", new List<GameEvent>()
                {
                    ShowTooltip.Create(UIM.RandomScreenPoint, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"NotEatMushrooms1"),
                    ShowTooltip.Create(UIM.ScreenCenter, "", 1.5f, timeMode:TooltipTimeMode.Click, Text:"TrollRequestFarm")
                });
            Stat.Create("TrollMushroom");

            MapLocationObject sanctuary = GM.GetObject("Sanctuary") as MapLocationObject;

            Object.Activity.PushPack("TrollWon", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, "MyTroll", Text:"TrollGoAway"),
                FadeScreen.Create(true, false),
                ActivateLocationOnIsland.Create(1, false),
                MoveIcon.Create("Player", sanctuary.GetQuitObject().ID),
                MoveCameraToPoint.Create(sanctuary.CameraPoint, true),
                SafeCallFunction.Create("OpenLocations", sanctuary.ID),
                FadeScreen.Create(false, false),
                SafeCallFunction.Create("CallAfterMovePack", sanctuary.ID)
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

        public void ClearMapIcon()
        {
            trollFirstClick = false;
            MapLocationIcon icon = GM.GetIcon("CaveMapIcon") as MapLocationIcon;
            if(icon != null)
            {
                icon.AfterZoomPack = "";
            }
        }

        public void EatMushrooms()
        {
            if (trollFirstClick)
                ClearMapIcon();

            GM.Player.Group.AddSkills("Player", "intellegence", 2, 8);
            Object.Activity.callActivityPack("EatMushrooms");

            End();
        }

        public void NotEatMushrooms()
        {
            if (trollFirstClick)
                ClearMapIcon();


            Object.Activity.callActivityPack("NotEatMushrooms");
            End();
        }

        public void WonFirstFight()
        {
            Object.Group.RestoreCurHP("Troll");
            trollFirstClick = false;
            MapLocationIcon icon = GM.GetIcon("CaveMapIcon") as MapLocationIcon;
            if (icon != null)
            {
                icon.AfterZoomPack = "";
                icon.Activity.PushPack("TrollAngryForFight", new List<GameEvent>()
                {
                    ShowTooltip.Create(UIM.ScreenCenter, "", Text:"TrollComeAgain"),
                    BattleEvent.Create("Player", "MyTroll", ResultID.Create().SetFailCallback(PlayerWon).SetSuccessCallback(TrollWon))
                    
                });
            }

            End();
        }

        public void KilledTroll()
        {
            if (trollFirstClick)
                ClearMapIcon();

            GM.GetIcon("MyTroll").Lock = true;
            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "WonTrollGG", gEvent:this);
            LS.AddItem("TrollHead", LS.LootType.Extra, "bag");
        }

        public void BuildFarm()
        {
            if(SM.Stats["Wood"].Count < 25 || SM.Stats["Food"].Count < 20 || SM.Stats["Unit"].Count < 1)
            {
                UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "TrollNoStatsFarm", gEvent: this);
                return;
            }

            SM.Stats["Wood"].Count -= 25;
            SM.Stats["Food"].Count -= 20;
            SM.Stats["Unit"].Count -= 1;
            ExpiredDay.ExpiredAfterTicks(6, act: StartProduction);
            End();
        }

        public void StartProduction()
        {
            SM.Stats["Units"].Count += 1;
            ExpiredDay.ExpiredAfterTicks(12, act: MushrommProduction);
        }

        public void MushrommProduction()
        {
            SM.Stats["TrollMushroom"].Count += (int)UnityEngine.Random.Range(1, 7);
            ExpiredDay.ExpiredAfterTicks(12, act: MushrommProduction);
        }

        public void SoldMushroom()
        {
            if(SM.Stats["TrollMushroom"].Count > 1)
            {
                int skyAmount = (int)(SM.Stats["TrollMushroom"].Count * 3);
                SM.Stats["Skystone"].Count += skyAmount;
                SM.Stats["TrollMushroom"].Count = 0;
                UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, LocalizationManager.Get("TrollMushroomReward", skyAmount), gEvent:this);
                return;
            }

            UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "TrollHotNoMushroom", gEvent:this);
        }

        public void TrollWon()
        {
            Object.Group.RestoreCurHP("MyTroll");
            Object.Activity.callActivityPack("TrollWon");
        }

        public void PlayerWon()
        {
            IconObject troll = Object as IconObject;
            troll.Visible = false;
            troll.Lock = true;
            troll.RemoveIcon();

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "WonTrollGG");
            QS.CompleteQuest("TrollQuest");
        }
    }
}