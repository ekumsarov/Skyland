using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour {

    public GameObject LoadImage;

	// Use this for initialization
	public void LoadScene (string scene) {

        LoadImage.SetActive(true);
        SceneManager.LoadScene(scene);
	}

}
