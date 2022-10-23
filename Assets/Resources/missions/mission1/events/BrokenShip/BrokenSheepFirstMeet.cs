using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class BrokenSheepFirstMeet : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'AddEvent', 'ID':'Nomads', 'To':'NomadVillage' }"),
               Make("{ 'Base':'IslandState', 'ID':'island_14' }"),
               Make("{ 'Base':'IslandState', 'ID':'island_6', 'state':'Unexplored' }"),
               Make("{ 'Base':'IslandState', 'ID':'island_15', 'state':'Unexplored' }"),
               Make("{ 'Base':'InitMap'}"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'DisableAfterIslandOpen' }"),
               Make("{ 'Base':'FlagWork', 'ActType':'On', 'flag':'gotii' }"),
               Make("{ 'Base':'CreateObject', 'ID':'tower' }"),
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
                Make("{ 'Base':'CompleteCutScene' }")
            };
        }
    }
}