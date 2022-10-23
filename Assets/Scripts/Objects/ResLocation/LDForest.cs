using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class LDForest : SceneObject {

    public static float yExtra = 0.85f;

    public override void HardSet()
    {
        base.HardSet();

        this.ID = "forest" + this.IslandNumber;
    }

}
