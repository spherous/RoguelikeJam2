using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class CameraFollow : MonoBehaviour
{
    private Transform cameraLeader;
    public Camera cam;

    public float zoomSpeed;
    private Transform pos;
    private Vector3 offset;
    public float followSpeed;
    private float elapsedTime;
    private float cycleStartTime;
    private void Awake()
    {
        cameraLeader = GameObject.FindObjectOfType<CameraMovementController>().transform;
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
}
