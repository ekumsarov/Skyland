using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class ForestInvasion : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'MoveCamera', 'to':'player', 'fast':'true' }"),
               Make("{ 'Base':'CompleteCutScene' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'asist_icon', 'text':'ForestInvasion1' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion2' }"),
               Make("{ 'Base':'Dialogue', 'text':'ForestInvasion3' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion4' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion5' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion6' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion7' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion8' }"),
               Make("{ 'Base':'OpenMenu', 'MenuID':'MapMenu' }"),
               Make("{ 'Base':'PlayUIAnimation', 'animationID':'islandPulse', 'To':'MapObject5', 'animation': { 'type':'Scale', 'offset': { 'x':'0,12' }, 'time':'0,3', 'loop': { 'count':'4', 'pingpong':'true' } } }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion9' }"),
               Make("{ 'Base':'CloseMenu', 'MenuID':'MapMenu' }"),
               Make("{ 'Base':'Dialogue', 'IconImage':'gerold', 'text':'ForestInvasion10' }"),
               Make("{ 'Base':'AddPack', 'ID':'GetLogsComplete', 'To':'forest14' }"),
               Make("{ 'Base':'ReactButSetup', 'To':'forest14', 'ID':'GetLogsComplete', 'timer':'4', 'Ico':'Axe', 'TooltipStr':'GetLogsQuestTitle', 'animation':'s_Work' }"),
               Make("{ 'Base':'QuestBut', 'To':'forest14', 'ID':'GetLogs', 'Ico':'log', 'TooltipStr':'GetLogs', 'MakeAction':'true', 'Title':'GetLogsQuestTitle', 'Des':'GetLogsQuestDes', 'Reward':'GetLogsQuestReward' }")
            };
        }
    }
}
/*
 * 
 */