using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CloneObject : GameEvent
    {
        static int IDStr = 100;

        string ObjectName;
        Island OnIsland;
        Island.Place place;
        bool TurnReactPanel;

        string RenameID;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CloneObject";

            IDStr += 1;

            if (node["ID"] != null)
                ObjectName = node["ID"].Value;

            RenameID = ObjectName + IDStr;
            if (node["RenameID"] != null)
                RenameID = node["RenameID"].Value;

            OnIsland = null;
            if (node["OnIsland"] != null)
            {
                if (node["OnIsland"].Value == "Random")
                    OnIsland = IM.GerRandomIsland();
                else
                    OnIsland = GetObject(node["OnIsland"].Value).GetComponent<Island>();
            }

            place = Island.Place.Object;
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
                Debug.LogError("No Such Object In Mission Folder: " + ObjectName);
                return;
            }

            SceneObject temp = UnityEngine.Object.Instantiate(GM.Uniqs[ObjectName]) as SceneObject;
            temp.ID = RenameID;
            temp.transform.SetParent(GM.GetParentContainer(Containers.Uniqs));
            temp.Visible = false;
            GM.AddUniq(temp);
            temp.HardSet();


            if (temp == null)
            {
                Debug.LogError("This object not a SceneObject: " + ObjectName);
                return;
            }

            if (OnIsland)
            {
                temp.IslandNumber = OnIsland.IslandNumber;
                if (place == Island.Place.Center)
                    BM.RemoveBonfire(OnIsland.IslandNumber);

                if (OnIsland.Visible)
                    temp.Visible = true;
            }

            temp.LockLocation(!TurnReactPanel);

            temp.gameObject.SetActive(true);
            End();
        }


        #region static
        public static CloneObject Create(string _id, string renameID, string ils = "Random")
        {
            CloneObject temp = new CloneObject();
            temp.ID = "CreateObject";

            temp.ObjectName = _id;
            temp.RenameID = renameID;

            if (ils.Equals("Random"))
                temp.OnIsland = IM.GerRandomIsland();
            else
                temp.OnIsland = temp.GetObject(ils).GetComponent<Island>();

            return temp;
        }
        #endregion
    }
}