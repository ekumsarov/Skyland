using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyObject : MonoBehaviour {

    public float timeDestroy;

	// Update is called once per frame
	void Update () {
        timeDestroy -= Time.deltaTime;

        if (timeDestroy <= 0)
            Destroy(gameObject);
	}
}
