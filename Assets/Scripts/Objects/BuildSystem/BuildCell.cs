using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using BuildTypes;

public class BuildCell : LocationObject
{
    [SerializeField] protected IconObject _cellIcon;
    [SerializeField] private GameObject _fence;

    public string ParentID;

    SceneObject _prefabObject;
    public BuildInfo Info;

    BuildState _state = BuildState.bs_Unactive;
    public BuildState State
    {
        get
        {
            return this._state;
        }
        set
        {
            this._state = value;
            if(this._state == BuildState.bs_Ready)
            {
                this._fence.gameObject.SetActive(false);
                this._prefabObject.gameObject.SetActive(true);
                this._cellIcon.MainEvent = this._standartActionID;
                this._cellIcon.Visible = true;

                ES.NotifySubscribers("BuildComplete", ParentID + this.Info.Name);
                this._state = BuildState.bs_Active;
            }
            else if(this._state == BuildState.bs_Unactive)
            {
                this._buildType = null;
                Destroy(this._prefabObject);
                this._fence.gameObject.SetActive(false);
                this._cellIcon.Visible = false;
            }
        }
    }

    public BuildInstance _buildType;

    private string _startBuildActionID;
    private string _standartActionID;
    private string _standartAcivateProdunction;

    public override void HardSet()
    {
        base.HardSet();

        this._cellIcon = GetComponentInChildren<IconObject>();
        if (this._cellIcon == null)
            this._cellIcon = IconObject.GetIcon(this.ID + "Icon", "BuildHammer");

        this._startBuildActionID = "StartBuildAction";
        this._standartActionID = "StandartAction";

        this._cellIcon.HardSet();
        this._cellIcon.SetObjectParent(this);
        this._cellIcon.Visible = false;

        Actions iconActionMain = Actions.Get("Context");

        iconActionMain.ID = this._startBuildActionID;
        iconActionMain.AddChoice(ActionButtonInfo.Create("StopBuild").SetCallback(StopBuilding));
        iconActionMain.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

        this._cellIcon.AddAction(iconActionMain);

        Actions iconStandartAction = Actions.Get("Context");

        iconStandartAction.ID = this._standartActionID;
        iconStandartAction.AddChoice(ActionButtonInfo.Create("StopProduction").SetCallback(StopProduction));
        iconStandartAction.AddChoice(ActionButtonInfo.Create("Upgrade").SetCallback(UpgradeBuild));
        iconStandartAction.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

        this._cellIcon.AddAction(iconStandartAction);

        Actions iconActivateAction = Actions.Get("Context");

        iconActivateAction.ID = this._standartActionID;
        iconActivateAction.AddChoice(ActionButtonInfo.Create("ActivateProduction").SetCallback(ActivateProduction));
        iconActivateAction.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

        this._cellIcon.AddAction(iconActivateAction);



        _fence.gameObject.SetActive(false);
        Info = null;
        this._state = BuildState.bs_Unactive;
    }

    public void StartBuild(BuildInfo info)
    {
        if(!iStat.CheckTakeList(info.Cost))
        {
            Debug.Log("Have no resources");
            return;
        }

        _prefabObject = UnityEngine.Object.Instantiate(Resources.Load<SceneObject>("Prefabs/units/Builds/" + info.Prefab));
        if(_prefabObject == null)
            _prefabObject = UnityEngine.Object.Instantiate(Resources.Load<SceneObject>("Prefabs/units/Builds/Template"));

        _prefabObject.gameObject.SetActive(false);
        _prefabObject.transform.SetParent(this.transform);
        _prefabObject.transform.localPosition = Vector3.zero;

        this._cellIcon.MainEvent = this._startBuildActionID;
        this._cellIcon.Visible = true;
        _fence.gameObject.SetActive(true);

        this.Info = info;
        _buildType = BuildInstance.GetBuild(this, this.Info);

        this._state = BuildState.bs_inBuilds;

        this.Lock = false;
        this.LocationPanel.gameObject.SetActive(true);
        this.Visible = true;
    }

    public void StopBuilding()
    {
        this._state = BuildState.bs_Unactive;
    }

    public void UpgradeBuild()
    {

    }

    public void StopProduction()
    {
        this._buildType.StopProduction();
    }

    public void ActivateProduction()
    {
        this._buildType.ActivateProduction();
    }

    public void Update()
    {
        if (this._buildType != null && this._state == BuildState.bs_inBuilds)
            this._buildType.Improve(Time.deltaTime);
    }

    public void AddActionChoice(ActionButtonInfo choice)
    {
        this._cellIcon.AddActionChoice(this._standartActionID, choice);
    }

    public void RemoveActionChoice(string choiceID)
    {
        this._cellIcon.RepmoveActionChoice(this._standartActionID, choiceID);
    }
}
