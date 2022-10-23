using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraControll : MonoBehaviour {

    const int orthographicSizeMin = 2;
    const int orthographicSizeMax = 12;

    public float edge;

    public float _velocity;

    public Vector3 _target;
    public float _targetVelocity;
    public Action _del;


    Transform _trans;
    
    Texture2D _texture;
    GUIStyle _panelStyle;

    // Use this for initialization
    void Start () {

        this._trans = this.transform;
    }
	
	// Update is called once per frame
	void Update () {

        if (GM.GameState == Lodkod.GameState.War || GM.GameState == Lodkod.GameState.EventWorking || GM.GameState == Lodkod.GameState.Action || GM.GameState == Lodkod.GameState.Reacting)
            return;

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            _trans.Translate(_velocity * Time.deltaTime * Input.GetAxisRaw("Horizontal"), 0f, 0f);
            
        }


        if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
        {
            _trans.position = new Vector3(_trans.position.x, _trans.position.y + _velocity * Time.deltaTime * Input.GetAxisRaw("Vertical"), _trans.position.z);
            //_trans.Translate(0f, _velocity * Time.deltaTime * Input.GetAxisRaw("Vertical"), 0f);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
        {
            float scale = Camera.main.transform.position.z;
            scale -= 1;
            if (scale < -145)
                scale = -145;
            
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, scale);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
        {
            float scale = Camera.main.transform.position.z;
            scale += 1;
            if (scale > -25)
                scale = -25;
            
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, scale);
        }
       

        if (Input.mousePosition.x > Screen.width - edge && Input.mousePosition.x < Screen.width)
        {
            _trans.Translate(_velocity * Time.deltaTime * ((Input.mousePosition.x-(Screen.width-edge))/edge), 0f, 0f);
        }

        if (Input.mousePosition.x < edge && Input.mousePosition.x > 0)
        {
            _trans.Translate(_velocity * Time.deltaTime * -((edge - Input.mousePosition.x) / edge), 0f, 0f);
        }

        if (Input.mousePosition.y < edge && Input.mousePosition.y>0)
        {
            _trans.Translate(0f, _velocity * Time.deltaTime * -((edge - Input.mousePosition.y) / edge), 0f);
        }

        if (Input.mousePosition.y > Screen.height - edge && Input.mousePosition.y < Screen.height)
        {
            _trans.Translate(0f, _velocity * Time.deltaTime * ((Input.mousePosition.y - (Screen.height-edge)) / edge), 0f);
        }


        //AspectCamera();
    }
    
    public void AspectCamera()
    {
//        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax);
//        affecterdFog.orthographicSize = Camera.main.orthographicSize;

        float height = 2f * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        if (Camera.main.transform.position.x < width / 2)
            _trans.position = new Vector3(width / 2, _trans.position.y, _trans.position.z);

        if (Camera.main.transform.position.x > GM.MapSize.x - width / 2)
            _trans.position = new Vector3(GM.MapSize.x - width / 2, _trans.position.y, _trans.position.z);

        if (Camera.main.transform.position.y < height / 2)
            _trans.position = new Vector3(_trans.position.x, height / 2, _trans.position.z);

        if (Camera.main.transform.position.y > GM.MapSize.y - height / 2)
            _trans.position = new Vector3(_trans.position.x, GM.MapSize.y - height / 2, _trans.position.z);
    }

    public void CameraDragWith(Vector2 points)
    {
        _trans.Translate(points.x, points.y, 0f);
        //AspectCamera();
    }

    public void moveTo(Vector3 point)
    {
        Camera.main.transform.position = point;
        //AspectCamera();
    }

    public void moveToAsynk(Vector3 point, Action del, float time = 2f)
    {
        this._target = point;
        this._targetVelocity = Vector3.Distance(Camera.main.transform.position, this._target) / time;
        this._del = del;

        StartCoroutine(moveToCoroutine());
    }

    IEnumerator moveToCoroutine()
    {
        yield return StartCoroutine(moveToPosition());

        if(this._del != null)
            this._del();
    }

    public IEnumerator moveToPosition()
    {
        while(!Mathf.Approximately(Camera.main.transform.position.magnitude, this._target.magnitude))
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, this._target, this._targetVelocity*Time.deltaTime);
            yield return null;
        }

        //AspectCamera();
    }

}