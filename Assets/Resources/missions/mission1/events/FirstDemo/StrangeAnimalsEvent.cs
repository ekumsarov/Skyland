using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents1
{
    public class StrangeAnimalsEvent : GameEvent
    {

        Subscriber subscriber;
        SkillCheckObject _findAnimalsCheck;
        bool _canHunt;
        int huntLevel = 0;

        bool gotBoss;
        bool firstTime = true;

        bool getWhisle = false;
        bool getBow = false;

        public override void Init()
        {
            this.ID = "StrangeAnimalsEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            _canHunt = true;

            Stat.Create("AnimalSkin", icon: "Medal1", mainStat: true, changeOnEmpty: true);
            Stat.Create("AnimalThooth", icon: "Medal2", mainStat: true, changeOnEmpty: true);

            Object.MainEvent = "StandartAction";

            _findAnimalsCheck = SkillCheckObject.Create("dexterity", 6);
            Object.Group.RemoveAllHeroes();

            Object.AddAction(Actions.Create("Context", "StandartAction").SetText("TryFindStrangeAnimals")
                .AddChoice(
                    ActionButtonInfo.Create("StartFindStrangeAnimals").SetCallback(FindForHunt).SetAppearCondition(FunctionCondition.Create(CheckForHunt)))
                .AddChoice(ActionButtonInfo.Create("WhisleAnimal").SetCallback(CallBoss).SetAppearCondition(LootCondition.Make("AnimalWhisle")))
                .AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close)));

            Object.Activity.PushPack("AnimalBattle", new List<GameEvent>()
            {
                BattleEvent.Create("Player", Object.ID, ResultID.Create().SetFailCallback(PlayerLoose).SetSuccessCallback(PlayerWon), DrawDelegate:Draw)
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

        public bool CheckForHunt()
        {
            return _canHunt;
        }

        public void CallBoss()
        {
            UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Instantly, TooltipObject.Game, "NotReady", time: 2f);
        }

        public void FindForHunt()
        {
            _findAnimalsCheck.CompleteCheck(GM.Player.Group.GetUnits());
            
            huntLevel = _findAnimalsCheck.ResultaAmount;
            int tempHuntLevel = huntLevel;
            if (huntLevel == 0)
            {
                UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "NotFoundStrangeAnimals");
                _canHunt = false;
                Object.ChangeActionText("StandartAction", "CantFindStrangeAnimals");
                ExpiredDay.ExpiredAfterTicks(6, act: CanHuntAgain);
                return;
            }
            else if (huntLevel == 1)
            {
                Object.Group.RemoveAllHeroes();
                Object.Group.AddNewHero("StrangeAnimal1");
                if (UnityEngine.Random.Range(0, 100) > 60)
                {
                    Object.Group.AddNewHero("StrangeAnimal2");
                    tempHuntLevel += 1;
                }

            }
            else if (huntLevel >= 2)
            {
                Object.Group.RemoveAllHeroes();
                Object.Group.AddNewHero("StrangeAnimal1");
                Object.Group.AddNewHero("StrangeAnimal2");
                
                if (UnityEngine.Random.Range(0, 100) > 70)
                {
                    Object.Group.AddNewHero("StrangeAnimal3");
                    Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
                    Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");
                    Object.Group.RemoveAction("StrangeAnimal3", "AnimalRetreat");
                    tempHuntLevel += 1;
                }

                if (UnityEngine.Random.Range(0, 100) > 40)
                {
                    Object.Group.AddNewHero("StrangeAnimalBig");
                    Object.Group.RemoveAction("StrangeAnimal1", "AnimalRetreat");
                    Object.Group.RemoveAction("StrangeAnimal2", "AnimalRetreat");
                    Object.Group.RemoveAction("StrangeAnimal3", "AnimalRetreat");
                    tempHuntLevel += 2;
                }

            }
            huntLevel = tempHuntLevel;

            Object.Activity.callActivityPack("AnimalBattle");
        }

        public void CanHuntAgain()
        {
            Object.ChangeActionText("StandartAction", "TryFindStrangeAnimals");
            _canHunt = true;
        }

        public void PlayerWon()
        {
            if (huntLevel == 1)
            {
                int amountOfSkins = UnityEngine.Random.Range(1, 3);
                SM.AddStat(amountOfSkins, "AnimalSkin");

            }
            else if (huntLevel == 2)
            {
                int amountOfSkins = UnityEngine.Random.Range(2, 7);
                SM.AddStat(amountOfSkins, "AnimalSkin");
            }
            else if (huntLevel == 3)
            {
                int amountOfSkins = UnityEngine.Random.Range(3, 10);
                SM.AddStat(amountOfSkins, "AnimalSkin");
                int amountOfThooth = UnityEngine.Random.Range(0, 3);
                SM.AddStat(amountOfThooth, "AnimalThooth");
            }
            else if (huntLevel >= 4 && huntLevel <= 6)
            {
                int amountOfSkins = UnityEngine.Random.Range(4, 13);
                SM.AddStat(amountOfSkins, "AnimalSkin");
                int amountOfThooth = UnityEngine.Random.Range(0, 5);
                SM.AddStat(amountOfThooth, "AnimalThooth");

                if (getBow == false)
                {
                    if (UnityEngine.Random.Range(0, 101) > 75)
                    {
                        getBow = true;
                        LS.AddItem("AnimalBow", LS.LootType.SecondWeapon);
                    }
                }

                if (getWhisle == false)
                {
                    if (UnityEngine.Random.Range(0, 101) > 85)
                    {
                        getWhisle = true;
                        LS.AddItem("AnimalWhisle", LS.LootType.Extra);
                    }
                }
            }

            if (firstTime)
            {
                UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Type, TooltipObject.Game, "GetFirstAnimalSkin", time: 1.5f);
            }
        }

        public void PlayerLoose()
        {
            SM.Stats["PlayerInfection"].Count += 5;
        }

        public void Draw()
        {
            UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Type, TooltipObject.Game, "AllAnimalsGone", time: 1.5f);
        }
    }
}