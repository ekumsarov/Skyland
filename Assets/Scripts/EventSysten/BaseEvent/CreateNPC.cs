using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CreateNPC : GameEvent
    {
        string ObjectName;
        Island OnIsland;
        Island.Place place;
        SkyObject sceneObject;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CreateNPC";

            if (node["ID"] != null)
                ObjectName = node["ID"].Value;
            
            OnIsland = null;
            if (node["OnIsland"] != null)
                OnIsland = GetObject(node["OnIsland"].Value).GetComponent<Island>();

            sceneObject = null;
            if (node["NearObject"] != null)
                sceneObject = GetObject(node["NearObject"].Value);

            if(OnIsland == null && sceneObject == null)
                OnIsland = IM.GerRandomIsland();

            place = Island.Place.Center;
            if (node["Place"] != null)
                place = (Island.Place)Enum.Parse(typeof(Island.Place), node["Place"].Value);
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (!GM.Uniqs.ContainsKey(ObjectName))
            {
                Debug.LogError("No Such Object In Mission Folder");
                return;
            }

            SkyObject temp = GM.Uniqs[ObjectName];
            SceneObject curObject = temp.GetComponent<SceneObject>();

            if(curObject != null && sceneObject != null)
            {
                curObject.PlaceObject(sceneObject, true);
                curObject.Rotation = new Quaternion(curObject.Rotation.x, 180f, curObject.Rotation.z, curObject.Rotation.w);
            }

            temp.GetComponent<Unit>().Process(UnitState.s_Activation);

            End();
        }


        #region static
        public static CreateNPC Create(string _id, string ils = "Random", Island.Place islPlace = Island.Place.Center)
        {
            CreateNPC temp = new CreateNPC();
            temp.ID = "CreateNPC";

            temp.ObjectName = _id;
            temp.place = islPlace;
            
            if (ils.Equals("Random"))
                temp.OnIsland = IM.GerRandomIsland();
            else
                temp.OnIsland = temp.GetObject(ils).GetComponent<Island>();

            return temp;
        }
        #endregion
    }
}