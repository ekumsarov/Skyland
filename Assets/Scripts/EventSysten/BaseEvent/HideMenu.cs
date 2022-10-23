using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class HideMenu : GameEvent
    {
        string MenuID;
        List<string> Menus;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "HideMenu";

            MenuID = "All";

            JSONArray ar = node["MenuID"].AsArray;

            if(ar == null)
            {
                if (node["MenuID"] != null)
                    MenuID = node["MenuID"].Value;
            }
            else
            {
                Menus = new List<string>();
                for(int i = 0; i < ar.Count; i++)
                {
                    Menus.Add(ar[i].Value);
                }
            }
            
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (MenuID.Equals("All"))
                UIM.HideAllMenu();
            else if(Menus != null)
            {
                foreach(var menu in Menus)
                {
                    UIM.HideMenu(menu);
                }
            }
            else
                UIM.HideMenu(MenuID);

            End();
        }

        #region static
        public static HideMenu Create(string MenuID = "all", List<string> IDs = null)
        {
            HideMenu temp = new HideMenu();
            temp.ID = "HideMenu";

            temp.MenuID = MenuID;

            if (IDs != null)
            {
                temp.Menus = new List<string>();
                temp.Menus.AddRange(IDs);
            }

            return temp;
        }
        #endregion
    }
}