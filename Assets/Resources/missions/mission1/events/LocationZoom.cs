using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class LocationZoom : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'InitCanvas' }"),
               Make("{ 'Base':'HideMenu' }"),
               Make("{ 'Base':'PerpareCutSene' }"),
               Make("{ 'Base':'ActiveStat', 'ID':'Stone', 'active':'false' }"),
               Make("{ 'Base':'ActiveStat', 'ID':'Skystone', 'active':'false' }"),
               Make("{ 'Base':'ActiveStat', 'ID':'Food', 'active':'false' }"),
               Make("{ 'Base':'ActiveStat', 'ID':'Unit', 'active':'false' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Unit', 'value':'0' }"),
               Make("{ 'Base':'SetupStat', 'ID':'Wood', 'value':'0' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'Add', 'flag':'FirstStepComplete' }"),
               Make("{ 'Base':'PlayerInit', 'City':'false', 'HeroIsland':'island_14'}"),
               Make("{ 'Base':'AddEvent', 'ID':'Nomads', 'To':'NomadVillage' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'DisableAfterIslandOpen' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'gotii' }"),
               Make("{ 'Base':'CreateObject', 'ID':'tower' }"),
               Make("{ 'Base':'CreateObject', 'ID':'Village' }"),
               Make("{ 'Base':'CreateObject', 'ID':'Forest' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'tavern', 'Icon':'city_hall_lvl_1', 'Type':'SubLocation', 'ObjectID':'tower' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'casarm', 'Icon':'casarm_lvl_1', 'Type':'SubLocation', 'ObjectID':'tower' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'torald', 'Icon':'torald_icon', 'Type':'Object', 'ObjectID':'tower' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'asist', 'Icon':'asist_icon', 'Type':'Object', 'ObjectID':'tavern' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'archer', 'Icon':'archerIcon', 'Type':'Object', 'ObjectID':'tavern' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'leg', 'Icon':'legionerIcon', 'Type':'Object', 'ObjectID':'casarm' }"),
               Make("{ 'Base':'CreateIcon', 'ID':'pers', 'Icon':'persIcon', 'Type':'Object', 'ObjectID':'casarm' }"),
               Make("{ 'Base':'SetupMainEvent', 'ID':'torald', 'MainEvent':'Greeting' }"),
               Make("{ 'Base':'AddEvent', 'ID':'IconTestPack', 'To':'torald' }"),
               Make("{ 'Base':'CallEvent', 'ID':'IconTestPack', 'To':'torald' }"),
               Make("{ 'Base':'ShowMenu' }"),
                Make("{ 'Base':'CallEditorPacks' }"),
                Make("{ 'Base':'CompleteCutScene' }"),
                Make("{ 'Base':'MoveCamera', 'to':'island_0', 'fast':'false', 'time':'5' }"),
                Make("{ 'Base':'ZoomIsland', 'ID':'island_0' }")
            };
        }
    }
}