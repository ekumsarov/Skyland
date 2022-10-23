using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ShowMenu : GameEvent
    {
        string MenuID;
        List<string> Menus;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ShowMenu";

            MenuID = "All";

            JSONArray ar = node["MenuID"].AsArray;

            if (ar == null)
            {
                if (node["MenuID"] != null)
                    MenuID = node["MenuID"].Value;
            }
            else
            {
                Menus = new List<string>();
                for (int i = 0; i < ar.Count; i++)
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
                UIM.ShowAllMenu();
            else if (Menus != null)
            {
                foreach (var menu in Menus)
                {
                    UIM.ShowMenu(menu);
                }
            }
            else
                UIM.ShowMenu(MenuID);

            End();
        }

        #region static
        public static ShowMenu Create(string MenuID = "all", List<string> IDs = null)
        {
            ShowMenu temp = new ShowMenu();
            temp.ID = "ShowMenu";

            temp.MenuID = MenuID;
            
            if(IDs != null)
            {
                temp.Menus = new List<string>();
                temp.Menus.AddRange(IDs);
            }

            return temp;
        }
        #endregion
    }
}