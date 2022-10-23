using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Lodkod;

public class Island : SceneObject {

    public enum iState
    {
        Nill,
        Invisible,
        Unexplored,
        Explored,
        Active
    }

    public enum Place
    {
        Center,
        Left,
        Right,
        Object,
        LeftUnit,
        RightUnit,
        SpikePlace
    }

    #region parameters


    public BoxCollider Collider;

    bool _inWeb;
    bool _explored;
    bool _hasResource;
    bool _hasBuild;
    bool _hasObject;
    bool _withSkystone;
    bool _lockBonfire = true;
    public bool _lockLeft;
    public bool _lockRight;

    [SerializeField]
    private Transform _boatPlace;

    [SerializeField]
    private CameraPoint _zoomPoint;
    public CameraPoint ZoomPoint
    {
        get { return this._zoomPoint; }
    }

    public iState _state = iState.Nill;

    public List<int> IslandConnection;

    public Dictionary<string, ActionButtonInfo> _mapActions;

    public Dictionary<string, string> _questIcons;

    #endregion

    public ActionButtonInfo ExploreAction;
    public ActionButtonInfo FlyToAction;

    public void initIsland()
    {

        this.ID = "island_" + this.IslandNumber;

        _explored = false;
        HasResource = false;
        HasObject = false;
        HasBuild = false;
        InWeb = false;
        this.IslandConnection = new List<int>();

        this._mapActions = new Dictionary<string, ActionButtonInfo>();
        this._questIcons = new Dictionary<string, string>();

        ExploreAction = ActionButtonInfo.Create("Explore").SetText("ExploreMapAction").SetCallData("StartFly").SetType(ActionType.Pack);
        FlyToAction = ActionButtonInfo.Create("Fly").SetText("FlyMapAction").SetCallData("StartFly").SetType(ActionType.Pack);

        _alreadyVisible = false;
        State = iState.Nill;
    }

    public new Vector3 position
    {
        get { return gameObject.transform.position; }
        set { gameObject.transform.position = value; }
    }

    public void getToIsland()
    {

        GM.GameState = GameState.EventWorking;

        if(!this._explored)
        {
            this._explored = true;
            ES.NotifySubscribers(TriggerType.IslandFirstExplore.ToString(), this.ID);
            IM.IslandActions(this.IslandNumber);
            CheckActions();
            return;
        }
        IM.IslandActions(this.IslandNumber);
        CheckActions();
        //GM.questManager.improveExploreQuests(this.IslandNumber);
    }

    private void CheckActions()
    {
        GM.GameState = GameState.Game;
        this._act.callAllActivityPack();
    }

    public Vector3 BoatPlace
    {
        get
        {
            return this._boatPlace == null ? this.position : this._boatPlace.position;
        }
    }

    /*public void PlaceCenter(int side = 0, bool already = false)
    {
        LDObject temp_c = GM.instatinatePrefab("Prefabs/units/Builds/center", Containers.Centers);
        temp_c.transform.position = ObjectPlace.position;
        BM.Centers.Add(temp_c.GetComponent<Center>());
        
        temp_c.GetComponent<Center>().Side = side;
        temp_c.GetComponent<Center>().IslandNumber = IslandNumber;
        temp_c.GetComponent<Center>().InitBuild();

//        if (already)
//            temp_c.GetComponent<Center>().PlaceBuild();

        HasBuild = true;

        BM.RemoveBonfire(IslandNumber);
    }*/

    public void RemoveCenter()
    {
        HasBuild = false;
    }

     
    public bool Active
    {
        set
        {
            this.LockLocation(!value);
        }
    }

    bool _alreadyVisible = true;
    public iState State
    {
        get { return this._state; }
        set
        {
            if (value == iState.Nill)
            {
                this._state = iState.Nill;
                return;
            }
                

            if (value == _state)
                return;

            _state = value;
            _alreadyVisible = false;

            if(value == iState.Invisible)
            {
                this.Visible = false;
                this._alreadyVisible = false;

                foreach (var build in GM.Uniqs.Values)
                {
                    if(build.IslandNumber == this.IslandNumber)
                        build.Visible = false;
                }

                return;
            }
            
            if(value == iState.Unexplored)
            {
                this.Visible = true;
                this._alreadyVisible = false;

                this._mapActions.Remove("Fly");
                this._mapActions.Add(ExploreAction.ID, ExploreAction);

                foreach (var build in GM.Uniqs.Values)
                {
                    if (build.IslandNumber == this.IslandNumber)
                        build.Visible = false;
                }

                return;
            }

            if (value == iState.Explored)
            {
                this._mapActions.Remove("Explore");
                this._mapActions.Add(FlyToAction.ID, FlyToAction);
            }
            else if(value == iState.Active)
            {
                ES.NotifySubscribers(TriggerType.IslandCompleteFly.ToString(), this.ID);
            }

            if (!_alreadyVisible)
            {
                this._alreadyVisible = true;
                this.Visible = true;

                foreach (var build in GM.Uniqs.Values)
                {
                    if (build.IslandNumber == this.IslandNumber)
                        build.Visible = true;
                }
            }
        }
    }

    public bool HasBuild
    {
        get { return _hasBuild; }
        set
        {
            _hasBuild = value;

            if (_hasBuild)
            {
                BM.RemoveBonfire(IslandNumber);
                HasObject = true;
            }
        }
    }
    // получение области острова
    public bool HasObject
    {
        get { return _hasObject; }
        set
        {
            _hasObject = value;
        }
    }
    // получение области острова
    public bool InWeb
    {
        get { return _inWeb; }
        set { _inWeb = value; }
    }
    // исследован ли остров
    public bool Explored
    {
        get { return _explored; }
        /*set {
            if(value)
            {
                _explored = value;
                ES.NotifySubscribers(TriggerType.IslandFirstExplore.ToString(), this.ID);

//                for (int i = 0; i < Global.instance.questObjects.Count; i++)
//                {
//                   if (Global.instance.questObjects[i].gameObject.GetComponent<QuestObject>().IslandNumber == this.IslandNumber)
//                        Global.instance.questObjects[i].gameObject.GetComponent<QuestObject>().actvateObject();
//                }
                
            }
            else
            {
                _explored = value;
            }

            IM.IslandActions(this.IslandNumber);
        }*/
    }
    // отсров с ресурсами?
    public bool HasResource
    {
        get { return _hasResource; }
        set
        {
            _hasResource = value;
        }
    }

    public bool LockLeft
    {
        get { return this._lockLeft; }
    }

    public bool LockRight
    {
        get { return this._lockRight; }
    }

    public void LockEnter(bool left)
    {
        if(left)
        {
            this._lockLeft = true;
            return;
        }

        this._lockRight = true;
    }

    public void UnlockEnter(bool left)
    {
        if (left)
        {
            this._lockLeft = false;
            return;
        }

        this._lockRight = false;
    }

    #region Map Actions
    public void AddMapAction(ActionButtonInfo act)
    {
        this._mapActions.Add(act.ID, act);

        ES.NotifySubscribers(TriggerType.IslandUpdate.ToString(), this.ID);
    }

    public void RemoveMapAction(string actID)
    {
        if (this._mapActions.ContainsKey(actID))
            this._mapActions.Remove(actID);

        ES.NotifySubscribers(TriggerType.IslandUpdate.ToString(), this.ID);
    }

    public void ReplaceMapAction(string actID, ActionButtonInfo info)
    {
        if (!this._mapActions.ContainsKey(actID))
            this._mapActions.Add(info.ID, info);
        else
        {
            this._mapActions.Remove(actID);
            this._mapActions.Add(info.ID, info);
        }

        ES.NotifySubscribers(TriggerType.IslandUpdate.ToString(), this.ID);
    }

    public List<string> GetQuestIcons()
    {
        return this._questIcons.Values.ToList();
    }

    public void AddQuestIcon(string id, string ico)
    {
        if (this._questIcons.Any(val => val.Key == id))
            return;

        this._questIcons.Add(id, ico);
        ES.NotifySubscribers(TriggerType.IslandUpdate.ToString(), this.ID);
    }

    public void RemoveQuestIcon(string id)
    {
        if (!this._questIcons.ContainsKey(id))
            return;

        this._questIcons.Remove(id);
        ES.NotifySubscribers(TriggerType.IslandUpdate.ToString(), this.ID);

    }
    #endregion
}
