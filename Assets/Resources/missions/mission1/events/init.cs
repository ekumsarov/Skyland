using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class init : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'InitCanvas' }"),
               Make("{ 'Base':'HideMenu' }"),
               Make("{ 'Base':'PerpareCutSene' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Metal', 'value':'0' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Stone', 'value':'0' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Unit', 'value':'5' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Wood', 'value':'25' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Skystone', 'value':'15' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Food', 'value':'30' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'Add', 'flag':'FirstStepComplete' }"),
               Make("{ 'Base':'PlayerInit', 'City':'false', 'HeroIsland':'island_14'}"),
               Make("{ 'Base':'AddEvent', 'ID':'PlayerInfection', 'To':'Player', 'Activate':'true' }"),
               Make("{ 'Base':'AddEvent', 'ID':'DemoControll', 'Activate':'true' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'DisableAfterIslandOpen' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'gotii' }"),

               Make("{ 'Base':'LockObject', 'ID':'Town', 'Activate':'false' }"),
               Make("{ 'Base':'LockObject', 'ID':'Sanctuary', 'Activate':'false' }"),
               Make("{ 'Base':'LockObject', 'ID':'ForestPlace', 'Activate':'true' }"),
               Make("{ 'Base':'LockObject', 'ID':'Woodcutter', 'Activate':'true' }"),

               Make("{ 'Base':'ActivateObject', 'ID':'Town' }"),
               Make("{ 'Base':'ActivateObject', 'ID':'Sanctuary' }"),

               Make("{ 'Base':'CreateIcon', 'ID':'Wolf', 'Icon':'WolfIcon', 'MainEvent':'WolfAction', 'IsActive':'false', 'ObjectID':'WolfPlace' }"),
               Make("{ 'Base':'AddEvent', 'ID':'WolfQuestEvent', 'To':'Wolf', 'Activate':'true' }"),

               Make("{ 'Base':'CreateIcon', 'ID':'VillageHeadman', 'Icon':'Pesant_1', 'MainEvent':'HeadnmanGreetings', 'IsActive':'true', 'ObjectID':'HeadmanPlace' }"),
               Make("{ 'Base':'AddEvent', 'ID':'VillageHeadmanEvent', 'To':'VillageHeadman', 'Activate':'true' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'VillageTrader', 'Icon':'Megaball_2', 'MainEvent':'StandartAction', 'IsActive':'true', 'ObjectID':'VillageTraderPlace' }"),
               Make("{ 'Base':'AddEvent', 'ID':'VillageTraderEvent', 'To':'VillageTrader', 'Activate':'true' }"),
               Make("{ 'Base':'AddEvent', 'ID':'VillageEvents', 'To':'MyVillage', 'Activate':'true' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'VillageKeeper', 'Icon':'Megaball_3', 'MainEvent':'VillageKeeperStandart', 'IsActive':'true', 'ObjectID':'VillageStorehouse' }"),

               Make("{ 'Base':'MapIconAdditionalEvent', 'ID':'CaveMapIcon', 'EventID':'TrollStart' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'MyTroll', 'Icon':'TollIcon', 'MainEvent':'StandartAction', 'IsActive':'true', 'ObjectID':'TrollPlace' }"),
               Make("{ 'Base':'AddEvent', 'ID':'MyTrollEvent', 'To':'MyTroll', 'Activate':'true' }"),

               Make("{ 'Base':'CreateIcon', 'ID':'CaveToTown', 'Icon':'BaseIcon', 'MainEvent':'StartGoToTown', 'IsActive':'false', 'ObjectID':'TrollCave' }"),
               Make("{ 'Base':'AddEvent', 'ID':'CaveTownEvent', 'To':'CaveToTown', 'Activate':'true' }"),

               Make("{ 'Base':'CreateIcon', 'ID':'StrangeAnimals', 'Icon':'DiamondICon', 'IsActive':'true', 'ObjectID':'ForestPlace' }"),
               Make("{ 'Base':'AddEvent', 'ID':'StrangeAnimalsEvent', 'To':'StrangeAnimals', 'Activate':'true' }"),

               Make("{ 'Base':'CreateIcon', 'ID':'SentryGuard', 'Icon':'SentryGuard', 'MainEvent':'SentryFirstMeet', 'IsActive':'true', 'ObjectID':'Sentry' }"),
               Make("{ 'Base':'AddEvent', 'ID':'SentryGuardsEvent', 'To':'SentryGuard', 'Activate':'true' }"),

               Make("{ 'Base':'MapIconAdditionalEvent', 'ID':'TownMapIcon', 'EventID':'StartCity' }"),
               Make("{ 'Base':'ShowMenu' }"),
               Make("{ 'Base':'CallEditorPacks' }"),
               Make("{ 'Base':'CallPack', 'ID':'StartScene' }")
                
                //Make("{ 'Base':'ZoomIsland', 'ID':'island_0' }")
            }; 
        }
    }
}