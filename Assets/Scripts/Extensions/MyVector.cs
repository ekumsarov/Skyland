using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public static class MyVector 
{
    public static Vector2 ParseVector(string vector)
    {
        if (vector.Length == 3)
            vector = "0" + vector;
        else if (vector.Length > 4 || vector.Length < 3)
            return Vector2.zero;

        return new Vector2(int.Parse(vector.Substring(0, 2)), int.Parse(vector.Substring(2, 2)));
    }

    public static Vector3 ParseFromJSON(JSONNode data)
    {
        float x = 0;
        if (data["X"] != null)
            x = data["X"].AsFloat;
        else if(data["x"] != null)
            x = data["x"].AsFloat;

        float y = 0;
        if (data["Y"] != null)
            y = data["Y"].AsFloat;
        else if (data["y"] != null)
            y = data["y"].AsFloat;

        float z = 0;
        if (data["Z"] != null)
            x = data["Z"].AsFloat;
        else if (data["z"] != null)
            x = data["z"].AsFloat;

        return new Vector3(x, y, z);
    }

    public static JSONNode ParseToJSON(Vector3 vector)
    {
        JSONNode point = new JSONClass();

        point.Add("X", vector.x.ToString());
        point.Add("Y", vector.y.ToString());
        point.Add("Z", vector.z.ToString());

        return point;
    }
}
