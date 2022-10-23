using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents1
{
    public class CaveTownEvent : GameEvent
    {

        Subscriber subscriber;
        int step = 1;

        public override void Init()
        {
            this.ID = "CaveTownEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            Object.MainEvent = "FirstBattle";
            Object.Group.RemoveAllHeroes();

            Object.Group.AddNewHero("StrangeAnimal1");
            Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
            Object.Group.AddNewHero("StrangeAnimal2");
            Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");

            Object.Activity.PushPack("FirstBattle", new List<GameEvent>()
            {
                ShowTooltip.Create(UIM.ScreenCenter, "Player", Text:"FirstStepInCave"),
                BattleEvent.Create("Player", Object.ID, ResultID.Create().SetFailCallback(PlayerLoose).SetSuccessCallback(PlayerWon))
            });

            Object.AddAction(Actions.Create("Context", "StandartAction")
                .AddChoice(
                    ActionButtonInfo.Create("StartCavePathToTown").SetCallback(StartPath))
                .AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close)));

            Object.Activity.PushPack("StartBattle", new List<GameEvent>()
            {
                BattleEvent.Create("Player", Object.ID, ResultID.Create().SetFailCallback(PlayerLoose).SetSuccessCallback(PlayerWon))
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

        public void StartPath()
        {
            step = 1;
            Object.Group.RemoveAllHeroes();

            Object.Group.AddNewHero("StrangeAnimal1");
            Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
            Object.Group.AddNewHero("StrangeAnimal2");
            Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");

            Object.Activity.callActivityPack("StartBattle");
            End();
        }

        public void PlayerWon()
        {
            if(step == 1)
            {
                step = 2;
                Object.Group.RemoveAllHeroes();

                Object.Group.AddNewHero("StrangeAnimal1");
                Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
                Object.Group.AddNewHero("StrangeAnimal2");
                Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");
                Object.Group.AddNewHero("StrangeAnimal3");
                Object.Group.RemoveAction("StrangeAnimal3", "AnimalRetreat");

                Object.Activity.callActivityPack("StartBattle");
            }
            else if(step == 2)
            {
                step = 3;
                Object.Group.RemoveAllHeroes();

                Object.Group.AddNewHero("StrangeAnimal1");
                Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
                Object.Group.AddNewHero("StrangeAnimal2");
                Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");
                Object.Group.AddNewHero("StrangeAnimal3");
                Object.Group.RemoveAction("StrangeAnimal3", "AnimalRetreat");
                Object.Group.AddNewHero("StrangeAnimalBig");

                Object.Activity.callActivityPack("StartBattle");
            }
            else if(step == 3)
            {
                UIM.FastFade();

                UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "YouAreInTownFromCave");
            }
        }

        public void PlayerLoose()
        {
            SM.Stats["PlayerInfection"].Count += 5;
        }
    }
}