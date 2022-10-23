using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MoveCamera : GameEvent
    {

        bool fast = false;
        Vector3 target;
        SkyObject obj;
        float z;
        float time = 2f;

        string ToGo;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MoveCamera";

            if (node["fast"] != null)
                fast = node["fast"].AsBool;

            this.obj = null;
            this.target = Vector3.zero;
            ToGo = "";


            if (node["time"] != null)
                time = node["time"].AsFloat;

            if (node["point"] != null)
            {
                this.target = new Vector3(node["point"]["x"].AsFloat, node["point"]["y"].AsFloat, node["point"]["z"].AsFloat);
            }
            else if (node["to"] != null)
            {
                ToGo = node["to"].Value;
                if (node["z"] != null)
                    z = node["z"].AsFloat;
                else
                    z = -38f;
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);

            if (target == null)
                Debug.LogError("NOT SET CAMERA TARGET");
        }

        public override bool CanActive()
        {
            if (ToGo.Equals(""))
                return true;

            if (ToGo.Equals("self"))
            {
                this.obj = Object;
            }
            else
            {
                this.obj = GM.GetObject(ToGo);
            }

            return base.CanActive();
        }

        public override void Start()
        {
            if (this.target == Vector3.zero)
            {
                SceneObject temp = this.obj as SceneObject;
                if(temp == null)
                {
                    Debug.LogError("Not found object in camera move");
                    End();
                    return;
                }

                if(fast)
                {
                    GM.Camera.MoveToPoint(temp.CameraPoint);
                    End();
                    return;
                }
                else
                    GM.Camera.MoveToPointAsynk(temp.CameraPoint, End, time: time);

                return;
            }
                

            if (fast)
            {
                GM.Camera.moveTo(target);
                End();
            }
            else
            {
                Action del = End;
                GM.Camera.moveToAsynk(target, del, time:time);
            }
        }

        #region static
        public static MoveCamera Create(Vector3 point, bool fast = false,  string to = null, float z = -38f)
        {
            MoveCamera temp = new MoveCamera();
            temp.ID = "MoveCamera";

            temp.fast = fast;
            temp.obj = null;
            temp.target = Vector3.zero;
            temp.ToGo = "";
            temp.z = z;

            if (point != Vector3.zero)
                temp.target = point;
            else if (to != null)
                temp.ToGo = to;

            return temp;
        }
        #endregion
    }
}