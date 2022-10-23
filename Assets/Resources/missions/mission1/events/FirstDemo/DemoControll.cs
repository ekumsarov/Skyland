using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;
using BuildTypes;


namespace GameEvents1
{
    public class DemoControll : GameEvent
    {

        Subscriber subscriber;
        

        public override void Init()
        {
            this.ID = "DemoControll";

            Simple = false;

            subscriber = Subscriber.Create(this);

            subscriber.AddEvent("BuildComplete", "SanctuaryLumberjack");
            subscriber.AddEvent("FoodChanged");
            subscriber.AddEvent("UnitChanged");

            IconObject temp = GM.GetIcon("SanctuaryMainIcon");
            temp.AddAction(SkillCheckAction.Make("TestSkillCheck", "TestSkillCheck", "fail", ResultID.Create().SetSuccesID("StandartAction").SetFailID("StandartAction"), new List<SkillCheckObject>() { SkillCheckObject.Create("strenght", 10, 2) }));
            temp.GetAction("StandartAction").AddChoice(ActionButtonInfo.Create("DemoFight").SetCallData("AnimalFightAtHome").SetType(ActionType.Pack).SetAvailableCondition(StatCondition.Make("Wood", 4, StatCondition.StatConType.More)).SetAvailableCondition(StatCondition.Make("Stone", 20, StatCondition.StatConType.More)));
            temp.GetAction("StandartAction").AddChoice(ActionButtonInfo.Create("DemoFight2").SetCallData("AnimalFightAtHome").SetType(ActionType.Pack));
            temp.GetAction("StandartAction").AddChoice(ActionButtonInfo.Create("DemoFight3").SetCallData("TestSkillCheck").SetType(ActionType.SkillCheck));
            temp.GetAction("StandartAction").AddChoice(ActionButtonInfo.Create("DemoFight4").SetCallData("AnimalFightAtHome").SetType(ActionType.Pack));
            temp.Group.AddNewHero("StrangeAnimal1");
            temp.Group.AddNewHero("StrangeAnimal2");
            temp.Activity.PushPack("AnimalFightAtHome", new List<GameEvent>()
            {
                BattleEvent.Create("Player", temp.ID, ResultID.Create().SetFailCallback(PlayerLoose).SetSuccessCallback(PlayerWon), null)

            });

            initialized = false;
        }

        public void PlayerLoose()
        {
            SM.Stats["PlayerInfection"].Count += 5;
        }

        public void PlayerWon()
        {
            int amountOfSkins = UnityEngine.Random.Range(1, 3);
            SM.AddStat(amountOfSkins, "AnimalSkin");
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

        public void SetupQuests()
        {
            QS.AddQuest(QuestNode.Create("RepairBoat").SetIcon("GearIcon").SetDescription("RepairBoatDes").SetTitle("RepairBoat"));
            QS.AddQuest(QuestNode.Create("FindEngineer").SetIcon("FindIcon").SetDescription("FindEngineerDes").SetTitle("FindEngineer"));
            End();
        }

        #region ForestersProblemsQuest
        List<BuildCell> _lumbers;
        MapLocationIcon _sanIcon;
        public void BuildComplete()
        {
            subscriber.RemoveEvent("BuildComplete");
            ExpiredDay.ExpiredAfterTicks(UnityEngine.Random.Range(6, 18), act: ForestersProblems);
        }

        public void ForestersProblems()
        {
            if (_lumbers == null)
                _lumbers = new List<BuildCell>();

            BuildPlace build = GetObject("Sanctuary") as BuildPlace;
            if (build != null)
            {
                foreach (var cell in build._cells)
                {
                    if (cell.State == BuildState.bs_Active && cell.Info.Name.Equals("Lumberjack"))
                    {
                        if (!_lumbers.Contains(cell))
                        {
                            cell.State = BuildState.bs_Sleep;
                            cell._buildType.StopProduction();
                            _lumbers.Add(cell);
                        }
                    }
                }

                MapLocationIcon icon = GM.GetIcon("SanctuaryMapIcon") as MapLocationIcon;
                if (icon != null)
                {
                    _sanIcon = icon;
                    _sanIcon.AfterZoomPack = "ForesterProblems";
                    _sanIcon.Activity.PushPack("ForesterProblems", new List<GameEvent>()
                    {
                        ShowTooltip.Create(Vector3.zero, "Player", Text:"ForesterProblems1"),
                        ShowTooltip.Create(Vector3.zero, "Player", Text:"ForesterProblems2"),
                        ShowTooltip.Create(Vector3.zero, "Player", Text:"ForesterProblems3"),
                        LockObject.Create("Woodcutter", false),
                        QuestWork.Create("WolfQuest", nQuest:QuestNode.Create("WolfQuest").SetDescription("WolfQuestDes").SetIcon("WolfIcon").SetTitle("WolfQuestTitle")),
                        MapIconAdditionalEvent.Create("SanctuaryMapIcon", ""),
                        OnlyScriptCallDelegate.Create(OnWolfQuestComplete)
                    });
                }
            }
        }

        public void OnWolfQuestComplete(Action end)
        {
            subscriber.AddEvent("CompleteQuest", "WolfQuest");
            end?.Invoke();
        }

        public void CompleteQuest()
        {
            foreach (var lum in _lumbers)
            {
                lum.State = BuildState.bs_Active;
                lum._buildType.ActivateProduction();
            }

            MapLocationIcon icon = GM.GetIcon("SanctuaryMapIcon") as MapLocationIcon;
            if (icon != null)
            {
                _sanIcon = icon;
                _sanIcon.AfterZoomPack = "ForesterProblemsResolved";
                _sanIcon.Activity.PushPack("ForesterProblemsResolved", new List<GameEvent>()
                    {
                        ShowTooltip.Create(Vector3.zero, "Player", Text:"ForesterProblemsResolved1"),
                        MapIconAdditionalEvent.Create("SanctuaryMapIcon", string.Empty)
                        
                    });
            }

            _sanIcon = null;
            _lumbers.Clear();
            _lumbers = null;
        }
        #endregion

        #region UnitHungryController - Prototype
        BuildPlace _sanctuary;
        List<BuildCell> _sleepingBuilds;
        bool hungryAgain = true;
        public void FoodChanged()
        {
            if (_sanctuary == null)
                _sanctuary = GetObject("Sanctuary") as BuildPlace;

            if (_sleepingBuilds == null)
                _sleepingBuilds = new List<BuildCell>();

            if (SM.Stats["Food"].Count <= 0 && hungryAgain)
            {
                if(_sanctuary != null)
                {
                    foreach(var cell in _sanctuary._cells)
                    {
                        if(cell.State == BuildState.bs_Active)
                        {
                            if(!_sleepingBuilds.Contains(cell))
                            {
                                bool haveFood = false;
                                foreach (var stat in cell.Info.Consumtion)
                                {
                                    if (stat.type.Equals("Food"))
                                    {
                                        if (stat.amount > 0)
                                        {
                                            haveFood = true;
                                            break;
                                        }
                                    }
                                }

                                if(haveFood)
                                {
                                    cell.State = BuildState.bs_Sleep;
                                    cell._buildType.StopProduction();
                                    _sleepingBuilds.Add(cell);
                                    hungryAgain = false;
                                    ExpiredDay.ExpiredAfterTicks(6, act: CanBeHungryAgain);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    subscriber.RemoveEvent("FoodChanged");
                    Debug.LogError("Notfoundsanctuary");
                }
            }
            else if(SM.Stats["Food"].Count > 20 && _sleepingBuilds.Count>0)
            {
                foreach(var cell in _sleepingBuilds)
                {
                    if (_lumbers != null && _lumbers.Contains(cell))
                        continue;

                    cell.State = BuildState.bs_Active;
                    cell._buildType.ActivateProduction();
                }

                _sleepingBuilds.Clear();
                hungryAgain = true;
            }
        }

        public void CanBeHungryAgain()
        {
            hungryAgain = true;
        }

        #endregion

        #region NeedUnits

        public void UnitChanged()
        {
            if(SM.Stats["Unit"].Count <= 1)
            {
                subscriber.RemoveEvent(SM.Stats["Unit"].CallFunction);
                UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "UnitIsEnding", lSize:45);
                BuildPlace center = GM.GetObject("Sanctuary") as BuildPlace;
                if(center != null)
                {
                    center.AddAvaliable("ScoutHut");
                    UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "WeNeedScouthut", lSize:45);
                }
            }
        }

        #endregion
    }
}