using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;
using GameEvents;

public class MapObject : UIItem { 

    public Image MainIco;

    public List<UIImage> icos;

    public Island _connected;

    

    public override void Setting()
    {
        base.Setting();

        this.ID = "MapObject" + this._connected.IslandNumber;

        Actions act = Actions.Get("Context");
        act.ID = "MapAct";
        act.list.Add(ActionButtonInfo.Create("CloseDialogue").SetType(ActionType.Close));
        this.AddAction(act);
        
        this.Activity.PushPack("StartFly", new List<GameEvent>
            {
                HideMenu.Create("MapMenu"),
//                MoveCamera.Create(Vector3.zero, to:"MainShip"),
                FollowCamera.Create("MainShip", true),
                MoveObject.Create("MainShip", Vector3.zero, this._connected.ID, false, true)
            });

        icos[0].Image = "shippp";
        icos[1].Image = "iconhome";

        AddSubscriber(TriggerType.IslandUpdate, this._connected.ID);
        IslandUpdate();
    }

    public void IslandUpdate()
    {
        icos[0].Visible = false;
        if (_connected.State == Island.iState.Active)
        {
            this.Visible = true;
            this.MainIco.color = new Color(1, 1, 1, 1);
            this.Interactable = false;
            icos[0].Visible = false;
        }
        else if(_connected.State == Island.iState.Invisible || _connected.State == Island.iState.Nill)
        {
            this.Visible = false;
            this.Interactable = false;
        }
        else
        {
            this.Visible = true;
            this.Interactable = true;

            if (_connected._mapActions.Count > 0)
            {
                List<ActionButtonInfo> listac = this.Action.GetActionList("MapAct");
                List<string> removinglis = new List<string>();

                foreach (var act in listac)
                {
                    if (act.ID == "CloseDialogue" || _connected._mapActions.Any(it => it.Key == act.ID))
                        continue;

                    if (_connected._mapActions.All(it => it.Key != act.ID))
                    {
                        removinglis.Add(act.ID);
                        continue;
                    }
                }

                foreach (var key in removinglis)
                {
                    this.Action.RemoveActionChoice("MapAct", key);
                }

                listac = this.Action.GetActionList("MapAct");

                foreach (var act in _connected._mapActions)
                {
                    if (listac.All(it => it.ID != act.Key))
                        this.AddActionChoice("MapAct", act.Value);
                }
            }

            if (_connected.State == Island.iState.Explored)
            {
                

                
                this.MainIco.color = new Color(1, 1, 1, 1);
                if (_connected.HasBuild)
                {
                    // TODO: Make sprite change
                    //Ico2.sprite = GuiIconProvider.GetIcon("");
                    icos[1].Visible = true;
                }
                else
                    icos[2].Visible = false;

                List<string> conIco = this._connected.GetQuestIcons();

                if(conIco.Count > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < conIco.Count)
                        {
                            this.icos[i + 2].Image = conIco[i];
                            this.icos[i + 2].Visible = true;
                        }
                        else
                            this.icos[i + 2].Visible = false;
                            
                    }
                }
                else
                {
                    for (int i = 2; i < 5; i++)
                        this.icos[i].Visible = false;
                }
                
            }
            else if(_connected.State == Island.iState.Unexplored)
            {
                
                this.MainIco.color = new Color(0.5f, 0.5f, 0.5f, 1);

                foreach (var ic in icos)
                    ic.Visible = false;
            }
            
        }
    }

    public override void Pressed()
    {
        this.CallAction("MapAct");
    }

    public void ExploreIsland()
    {
        //Ship ship = GM.GetObject("MainShip") as Ship;
        if(_connected.IslandNumber == 9)
            IM.Islands[2].AddMapAction(ActionButtonInfo.Create("Ignore").SetText("AttackIsland").SetCallData("").SetType(ActionType.Pack));
        
    }

    public void FlyToIsland()
    {
        Debug.LogError("Start Fly");
    }
}
