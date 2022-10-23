using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lodkod;
using GameEvents;
using System;
using BuildTypes;

public class BuildPlace : MapLocationObject
{
    [SerializeField] private GameObject _mains1;
    [SerializeField] private LocationObject _mainPlace;

    public List<BuildCell> _cells;

    private BuildType _type;
    public BuildType BuildType
    {
        get { return this._type; }
    }

    private BuildState _state;
    public BuildState State
    {
        get { return this._state; }
    }

    private bool _activatedMain;
    private List<BuildInfo> _openBuilds;
    public List<BuildInfo> OpenBuilds
    {
        get => this._openBuilds;
    }
    private BuildInfo _buildInfo;
    private int _openCells;
    public int OpenCells
    {
        get => this._openCells;
    }
    private int _level;

    private IconObject _mainIcon;

    public override void HardSet()
    {
        if (InitBased)
            return;
        InitBased = true;

        this._trans = this.transform;

        if (this.LocationPanel == null)
            this.LocationPanel = this.GetComponentInChildren<LocationPanel>(true);

        _act = new Activity();
        _act.Object = this;
        _act.initActivity();

        this.Action = new ActionAddition
        {
            parent = this
        };

        this.MainEvent = "Event";
        this._activatedMain = false;

        _sub = Subscriber.Create(this);

        this.IslandNumber = IM.GetIslandNumber(this.position);
        this._cells = new List<BuildCell>();
        this._openBuilds = new List<BuildInfo>(IOM.GetAvaliableBuildForMain());

        int cellIndex = 0;
        for (int i = 0; i < this._locations.Count; i++)
        {
            BuildCell tempType = this._locations[i].GetComponent<BuildCell>();
            if (tempType != null)
            {
                tempType.ParentID = this.ID;
                tempType.ID = this.ID + "Cell" + cellIndex;
                this._cells.Add(tempType);
                tempType.HardSet();
                tempType.Visible = false;
                tempType.Lock = true;
                cellIndex++;
            }
            else
            {
                this._locations[i].HardSet();
                this._locations[i].Visible = true;
                this._locations[i].LocationPanel.gameObject.SetActive(false);
                GM.AddUniq(this._locations[i]);
            }
        }

        IconObject.Create(this.ID + "QuitIcon", "BackArrow", IconInteractType.Object, this._quitLocation);
        IconObject tempIcon = GM.GetIcon(this.ID + "QuitIcon");
        tempIcon.Activity.PushPack("QuitMapLocation", new List<GameEvents.GameEvent>()
        {
            SafeCallFunction.Create("CloseLocations", this.ID),
            MoveCamera.Create(Vector3.zero, false, "island_" + this.IslandNumber),
            ActivateLocationOnIsland.Create(this.IslandNumber)
        });
        tempIcon.MainEvent = "QuitMapLocation";

        MapLocationIcon temp = this.gameObject.GetComponentInChildren<MapLocationIcon>(true);
        if (temp != null)
        {
            temp.name = this.ID + "MapIcon";
            temp.ID = this.ID + "MapIcon";
            temp.SetupMapIcon(this);
            GM.AddIcon(temp);
        }

        //BuildHammer
        this._mainIcon = this._mainPlace.GetComponentInChildren<IconObject>(true);
        if(this._mainIcon == null)
        {
            IconObject.Create(this.ID + "MainIcon", "HomeIcon", IconInteractType.Object, this);
            this._mainIcon = GM.GetIcon(this.ID + "MainIcon");
        }

        if (this._mainIcon != null)
        {
            this._mainIcon.name = this.ID + "MainIcon";
            this._mainIcon.ID = this._mainIcon.name;
            this._mainIcon.HardSet();
            GM.AddIcon(this._mainIcon);

            Actions mainAction = Actions.Get("Context");

            mainAction.ID = "StandartAction";
            mainAction.AddChoice(ActionButtonInfo.Create("Build").SetCallback(OpenMenu));
            mainAction.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

            this._mainIcon.AddAction(mainAction);

            Actions buildBase = Actions.Get("Context");

            buildBase.ID = "BuildCenter";
            buildBase.AddChoice(ActionButtonInfo.Create("BuildMain").SetAvailableCondition(StatCondition.Make("Wood", 15, StatCondition.StatConType.More)).SetAdditionalText("BuildMainAdditional").SetCallback(() => { this.ActivateMain(); }));
            buildBase.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

            this._mainIcon.AddAction(buildBase);
        }

        

        this.InstatinateEmptyMain();
    }

    void InstatinateEmptyMain(string infoID = "Center", int level = 0)
    {
        this._buildInfo = IOM.getBuildInfo("Center");
        this._mains1.gameObject.SetActive(false);
        this._mainIcon.MainEvent = "BuildCenter";
    }

    public void ActivateMain(string infoID = "Center", int level = 0)
    {
        SM.AddStat(-15, "Wood");

        if (!this._buildInfo.Name.Equals(infoID) || this._buildInfo.Level != level)
        {
            this._buildInfo = IOM.getBuildInfo(infoID, level);
        }

        this._openCells = this._buildInfo.Special["OpenSlots"].AsInt;
        this._level = this._buildInfo.Level;

        for (int i = 0; i < this._openCells; i++)
        {
            this._cells[i].Lock = false;
            this._cells[i].LockLocation();
        }

        this._mains1.gameObject.SetActive(true);
        this._mainIcon.MainEvent = "StandartAction";
    }

    public void StartBuild(BuildInfo build)
    {
        for(int i = 0; i < this._cells.Count; i++)
        {
            if (this._cells[i].State == BuildState.bs_Unactive)
            {
                this._cells[i].StartBuild(build);
                break;
            }
        }

    }

    public void PlaceBuild(BuildInfo build)
    {

    }

    public override bool Visible
    {
        get => base.Visible;
        set
        {
            if (this._visible == value)
                return;

            if (value && this._lock)
                return;

            this.gameObject.SetActive(true);
            this._visible = value;
            this.LocationPanel.gameObject.SetActive(value);
        }
    }

    public override void OpenLocations(Action del = null)
    {
        for (int i = 0; i < this._locations.Count; i++)
        {
            this._locations[i].RotatePanels(this.CameraPoint.Point);
            this._locations[i].LocationPanel.gameObject.SetActive(true);
        }

        this.Visible = true;
        this.LocationPanel.gameObject.SetActive(false);

        del?.Invoke();
    }

    public override void CloseLocations(Action del = null)
    {
        for (int i = 0; i < this._locations.Count; i++)
        {
            this._locations[i].LocationPanel.gameObject.SetActive(false);
        }

        this.Visible = false;
        this.LocationPanel.gameObject.SetActive(true);

        del?.Invoke();
    }

    public void OpenMenu()
    {
        UIM.OpenBuildMenu(this);
    }

    // Temp function
    public List<BuildInstance> GetBuildType(string id)
    {
        List<BuildInstance> temp = new List<BuildInstance>();
        for(int i = 0; i < this._cells.Count; i++)
        {
            if (this._cells[i].Info != null && this._cells[i].Info.Name.Equals(id))
                temp.Add(this._cells[i]._buildType);
        }
        return temp;
    }

    #region AvaliableForBuilding

    public void AddAvaliable(string info)
    {
        this._openBuilds.Add(IOM.getBuildInfo(info));
    }

    public void RemoveAvaliable(string info)
    {
        this._openBuilds = this.OpenBuilds.Where(item => !item.Name.Equals(info)).ToList();
    }

    public void ClearAllAvaliable()
    {
        this._openBuilds.Clear();
    }
    #endregion
}
