using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using SimpleJSON;
using System;

[Serializable]
public class CameraPoint
{
    [SerializeField]
    private Vector3 _cameraPoint;
    [SerializeField]
    private Vector3 _cameraRotation;

    public Vector3 Point
    {
        get { return this._cameraPoint; }
    }

    public Vector3 Rotation
    {
        get { return this._cameraRotation; }
    }

    public JSONNode ConvertToJSON()
    {
        JSONNode temp = new JSONClass();

        temp.Add("Point", MyVector.ParseToJSON(this._cameraPoint));
        temp.Add("Rotation", MyVector.ParseToJSON(this._cameraRotation));

        return temp;
    }

    public static CameraPoint CreateFomJSON(JSONNode data)
    {
        CameraPoint temp = new CameraPoint();

        temp._cameraPoint = MyVector.ParseFromJSON(data["Point"]);
        temp._cameraRotation = MyVector.ParseFromJSON(data["Rotation"]);

        return temp;
    }

    // 83 9.9 -13.4

    // 21.9 0 0
}
