using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MoveCameraToPoint : GameEvent
    {

        bool fast = false;
        CameraPoint point;
        float time = 2f;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MoveCameraToPoint";

            if (node["fast"] != null)
                fast = node["fast"].AsBool;


            if (node["time"] != null)
                time = node["time"].AsFloat;

            point = CameraPoint.CreateFomJSON(node["CameraPoint"]);
        }

        public override void Start()
        {
            if (fast)
            {
                GM.Camera.MoveToPoint(point);
                End();
                return;
            }
            else
                GM.Camera.MoveToPointAsynk(point, End, time: time);
        }

        #region static
        public static MoveCameraToPoint Create(CameraPoint point, bool fast = false, float time = 2f)
        {
            MoveCameraToPoint temp = new MoveCameraToPoint();
            temp.ID = "MoveCameraToPoint";

            temp.fast = fast;
            temp.time = time;
            temp.point = point;

            return temp;
        }
        #endregion
    }
}