using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class FirstStep : GameEvent
    {

        public override void Init()
        {
            this.ID = "FirstStep";
            Simple = false;
            initialized = false;

//            RM.GetObject("forest14").AddIcon(ReactButton.CreateButton("HunterTalk", 0));
//            RM.GetObject("forest14").LockLocation();

            Actions act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Stepone");
            act.ID = "FirstDialogue";
            GetObject("Sanctuary").AddAction(act);

            act.list.Add(ActionButtonInfo.Create("Whatneed")
                .SetText("Whatneed")

                .SetCallData("Help")
                );

            act = Actions.Get("Context");
            act.Text = LocalizationManager.Get("Stepone1");
            act.ID = "Help";
            GetObject("Sanctuary").AddAction(act);

                act.list.Add(ActionButtonInfo.Create("Weneed")
                .SetText("Weneed")
                .SetCallData("Jopa")
                .SetType(ActionType.Pack)
                .SetType(ActionType.Close)
                );

            Object.Activity.PushPack("Jopa", new List<GameEvent>() {
                ActionWork.Create("Whatneed", "Sanctuary", null, _WorkType: "ChangeText", text:"Godothis"),
                ReactLock.Create("Sanctuary", false)
            });

            SceneObject build = GetObject("Sanctuary") as SceneObject;
            //build.RebuildReactButton("Main", eventID: "FirstDialogue");

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


    }
}