using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform playerTransform;
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
        transform.position = Vector3.Lerp(pos.position + offset, playerTransform.position + offset, elapsedTime / followSpeed);
        {
            pos = transform;
            cycleStartTime = Time.time;
        }
    }
}
