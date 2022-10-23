using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using SimpleJSON;
using System;

namespace Lodkod
{
    public class SceneObject : SkyObject
    {

        #region Base
        /*
         * описание объекта
         */

        [SerializeField]
        private CameraPoint _position;
        public CameraPoint CameraPoint
        {
            get { return this._position; }
        }

        // int types
        private int _type;  // тип юнита(объекта) в данной игре
        private int _side; // сторона объекков
        public int _islandNumber; // номер острова
        protected bool _unit = false;

        private double _HP; // жизни
        protected bool _visible = true;
        protected bool _lock = false;
        
        /*
         *
         * определение всех данных типов как опции для доступа и чтение
         *
         */

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

            _sub = Subscriber.Create(this);
        }


        public virtual bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if(value && this._lock == false && this.gameObject.activeSelf == false)
                {
                    this._visible = value;
                    this.gameObject.SetActive(true);
                    return;
                }

                if (_visible == value)
                    return;

                if (value && this._lock)
                    return;

                _visible = value;
                this.gameObject.SetActive(value);
            }
        }

        public virtual bool Lock
        {
            get { return this._lock; }
            set
            {
                if (this._lock == value)
                    return;

                if (value == false)
                    this.gameObject.SetActive(value);

                this._lock = value;
            }
        }


        // функции для записи

        /**
        * запись стороны
        */
        public int Side
        {
            get { return _side; }
            set { _side = value; }
        }
        /**
         * запись типа
         */
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /*
         * sets the HP
         */
        public double HP
        {
            get { return _HP; }
            set { _HP = value; }
        }
        public bool IsUnit
        {
            get { return _unit; }
        }
        /**
         * запись номера острова
         */
        public int IslandNumber
        {
            get { return _islandNumber; }
            set { _islandNumber = value; }
        }
        /**
         * запись Позиции
         */
        public override Vector3 position
        {
            get
            {
                if (_trans == null)
                    _trans = this.transform;

                return _trans.position;
            }
            set
            {
                if (_trans == null)
                    _trans = this.transform;

                _trans.position = new Vector3(value.x, value.y, _trans.position.z);
            }
        }

        public  Quaternion Rotation
        {
            get
            {
                if (_trans == null)
                    _trans = this.transform;

                return _trans.rotation;
            }
            set
            {
                if (_trans == null)
                    _trans = this.transform;

                _trans.rotation = value;
            }
        }

        public Vector2 VectorPosition
        {
            get
            {
                return new Vector2(this.position.x, this.position.z);
            }
        }


        ////////////////////////////////
        // Activity part

        #endregion

        #region MapLocation

        [SerializeField]
        public LocationPanel LocationPanel;
        protected bool _locked = true;
        public bool IsLocked
        {
            get { return this._locked; }
        }

        public void LockLocation(bool _lock = true)
        {
            this._locked = _lock;
            if(this.LocationPanel != null)
                this.LocationPanel.gameObject.SetActive(!_lock);
        }


        #endregion

        #region Icon Object

        public virtual void AddIcon(string icon)
        {
            IconObject temp = GM.GetObject(icon) as IconObject;
            if(temp == null)
            {
                Debug.LogError("Not Found Icon: " + icon);
                return;
            }

            temp.SetObjectParent(this);
        }

        public virtual void AddIcon(IconObject icon)
        {
            icon.SetObjectParent(this);
        }

        public void LockIcon(string icon, bool _lock = true)
        {

        }

        #endregion


        #region Object Move

        public override Vector3 ScreePoint
        {
            get
            {
                if (_trans == null)
                    _trans = gameObject.transform;

                return Camera.main.WorldToScreenPoint(_trans.position);
            }
        }


        public override void PlaceObject(Vector3 point, bool fast = true, Action deli = null, float time = 2f)
        {
            if (IsUnit)
            {
                this.gameObject.GetComponent<Unit>().GoTo(point, fast, deli);
                return;
            }

            if (!Visible)
                this.Visible = true;

            if (fast)
            {
                this.transform.position = point;
                this.IslandNumber = IM.GetIslandNumber(this.position);

                if (deli != null)
                    deli();

                return;
            }

            target = point;
            targetVelocity = Vector3.Distance(this.transform.position, target) / time;
            _del = deli;

            StartCoroutine(moveToCoroutine());
        }

        public override void PlaceObject(SkyObject point, bool fast = true, Action deli = null, float time = 2f)
        {
            if (IsUnit)
            {
                this.gameObject.GetComponent<Unit>().GoTo(point, fast, deli);
                return;
            }

            if (!Visible)
                this.Visible = true;

            Vector3 tar;

            tar = point.position;

            if (fast)
            {
                this.transform.position = tar;
                this.IslandNumber = IM.GetIslandNumber(this.position);

                if (deli != null)
                    deli();

                return;
            }

            target = tar;
            targetVelocity = Vector3.Distance(this.transform.position, target) / time;
            _del = deli;

            StartCoroutine(moveToPosition());
        }

        public override IEnumerator moveToCoroutine()
        {
            yield return StartCoroutine(moveToPosition());
            if (_del != null)
            {
                _del();
                _del = null;
            }
        }

        public override IEnumerator moveToPosition()
        {
            while (!Mathf.Approximately(this.transform.position.magnitude, target.magnitude))
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, target, targetVelocity * Time.deltaTime);
                yield return null;
            }

            this.IslandNumber = IM.GetIslandNumber(this.position);
            //AspectCamera();
        }

        #endregion
    }
}


