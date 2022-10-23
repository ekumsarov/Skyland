using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CreateIcon : GameEvent
    {
        string IconObjectID;
        string IconID;
        string ObjectID;
        string mainEvent;
        IconInteractType type;
        IconInteractType layout;
        SkyObject parent = null;
        bool visibility = true;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CreateIcon";


            if (node["ID"] != null)
                IconObjectID = node["ID"].Value;

            IconID = "oprions";
            if (node["Icon"] != null)
                IconID = node["Icon"].Value;

            ObjectID = string.Empty;
            if (node["ObjectID"] != null)
                ObjectID = node["ObjectID"].Value;

            mainEvent = string.Empty;
            if (node["MainEvent"] != null)
                mainEvent = node["MainEvent"].Value;

            type = IconInteractType.Object;
            if (node["Type"] != null)
                type = (IconInteractType)Enum.Parse(typeof(IconInteractType), node["Type"].Value);

            layout = IconInteractType.SubLocation;
            if (node["Layout"] != null)
                layout = (IconInteractType)Enum.Parse(typeof(IconInteractType), node["Layout"].Value);

            if (node["IsActive"] != null)
                visibility = node["IsActive"].AsBool;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            SceneObject patent = null;
            if (!ObjectID.IsNullOrEmpty())
                patent = GetObject(ObjectID) as SceneObject;

            IconObject.Create(IconObjectID, IconID, type, layout, patent);
            IconObject temp = GM.GetIcon(IconObjectID);
            temp.Visible = visibility;

            if (!this.mainEvent.IsNullOrEmpty())
                temp.MainEvent = this.mainEvent;

            End();
        }

        #region static
        public static CreateIcon Create(string iconID, string icon, IconInteractType type, IconInteractType layout, string parent = null)
        {
            CreateIcon temp = new CreateIcon();
            temp.ID = "CreateIcon";

            temp.IconObjectID = iconID;
            temp.IconID = icon;
            temp.type = type;
            temp.layout = layout;
            temp.ObjectID = parent;

            return temp;
        }
        #endregion
    }
}