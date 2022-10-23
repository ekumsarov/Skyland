using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class StartScene : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'ActivateLocationOnIsland', 'Activate':'false', 'Island':'1' }"),
               Make("{ 'Base':'MoveCamera', 'to':'Sanctuary', 'fast':'true' }"),
               Make("{ 'Base':'MoveIcon', 'To':'QuitLocation', 'ID':'Player' }"),
               Make("{ 'Base':'SafeCallFunction', 'To':'Sanctuary', 'Function':'OpenLocations' }"),
               Make("{ 'Base':'FadeScreen', 'Fade':'out' }"),
               Make("{ 'Base':'ShowActionTooltip', 'Text':'mercenary1' }"),
               Make("{ 'Base':'ShowActionTooltip', 'Text':'mercenary2' }"),
               Make("{ 'Base':'ShowActionTooltip', 'Text':'mercenary3' }"),
               Make("{ 'Base':'ShowActionTooltip', 'Text':'mercenary4' }"),
               Make("{ 'Base':'ShowActionTooltip', 'Text':'GG1' }"),
               Make("{ 'Base':'CallEvent', 'To':'Manager', 'ID': { 'Event':'DemoControll', 'Action':'SetupQuests'} }")
            };
        }
    }
}
