using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandEnter : MonoBehaviour {

    public Island parent;
    public bool left;

    public void OnTriggerEnter(Collider other)
    {
        parent.LockEnter(left);
    }

    public void OnTriggerExit(Collider other)
    {
        parent.LockEnter(left);
    }
}
