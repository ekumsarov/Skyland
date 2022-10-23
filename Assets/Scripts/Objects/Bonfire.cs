using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class Bonfire : SceneObject {

    public bool Active = false;

    public override void Actioned(string ev = "")
    {
        Active = true;
        //GIM.OpenBonfireMenu(this.IslandNumber);
    }
}
