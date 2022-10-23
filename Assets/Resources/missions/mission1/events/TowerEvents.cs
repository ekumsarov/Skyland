using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class TowerEvents : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               //Tower2
               Make("{ 'Base':'CloneObject', 'ID':'tower', 'RenameID':'tower2', 'TurnReactPanel':'true', 'ReactButton':{ 'icon':'LookEye', 'timer':0, 'EventID':'MainAction' }, 'OnIsland':'island_2', 'Place':'Center' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore', 'To':'tower2' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2ExploreDone', 'To':'tower2' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore2', 'To':'tower2' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore2Done', 'To':'tower2' }"),
               Make(@"{ 'Base':'ActionWork', 'ActionType':'Context', 'Text':'Tower2Explore', 'To':'tower2', 'ActionID':'MainAction', 'Actions':
                    [
                        { 'type':'Pack', 'CallID':'Tower2Explore', 'Text':'SearchTower2', 'Condition':{ 'type':'Stat', 'stat':'Unit', 'count':'2' } },
                        { 'type':'Pack', 'CallID':'Tower2Explore2', 'Text':'SearchTower2Own' },
                        { 'type':'Close', 'Text':'CloseDialogue'}
                    ]}"),

               //Tower14
               Make("{ 'Base':'CloneObject', 'ID':'tower', 'RenameID':'tower14', 'TurnReactPanel':'true', 'ReactButton':{ 'icon':'LookEye', 'timer':0, 'EventID':'MainAction' }, 'OnIsland':'island_14', 'Place':'Center' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore', 'To':'tower14' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2ExploreDone', 'To':'tower14' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore2', 'To':'tower14' }"),
               Make("{ 'Base':'AddPack', 'ID':'Tower2Explore2Done', 'To':'tower14' }"),
               Make(@"{ 'Base':'ActionWork', 'ActionType':'Context', 'Text':'Tower2Explore', 'To':'tower14', 'ActionID':'MainAction', 'Actions':
                    [
                        { 'type':'Pack', 'CallID':'Tower2Explore', 'Text':'SearchTower2', 'Condition':{ 'type':'Stat', 'stat':'Unit', 'count':'2' } },
                        { 'type':'Pack', 'CallID':'Tower2Explore2', 'Text':'SearchTower2Own' },
                        { 'type':'Close', 'Text':'CloseDialogue'}
                    ]}"),

               //Tower7
               Make("{ 'Base':'CloneObject', 'ID':'tower', 'RenameID':'tower7', 'TurnReactPanel':'true', 'ReactButton':{ 'icon':'LookEye', 'timer':0, 'EventID':'MainAction' }, 'OnIsland':'island_7' }"),
               Make("{ 'Base':'CloneObject', 'ID':'tower', 'RenameID':'tower6', 'TurnReactPanel':'true', 'ReactButton':{ 'icon':'LookEye', 'timer':0, 'EventID':'MainAction' }, 'OnIsland':'island_6' }"),
               Make("{ 'Base':'CloneObject', 'ID':'tower', 'RenameID':'tower0', 'TurnReactPanel':'true', 'ReactButton':{ 'icon':'LookEye', 'timer':0, 'EventID':'MainAction' }, 'OnIsland':'island_0' }")
               
            };
        }
    }
}