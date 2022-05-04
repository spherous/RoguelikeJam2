using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform cameraLeader;
    public Camera cam;

    public float zoomSpeed;
    private Transform pos;
    private Vector3 offset;
    public float followSpeed;
    private float elapsedTime;
    private float cycleStartTime;
    private void Awake()
    {
        offset = new Vector3 (0,0,-10);
        pos = transform;
    }
    
    private void Start()
    {
        cycleStartTime = Time.time;
    }
    private void FixedUpdate()
    {
        Follow();
    }
    
    void Follow()
    {
        elapsedTime = Time.time - cycleStartTime;
        transform.position = Vector3.Lerp(pos.position + offset, cameraLeader.position + offset, elapsedTime / followSpeed);
        {
            pos = transform;
            cycleStartTime = Time.time;
        }
    }
    void Update() 
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y < 0)
        {
            cam.orthographicSize += zoomSpeed;
        }
        else if (scroll.y > 0)
        {
            cam.orthographicSize -= zoomSpeed;
        }
        
    }
}
