using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lodkod;

public class GUIHelper : MonoBehaviour {

    public bool onButton;

    public CameraControll camera;

    float temps;
    bool click;

    Vector2 StartPosition;

	// Use this for initialization
	void Start () {

        onButton = false;

	}
	
    /*
	// Update is called once per frame
	void Update () {
        

        if (Input.GetMouseButtonDown(0) && !click)
        {
            temps = Time.time;
            click = true;
            StartPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }

        if(click)
        {
            Vector2 point = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 newPos = StartPosition - point;

            camera.CameraDragWith(newPos);
            StartPosition = point;
            
        }
        
        if (Input.GetMouseButtonUp(0) && click)
        {
            click = false;
            if (EventSystem.current.IsPointerOverGameObject())
                return;
                

            if (Time.time - temps < 0.2)
                GIM.CloseAllMenu();
        }


        if (Input.GetMouseButtonDown(1))
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {

                if(hit.collider.gameObject.GetComponent<Island>())
                {
                    GM.Player.setPlayerTask(PlayerTask.GoToIsland, hit.collider.gameObject.GetComponent<Island>());
                }
                
            }
        }


    }*/


    public void setOnButton(bool t)
    {
        this.onButton = t;
    }

}
