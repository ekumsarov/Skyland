using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class NephewDanger : GameEvent
    {
        public int TaskType = 1;

        public override void Init()
        {
            this.ID = "NephewDanger";

            Simple = false;

            initialized = false;

            this.Object.AddAction(Actions.Create("Context", "NomadNephew")
                .AddChoice(ActionButtonInfo.Create("WolfThreat")
                    .SetText("WolfThreatNephew")
                    .SetCallData("WolfThreat")
                    .SetAppearCondition(FlagCondition.Make("FWolfFeed"))
                    .SetAppearCondition(FlagCondition.Make("FailNomadThreat", false)))

                .AddChoice(ActionButtonInfo.Create("PowerThreat")
                    .SetCallData("PowerThreat")
                    .SetAppearCondition(FlagCondition.Make("FWolfFeed", false))
                    .SetAppearCondition(FlagCondition.Make("FailNomadThreat", false)))

                .AddChoice(ActionButtonInfo.Create("RecogniseFromUncle")
                    .SetText("RecogniseFromUncleNephew")
                    .SetCallData("RecogniseFromUncle")
                    .SetType(ActionType.Pack)
                    .SetAppearCondition(FlagCondition.Make("FirstFoundForester"))
                    .SetAppearCondition(FlagCondition.Make("FailNomadThreat", false))
                    .SetAppearCondition(FlagCondition.Make("FailBuyout", false)))

                .AddChoice(ActionButtonInfo.Create("UnderstandRequest")
                    .SetText("UnderstandRequestNephew")
                    .SetCallData("UnderstandRequest")
                    .SetType(ActionType.Pack)
                    .SetAppearCondition(FlagCondition.Make("FirstFoundForester", false))
                    .SetAppearCondition(FlagCondition.Make("FailNomadThreat", false))
                    .SetAppearCondition(FlagCondition.Make("FailBuyout", false)))

                .AddChoice(ActionButtonInfo.Create("AttackNomad")
                    .SetText("AttackNomad")
                    .SetCallData("AttackNomad")
                    .SetType(ActionType.Pack))

                .AddChoice(ActionButtonInfo.Create("Buyout")
                    .SetText("BuyoutNephew")
                    .SetCallData("BuyoutCheck")
                    .SetAppearCondition(FlagCondition.Make("FailBuyout", false)))
            );

            /*
             * Threat setup
             */

            Object.AddAction(SkillCheckAction.Make("WolfThreat", "WolfThreat", "",
                res: ResultID.Create().SetSuccesID("ThreatSuccess").SetFailCallback(ThreatFail),
                new List<SkillCheckObject>() { SkillCheckObject.Create("strenght", 4, 2) })
            );

            Object.AddAction(SkillCheckAction.Make("PowerThreat", "PowerThreat", "",
                res: ResultID.Create().SetSuccesID("ThreatSuccess").SetFailCallback(ThreatFail),
                new List<SkillCheckObject>() { SkillCheckObject.Create("strenght", 5, 3), SkillCheckObject.Create("charisma", 4, 1) })
            );

            this.Object.Activity.PushPack("ThreatSuccess", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadAfraid"),
                FlagWork.Create("FoundNephew", "On"),
                CallEvent.Create(Event: "{ 'Event':'NomadCheckBuild', 'param':{ 'CallID':'NomadAngry' } }"),
                CallPack.Create("QuestClean", "self")
            });


            /*
             * Buyout setup
             */

            Object.AddAction(SkillCheckAction.Make("BuyoutCheck", "BuyoutCheck", "",
                res: ResultID.Create().SetSuccesID("BuyoutSuccess").SetFailCallback(BuyoutFail),
                new List<SkillCheckObject>() { SkillCheckObject.Create("charisma", 6, 2) })
            );

            this.Object.Activity.PushPack("BuyoutSuccess", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero,this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadBuyoutEnd"),
                FlagWork.Create("FoundNephew", "On"),
                CallEvent.Create(Event: "{ 'Event':'NomadCheckBuild', 'param':{ 'CallID':'NomadBuyout' } }"),
                CallPack.Create("QuestClean", "self")
            });

            Object.AddAction(Actions.Create("Context", "BuyoutSuccess")
                .AddChoice(ActionButtonInfo.Create("OfferFood").SetCallData("NoFood").SetType(ActionType.Pack).SetAppearCondition(FlagCondition.Make("NoFoodNephew", false)))
                .AddChoice(ActionButtonInfo.Create("OfferService").SetCallData("AgreeService").SetType(ActionType.Pack))
                .AddChoice(ActionButtonInfo.Create("OfferSkystone").SetCallData("AgreeSkystone").SetType(ActionType.Pack))
            );

            this.Object.Activity.PushPack("NoFood", new List<GameEvent>()
            {
                FlagWork.Create("NoFoodNephew", "On"),
                ActionWork.Create("BuyoutSuccess", "self", null, _WorkType:"ChangeText", text:"NoFoodNephew"),
                CallAction.Create("BuyoutSuccess")
            });

            this.Object.Activity.PushPack("AgreeService", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "OfferServiceEnd"),
                FlagWork.Create("FoundNephew", "On"),
                CallEvent.Create(Event: "{ 'Event':'NomadCheckBuild', 'param':{ 'CallID':'NomadService' } }"),
                CallPack.Create("QuestClean", "self")
            });

            this.Object.Activity.PushPack("AgreeSkystone", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadBuyoutEnd"),
                FlagWork.Create("FoundNephew", "On"),
                CallEvent.Create(Event: "{ 'Event':'NomadCheckBuild', 'param':{ 'CallID':'NomadBuyout' } }"),
                CallPack.Create("QuestClean", "self")
            });

            /*
             * Attack setup
             */

            this.Object.Activity.PushPack("AttackNomad", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadAngry"),
                BattleEvent.Create("Player", "Setup", ResultID.Create().SetSuccesID("NomadBeat").SetFailID("NomadWon"), isl:"island_15", enemyStack: new List<string>() { "LonelyNomad" })
            });

            this.Object.Activity.PushPack("AttackNomadFailed", new List<GameEvent>() {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadAngryFailed"),
                BattleEvent.Create("Player", "Setup", ResultID.Create().SetSuccesID("NomadBeat").SetFailID("NomadWon"), isl:"island_15", enemyStack: new List<string>() { "LonelyNomad" })
            });

            this.Object.Activity.PushPack("NomadBeat", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadAngryFailed"),
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NephewThanks"),
                FlagWork.Create("FoundNephew", "On"),   
                CallPack.Create("QuestClean", "self")
            });

            this.Object.Activity.PushPack("NomadWon", new List<GameEvent>()
            {
                ShowTooltip.Create(Vector3.zero, this.Object.ID, exTime: 1.0f, timeMode: TooltipTimeMode.Click, Text: "NomadAngryFailed"),
                FlagWork.Create("LostNephew", "On"),
                CallPack.Create("QuestClean", "self")
            });



            this.Object.Activity.PushPack("RecogniseFromUncle", new List<GameEvent>() {
                ActChoiceWork.Create("self", "NomadNephew", "RecogniseFromUncle", _ActType:"Remove"),
                ActionWork.Create("NomadNephew", "self", null, _WorkType:"ChangeText", text:""),
                ActionWork.Create("PowerThreat", "self", null, _WorkType:"Replace",
                    act:
                    SkillCheckAction.Make("PowerThreat", "PowerThreat", "",
                    res: ResultID.Create().SetSuccesID("ThreatSuccess").SetFailCallback(ThreatFail),
                    new List<SkillCheckObject>() { SkillCheckObject.Create("strenght", 7, 3), SkillCheckObject.Create("charisma", 8, 1) })),
                CallAction.Create("NomadNephew")
            });

            this.Object.Activity.PushPack("UnderstandRequest", new List<GameEvent>() {
                ActChoiceWork.Create("self", "NomadNephew", "UnderstandRequest", _ActType:"Remove"),
                ActionWork.Create("NomadNephew", "self", null, _WorkType:"ChangeText", text:"UnderstandRequestAnswer"),
                FlagWork.Create("KnowSecret", "On"),
                CallAction.Create("NomadNephew")
            });

            this.Object.Activity.PushPack("QuestClean", new List<GameEvent>()
            {
                DestroyObject.Create("LonelyNomad"),
                ReactLock.Create("LonelyNomad"),
                FlagWork.Create("FailNomadThreat", "Remove")
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

            SM.SetFlag("FoundNephew");

            this.Object.CallAction("NomadNephew");

            End();
        }

        public void ThreatFail()
        {
            SM.SetFlag("FailNomadThreat", true);

            if (SM.CheckFlag("FailBuyout") == true)
                Object.Actioned("AttackNomadFailed");
            else
                Object.Actioned("NomadNephew");
        }

        public void BuyoutFail()
        {
            SM.SetFlag("FailBuyout", true);

            if (SM.CheckFlag("FailNomadThreat") == true)
                Object.Actioned("AttackNomadFailed");
            else
                Object.Actioned("NomadNephew");
        }

    }
}