using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using SimpleJSON;
using System.Linq;
using System;

namespace Lodkod
{
    public class SkyObject : MonoBehaviour, ObjectID
    {
        #region Base
        /*
         * описание объекта
         */
        string ObjectID = "nil";

        public string ID
        {
            get { return ObjectID; }
            set { this.ObjectID = value; }
        }

        public void SafeCall(string methodName, int select = -1)
        {
            if (string.IsNullOrEmpty(methodName)) return;
            MethodInfo mi = this.GetType().GetMethod(methodName);
            if (mi != null)
            {
                if (select == -1)
                    mi.Invoke(this, null);
                else
                    mi.Invoke(this, new object[] { select });
            }
            else
                Debug.LogError(ID + ": Can't find method " + methodName);
        }

        public void SafeCall(string methodName, Action del = null)
        {
            if (string.IsNullOrEmpty(methodName)) return;
            MethodInfo mi = this.GetType().GetMethod(methodName);
            if (mi != null)
            {
                if (del == null)
                    mi.Invoke(this, null);
                else
                    mi.Invoke(this, new object[] { del });
            }
            else
                Debug.LogError(ID + ": Can't find method " + methodName);
        }

        protected Transform _trans;

        ////////////////////////////////
        // Activity part

        /**
         * Initialize LDObject
         */
        protected bool InitBased = false;
        public bool Initialized
        {
            get { return InitBased; }
        }

        public virtual void HardSet()
        {
            if (InitBased)
                return;
            InitBased = true;

            _act = new Activity();
            _act.Object = this;
            _act.initActivity();

            this.Action = new ActionAddition();
            this.Action.parent = this;

            this.MainEvent = "Event";

            _sub = Subscriber.Create(this);
        }
        #endregion

        #region Notify

        protected Subscriber _sub;

        public void Subscibe()
        {
            _sub = Subscriber.Create(this);
        }

        public TriggerType Subscribe
        {
            set
            {
                if (_sub == null)
                    _sub = Subscriber.Create(this);

                _sub.AddEvent(value.ToString());
            }
        }

        public TriggerType Unsubscribe
        {
            set
            {
                if (_sub == null)
                    _sub = Subscriber.Create(this);

                _sub.RemoveEvent(value.ToString());
            }
        }

        public void AddSubscriber(TriggerType ev, string objID = "")
        {
            _sub.AddEvent(ev.ToString(), objID);
        }

        public void AddListeningObject(TriggerType ev, string objID)
        {
            _sub.AddListeningObject(ev.ToString(), objID);
        }

        public void RemoveListeningObject(TriggerType ev, string objID)
        {
            _sub.RemoveListeningObject(ev.ToString(), objID);
        }

        public void AddSubscriber(string ev, string objID = "")
        {
            _sub.AddEvent(ev, objID);
        }

        public void RemoveSubscriber(string ev)
        {
            _sub.RemoveEvent(ev);
        }

        public void AddListeningObject(string ev, string objID)
        {
            _sub.AddListeningObject(ev, objID);
        }

        public void RemoveListeningObject(string ev, string objID)
        {
            _sub.RemoveListeningObject(ev, objID);
        }

        public void ClearListeningObject(string ev)
        {
            _sub.ClearListeningObject(ev);
        }
        #endregion

        #region Activity work
        protected Activity _act;
        public Activity Activity
        {
            get { return _act; }
            set { _act = value; }
        }

        public bool CanCallPack(string ID)
        {
            return Activity.CanCallPack(ID);
        }

        public virtual void Actioned(string ev = "")
        {
            if (ev.Equals(""))
                _act.callActivityPack(this.MainEvent);
            else if (_act.CanCallPack(ev))
                _act.callActivityPack(ev);
            else if (_act.CanCallEvent(ev))
                _act.CallEvent(ev);
            else
                this.Action.CallAction(ev);
        }

        string _main;
        public string MainEvent
        {
            get { return _main; }
            set { _main = value; }
        }
        #endregion

        #region ActionsInDialogue
        ActionAddition _actionObject;
        public ActionAddition Action
        {
            get { return this._actionObject; }
            set { this._actionObject = value; }
        }

        public void CallAction(string action)
        {
            this.Action.CallAction(action);
        }

        public Actions GetAction(string action)
        {
            return this.Action.GetAction(action);
        }

        public List<ActionButtonInfo> GetActionList(string action)
        {
            return this.Action.GetActionList(action);
        }

        public void SetupActionParameters(string action)
        {
            this.Action.SetupActionParameters(action);
        }

        public void AddAction(Actions action)
        {
            this.Action.AddAction(action);
        }

        public void ReplaceAction(Actions action)
        {
            this.Action.ReplaceAction(action);
        }

        public void RepmoveAction(string action)
        {
            this.Action.RemoveAction(action);
        }

        public void RepmoveAllActionChoice(string action)
        {
            this.Action.RepmoveAllActionChoice(action);
        }

        public void ChangeActionText(string action, string txt)
        {
            this.Action.ChangeActionText(action, txt);
        }

        public void AddActionChoice(string action, ActionButtonInfo info)
        {
            this.Action.AddActionChoice(action, info);
        }

        public void ReplaceActionChoice(string action, string actID, ActionButtonInfo info)
        {
            this.Action.ReplaceActionChoice(action, actID, info);
        }

        public void RepmoveActionChoice(string action, string actID)
        {
            this.Action.RemoveActionChoice(action, actID);
        }
        #endregion

        #region BattleGroup

        HeroGroup _group;
        public HeroGroup Group
        {
            get
            {
                if (_group == null)
                    _group = new HeroGroup();

                return _group;
            }
        }
        #endregion

        #region Object Move
        protected Vector3 Target;
        protected float targetVelocity;
        protected Action _del;

        public Vector3 target
        {
            get { return Target; }
            set { Target = value; }
        }

        public virtual Transform GetTransform
        {
            get
            {
                if (_trans == null)
                    _trans = gameObject.transform;

                return _trans;
            }
        }

        public virtual Vector3 position
        {
            get
            {
                if (_trans == null)
                    _trans = this.transform;

                return new Vector3(_trans.position.x, _trans.position.y, _trans.position.z);
            }
            set
            {
                if (_trans == null)
                    _trans = this.transform;

                _trans.position = new Vector3(value.x, value.y, _trans.position.z);
            }
        }

        public virtual Vector3 ScreePoint
        {
            get
            {
                if (_trans == null)
                    _trans = this.gameObject.transform;

                return _trans.position;
            }
        }

        public virtual void PlaceObject(Vector3 point, bool fast = true, Action deli = null, float time = 2f)
        {
            if (deli != null)
            {
                deli();
                deli = null;
            }
        }

        public virtual void PlaceObject(SkyObject point, bool fast = true, Action deli = null, float time = 2f)
        {
            if (deli != null)
            {
                deli();
                deli = null;
            }
        }

        public virtual IEnumerator moveToCoroutine()
        {
            yield return null;
        }

        public virtual IEnumerator moveToPosition()
        {
            yield return null;
        }

        #endregion
    }
}