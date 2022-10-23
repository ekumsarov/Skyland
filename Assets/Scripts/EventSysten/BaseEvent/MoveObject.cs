using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MoveObject : GameEvent
    {
        SkyObject obj;
        string ObjID;
        Vector3 target;
        SkyObject oTarget;
        bool fast;
        bool force;
        float moveTime;
        string iSide;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MoveObject";

            ObjID = "self";
            if (node["ID"] != null)
                ObjID = node["ID"].Value;

            fast = true;
            if (node["fast"] != null)
                fast = node["fast"].AsBool;

            force = false;
            if (node["force"] != null)
                fast = node["force"].AsBool;

            moveTime = 2f;
            if (node["MoveTime"] != null)
                moveTime = node["MoveTime"].AsFloat;

            obj = GM.GetObject(ObjID);

            oTarget = null;
            if (node["point"] != null)
                this.target = new Vector3(node["point"]["x"].AsFloat, node["point"]["y"].AsFloat, -15f);
            else if (node["to"] != null)
            {
                string objectRecognize = node["to"].Value;

                if (objectRecognize.Substring(0, 1) == "i")
                {
                    Island isl = GM.GetObject(objectRecognize).GetComponent<Island>();

                    this.target = isl.BoatPlace;
                }
                else
                    oTarget = GetObject(objectRecognize);
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            Action del = null;

            if (!force)
                del = End;

            if (oTarget != null)
                obj.PlaceObject(oTarget, fast, del, moveTime);
            else
                obj.PlaceObject(target, fast, del, moveTime);

            if (force)
                End();
        }



        #region static
        public static MoveObject Create(string _id, Vector3 point, string to = null, bool fast = true, bool force = false, float moveTime = 2.0f, string side = "left")
        {
            MoveObject temp = new MoveObject();
            temp.ID = "MoveObject";
            
            temp.obj = temp.GetObject(_id);
            temp.force = force;
            temp.fast = fast;

            if (point != Vector3.zero)
                temp.target = point;
            else
            {
                temp.oTarget = temp.GetObject(to);
                /*
                string objectRecognize = to;

                if (objectRecognize.Substring(0, 1) == "i")
                {
                    Island isl = GM.GetObject(objectRecognize).GetComponent<Island>();

                    if (side.Equals("left") && !isl.LockLeft)
                        temp.target = isl.EnterLeft.position;
                    else if (side.Equals("left") && isl.LockLeft)
                        temp.target = isl.EnterRight.position;
                    else if (side.Equals("right") && !isl.LockRight)
                        temp.target = isl.EnterRight.position;
                    else if (side.Equals("right") && isl.LockRight)
                        temp.target = isl.EnterLeft.position;
                    else if (side.Equals("near"))
                    {
                        if (isl.LockLeft)
                            temp.target = isl.EnterLeft.position;
                        else if (isl.LockRight)
                            temp.target = isl.EnterRight.position;
                        else
                        {
                            if (temp.obj.position.x < isl.position.x)
                                temp.target = isl.EnterLeft.position;
                            else
                                temp.target = isl.EnterRight.position;
                        }
                    }

                }
                else
                    temp.oTarget = temp.GetObject(objectRecognize);
                    */
            }


            return temp;
        }
        #endregion
    }
}