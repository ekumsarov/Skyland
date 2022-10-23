using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using GameEvents;
using EventPackSystem;

public class Ship : FlyingObject
{
    Island targetIsland;
    float moveTime;

    ActionButtonInfo stopAction;

    Rigidbody _rigidbody;
    // Use this for initialization
    public override void initUnit()
    {
        this.targetIsland = null;
        this.Type = (int)Unitype.Player;  // тип юнита(объекта) в данной игре
    }

    #region ProcessState

    protected override void s_Activation()
    {
        this.InWork = false;

        this.Velocity = new Vector2(1.0f, 1.0f);
        this.OnIsland = true;

        this.Side = 0;
        this.IslandNumber = IM.GetIslandNumber(this.position);

        this._rigidbody = gameObject.GetComponent<Rigidbody>();

        Actions act = Actions.Get("Context");
        act.ID = "MainAction";

        act.list.Add(ActionButtonInfo.Create("ShipActMap").SetCallData("MapAction"));
        act.list.Add(ActionButtonInfo.Create("ShipActGroup").SetCallData("MapAction"));
        act.list.Add(ActionButtonInfo.Create("CloseDialogue").SetType(ActionType.Close));

        stopAction = ActionButtonInfo.Create("StopShip").SetCallData("StopShip").SetType(ActionType.Special);

        this.Action.AddAction(act);

        // map action
        act = Actions.Get("Map");
        act.ID = "MapAction";
        this.Action.AddAction(act);

        this.gameObject.SetActive(true);

        this.Finish();
    }

    protected override void s_StartWork()
    {
        this.LockLocation();
        this.AddActionChoice("MainAction", stopAction);

        IM.Islands[this.IslandNumber].State = Island.iState.Explored;

        this._pointToGo = this.Points[0];
        distance = Vector3.Distance(this.position, this._pointToGo);
        moveTime = 1 / (distance / 4f);
        curTime = 0f;
        startPoint = this.position;

        this.Points.RemoveAt(0);

        //GM.Player.Activated = false;

        this.StartCoroutine(Moving());
    }

    protected override void s_Fly()
    {
        this.LockLocation(false);
        ES.NotifySubscribers("FlyingShip", this.ID);

        this._pointToGo = this.Points[0];
        distance = Vector3.Distance(this.position, this._pointToGo);
        moveTime = 1 / (distance / 4f);
        curTime = 0f;
        startPoint = this.position;

        this.Points.RemoveAt(0);

        this.StartCoroutine(Moving());
    }

    protected override void s_CompleteFly()
    {
        this.LockLocation();
        this._pointToGo = this.Points[0];
        distance = Vector3.Distance(this.position, this._pointToGo);
        moveTime = 1 / (distance / 4f);
        curTime = 0f;
        startPoint = this.position;

        this.Points.RemoveAt(0);

        this.StartCoroutine(CompleteFly());
    }

    protected override void s_Work()
    {
        this.State = UnitState.s_WaitTask;
        this.Finish();
    }

    #endregion



    #region Coroutines

    float distance;
    float curTime;
    Vector3 startPoint;

    IEnumerator Moving()
    {
        float sqrRemainingDistance = (this._rigidbody.position - this._pointToGo).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            float freqDis = curTime / distance;

            this._rigidbody.MovePosition(Vector3.Lerp(startPoint, this._pointToGo, freqDis));
            sqrRemainingDistance = (this._rigidbody.position - this._pointToGo).sqrMagnitude;

            curTime += 2 * Time.deltaTime;

            yield return null;

        }
        this.Finish();
    }

    IEnumerator CompleteFly()
    {
        float sqrRemainingDistance = (this._rigidbody.position - this._pointToGo).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            float freqDis = curTime / distance;

            this._rigidbody.MovePosition(Vector3.Lerp(startPoint, this._pointToGo, freqDis));
            sqrRemainingDistance = (this._rigidbody.position - this._pointToGo).sqrMagnitude;

            curTime += 2 * Time.deltaTime;

            yield return null;

        }

        GM.Camera.StopFollow();
        GM.Camera.moveToIsland(this.targetIsland.IslandNumber, false, AspectCamera);
    }

    public void AspectCamera()
    {
        this.Action.RemoveActionChoice("MainAction", stopAction.ID);
        this.IslandNumber = this.targetIsland.IslandNumber;

        this.targetIsland.getToIsland();
        this.targetIsland = null;
        this.OnIsland = true;

        GM.Player.IslandNumber = this.IslandNumber;
        GM.Player.position = IM.Islands[this.IslandNumber].BoatPlace;


        this.InProcess = false;
        this.Finish();
    }

    #endregion

    public override void GoTo(SkyObject point, bool fast = false, Action _del = null)
    {
        if(fast)
        {
            if(point.GetComponent<Island>() != null)
            {
                Island isl = point.GetComponent<Island>();
                Vector3 finalTarget = isl.BoatPlace;
                this.gameObject.SetActive(false);
                this.position = finalTarget;
                this.IslandNumber = isl.IslandNumber;
                this.gameObject.SetActive(true);

                if (_del != null)
                {
                    _del();
                }

                return;
            }
            else
            {
                GoTo(point.position, fast, _del);
                return;
            }
        }

        this.InProcess = true;
        Vector3 tar;

        if (point.GetComponent<Island>()!=null)
        {
            this.targetIsland = point.GetComponent<Island>();
            Vector3 finalTarget = this.targetIsland.BoatPlace;

            if (this.OnIsland)
            {
                // Start fly
                if (this.targetIsland.position.x < this.position.x)
                    tar = new Vector3(this.position.x - 3f, this.position.y + 10f, -30f);
                else
                    tar = new Vector3(this.position.x + 6f, this.position.y + 10f, -30f);

                this.Points.Add(tar);
                this._states.Add(UnitState.s_StartWork);
            }

            // Move to Island


            if (finalTarget.x < this.position.x)
                tar = new Vector3(finalTarget.x + 8f, finalTarget.y + 10f, -30f);
            else
                tar = new Vector3(finalTarget.x - 8f, finalTarget.y + 10f, -30f);

            this.Points.Add(tar);
            this._states.Add(UnitState.s_Fly);

            // Sit on Island

            this.Points.Add(finalTarget);
            this._states.Add(UnitState.s_CompleteFly);

            if(_del != null)
            {
                this.Delegate = _del;
                this._states.Add(UnitState.s_ContinueEvent);
            }

            if (this.State == UnitState.s_WaitTask)
                this.InProcess = false;
            return;
        }

        GoTo(point.position, fast, _del);
    }

    public override void GoTo(Vector3 point, bool fast = false, Action _del = null)
    {
        if(fast)
        {
            this.gameObject.SetActive(false);
            this.position = point;
            this.gameObject.SetActive(true);

            if (_del != null)
            {
                _del();
            }

            return;
        }

        this.InProcess = true;
        Vector3 tar;
        if (this.OnIsland)
        {
            // Start fly
            if (point.x < this.position.x)
                tar = new Vector3(this.position.x - 3f, this.position.y + 10f, -30f);
            else
                tar = new Vector3(this.position.x + 6f, this.position.y + 10f, -30f);

            this.Points.Add(tar);
            this._states.Add(UnitState.s_StartWork);
        }

        if (point.x < this.position.x)
            tar = new Vector3(point.x + 8f, point.y, -30f);
        else
            tar = new Vector3(point.x - 8f, point.y, -30f);

        this.Points.Add(tar);
        this._states.Add(UnitState.s_Fly);

        if (_del != null)
        {
            this.Delegate = _del;
            this._states.Add(UnitState.s_ContinueEvent);
        }

        if (this.State == UnitState.s_WaitTask)
            this.InProcess = false;
    }

    

    public void StopShip()
    {
        this.InProcess = true;
        this.StopAllCoroutines();

        if(_states.Any(stat => stat == UnitState.s_ContinueEvent))
        {
            if (this.Delegate != null)
                this.Delegate();

            this.Delegate = null;
        }

        this._states.Clear();
        this.Points.Clear();
        this.OnIsland = false;

        this.LockLocation(false);

        this.Action.RemoveActionChoice("MainAction", stopAction.ID);

        this.InProcess = false;
    }

    /*
     * 
     * Set And Get
     * 
     * 
     */
    public void setTargetIsland(Island island)
    { this.targetIsland = island; }
}