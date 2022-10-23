using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System;
using System.Reflection;
using Lodkod;

public class Unit : SceneObject {


    public override void HardSet()
    {
        if (InitBased)
            return;
        InitBased = true;

        this._trans = this.transform;

        if (this.LocationPanel == null)
            this.LocationPanel = this.GetComponentInChildren<LocationPanel>();

        _act = new Activity();
        _act.Object = this;
        _act.initActivity();

        this.Action = new ActionAddition
        {
            parent = this
        };

        this.MainEvent = "Event";

        _sub = Subscriber.Create(this);

        this._unit = true;
        anim = GetComponent<Animator>();
        this._points = new List<Vector3>();
        this._agent = this.GetComponent<NavMeshAgent>();
        this._states = new List<UnitState>();
        this.Velocity = new Vector2();

        this.HP = 0.0;

        this.State = UnitState.s_Deactive; // состояние объекта 


        // bool types
        this.InProcess = false; // процесс машины

        this.Side = -1;
        this.InWork = false;

        this.initUnit();
    }

    public virtual void initUnit()
    {

    }

    
    public static void preSet()
    {
    }

    #region Properties
    /*
     * описание юнита
     */


    // int types
    public UnitState _state;

    private Animator anim;

    private float _workTime;

    // bool types
    

    NavMeshAgent _agent;


    // cocos types
    Vector3 _targetPlace;
    Vector2 _velocityObject; // скорость

    protected Vector3 _pointToGo; // точка остоновки

    [SerializeField]
    public Action _delAct = null;

    // bool types
    private bool _inProcess; // процесс машины
    private bool _velocityRandom = false;
    private bool _inWork;


    // massives
    public List<UnitState> _states; // список необходимых состояний

    protected List<Vector3> _points; //  список точек для движения






    // start with set

    /*
     * sets the workTime
     */
    public float WorkTime
    {
        get { return _workTime; }
        set { _workTime = value; }
    }
    /*
     * sets the inWork
     */
    public bool InWork
    {
        get { return _inWork; }
        set { _inWork = value; }
    }
    /*
     * sets the state
     */
    public UnitState State
    {
        get { return _state; }
        set
        {
            if (anim && this.gameObject.activeSelf)
                anim.SetInteger("state", (int)value);

            _state = value;
        }
    }
    /*
     * sets the process
     */
    public bool InProcess
    {
        get { return _inProcess; }
        set { _inProcess = value; }
    }
    /*
     * sets the process
     */
    public NavMeshAgent Agent
    {
        get { return _agent; }
        set { _agent = value; }
    }
    /*
     * get the points
     */
    public List<Vector3> Points
    {
        get { return _points; }
    }
    /*
     * sets the velocity
     */
    public Vector2 Velocity
    {
        set
        {
            _velocityObject.x = value.x;
            _velocityObject.y = value.y;
        }
    }
    public Action Delegate
    {
        get { return this._delAct; }
        set
        {
            this._delAct = value;
        }
    }
    /*
     * get random velocity
     */
    public float getVelocity()  {
    
        if(_velocityRandom)
            return UnityEngine.Random.Range(_velocityObject.x, _velocityObject.y);
    
        return (_velocityObject.x+_velocityObject.y)/2;

    }

    public UnitState States
    {
        set
        {
            this._states.Add(value);

            if (this._states.Count == 1 && this.InProcess)
                this.InProcess = false;
                
        }
    }

    public override Vector3 position
    {
        get
        {
            return this._trans.position;
        }
        set
        {
            this._trans.position = value;
        }
    }

    #endregion
    /*
     *  Logic functions
     * 
     */

    protected virtual void Update()
    {
        if (this.InProcess)
            return;



        if (this._states.Count > 0)
        {
            UnitState state = _states[0];
            this.Process(state);
            return;
        }


        this.Process(UnitState.s_SetTasks);
    }

    Dictionary<string, MethodInfo> invokes;

    public void Process(UnitState State, string meth = null)
    {
        if (invokes == null)
            invokes = new Dictionary<string, MethodInfo>();

        this.InProcess = true;
        this.State = State;

        string state;
        if (meth == null)
            state = State.ToString();
        else
            state = meth;

        if (invokes.ContainsKey(state))
            invokes[state].Invoke(this, null);
        else
        {
            MethodInfo mi = this.GetType().GetMethod(state, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (mi != null)
            {
                invokes.Add(state, mi);
                mi.Invoke(this, null);
            }
            else
                Debug.LogError("In " + this.ID + " no function: " + state);
        }
    }

    public void Finish()
    {
        if (this._states.Count > 0)
            this._states.RemoveAt(0);

        this.InProcess = false;
    }

    void OnTriggerEnter(Collider other)
    {
//        this._islandNumber = other.GetComponent<Island>().IslandNumber;
    }

    protected virtual void adminTasks()
    {
        this.State = UnitState.s_WaitTask;
        this.InProcess = true;
    }

    /*
     * Walk Logic
     * 
     */
    public virtual void GoTo(Vector3 point, bool fast = false, Action _del = null)
    {
        if(fast)
        {
            this.gameObject.SetActive(false);
            this.position = point;
            this.gameObject.SetActive(true);
            this.IslandNumber = IM.GetIslandNumber(this.position);

            if (_del != null)
            {
                _del();
 //               this.Delegate = null;
            }

            return;
        }

        if (this._points.Count > 0)
        {
            if (!Vector3.Equals(this._points.Last(), point))
            {
                this._points.Add(point);
                this.States = UnitState.s_WalkTo;
            }
        }
        else if (!Vector3.Equals(this.position, point))
        {
            this._points.Add(point);
            this.States = UnitState.s_WalkTo;
        }

        if (_del != null)
        {
            this.Delegate = _del;
            this.States = UnitState.s_ContinueEvent;
        }
        else
            this.Delegate = null;

        this.Process(this._states[0]);
    }

    public virtual void GoTo(SkyObject point, bool fast = false, Action _del = null)
    {
        Vector3 tar;

        if (point.GetComponent<Island>() != null)
        {
            Island isl = point.GetComponent<Island>();

            /*if (isl.LockLeft)
                tar = isl.EnterLeft.position;
            else if (isl.LockRight)
                tar = isl.EnterRight.position;
            else
            {
                if (this.position.x < isl.position.x)
                    tar = isl.EnterLeft.position;
                else
                    tar = isl.EnterRight.position;
            }*/
        }
        else
        {
            SceneObject target = point.GetComponent<SceneObject>();
            if(target != null && target.IslandNumber > 0)
            {
                float deltaX = target.position.x > IM.Islands[target.IslandNumber].position.x ? -1.0f : 1.0f;
                tar = new Vector3(point.position.x + deltaX, point.position.y, point.position.z - 1.5f);
            }
            else
                tar = point.position;
        }
            

        //GoTo(tar, fast, _del);
    }

    #region StatePart

    protected virtual void s_Activation()
    {
        this.Finish();
    }

    protected virtual void s_Inactivation()
    {
        this.Finish();
    }

    protected virtual void s_SetTasks()
    {
        this.adminTasks();
    }

    protected virtual void s_Rest()
    {
        float restTime = UnityEngine.Random.Range(5, 10);
        this.Invoke("Finish", restTime);
    }

    protected virtual void s_Prepare()
    {
        this.Finish();
    }

    protected virtual void s_WalkTo()
    {
        StartCoroutine(PathMove());
    }

    public virtual IEnumerator PathMove()
    {
        this.Agent.isStopped = true;
        yield return null;

        if(this._points.Count == 0)
        {
            this.Finish();
            yield break;
        }

        this._pointToGo = this._points[0];
        this.Agent.SetDestination(this._pointToGo);
        yield return null;

        this.Agent.isStopped = false;
        yield return null;

        while (this.Agent.remainingDistance > this.Agent.stoppingDistance)
        {
            yield return null;
        }
        this._points.RemoveAt(0);
        this.Finish();
    }

    protected virtual void s_Fly()
    {
        this.Finish();
    }

    protected virtual void s_StartWork()
    {
        this.Finish();
    }

    protected virtual void s_Work()
    {
        this.Finish();
    }

    protected virtual void s_Die()
    {
        this.Finish();
    }

    protected virtual void s_GoHome()
    {
        this.Finish();
    }

    protected virtual void s_WalkToTarget()
    {
        this.Finish();
    }

    protected virtual void s_WaitTask()
    {
        this.Finish();
    }

    protected virtual void s_TakeResource()
    {
        this.Finish();
    }

    protected virtual void s_PutResource()
    {
        this.Finish();
    }

    protected virtual void s_ContinueEvent()
    {
        this.Delegate?.Invoke();
        this.Delegate = null;

        this.Finish();
    }

    protected virtual void s_CompleteFly()
    {
        this.Finish();
    }

    protected virtual void s_Talk()
    {
        this.Finish();
    }

    protected virtual void s_Use()
    {
        this.Finish();
    }

    #endregion
}
