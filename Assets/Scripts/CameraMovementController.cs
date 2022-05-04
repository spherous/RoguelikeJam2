using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraMovementController : MonoBehaviour
{
    private GridGenerator gridGenerator;
    [SerializeField] Rigidbody2D rb;
    public float maxSpeed;
    private float speedMax;
    [ShowInInspector, ReadOnly] private float horizontalInput;
    [ShowInInspector, ReadOnly] private float verticalInput;
    public float timeToMaxSpeed;
    private float velocityX;
    private float velocityY;
    private int hInput;
    private int vInput;

    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    public float zoomLevel;

    public float maxZoomIn;
    public float maxZoomOut;
    public float zoomSpeed;
    private int scrollWheelInput;
    
    private void Awake()
    {
        speedMax = maxSpeed;
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();

    }
    private void Update()
    {

        float acceleration = maxSpeed / timeToMaxSpeed;
        float h = acceleration * hInput;
        float v = acceleration * vInput;

        var boundingTiles = gridGenerator.GetBoundingBox();

        if(hInput != 0) 
            velocityX += h * Time.deltaTime; 
        else
            velocityX -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityX, 0);

        if(vInput != 0) 
            velocityY += v * Time.deltaTime;
        else 
            velocityY -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityY, 0);

        velocityY = Mathf.Clamp(velocityY, -maxSpeed, maxSpeed);
        velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));

        Vector2 lowerBound = new Vector2(boundingTiles.bottomLeft.transform.position.x, boundingTiles.bottomLeft.transform.position.y);
        Vector2 upperBound = new Vector2(boundingTiles.topRight.transform.position.x, boundingTiles.topRight.transform.position.y);
        
        if (transform.position.y > upperBound.y)
        {
            transform.position = new Vector3(transform.position.x, upperBound.y, transform.position.z);
        }
        else if (transform.position.y < lowerBound.y)
        {
            transform.position = new Vector3(transform.position.x, lowerBound.y, transform.position.z);
        }

        if (transform.position.x > upperBound.x)
        {
            transform.position = new Vector3(upperBound.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < lowerBound.x)
        {
            transform.position = new Vector3(lowerBound.x, transform.position.y, transform.position.z);
        }

        CameraZoom();


    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(velocityX,velocityY);
    }
    public void MoveHorizontal(CallbackContext context) 
    {

        horizontalInput = context.ReadValue<float>();
        if(horizontalInput > 0.5f) hInput=1;
        else if(horizontalInput < -0.5) hInput=-1;
        else hInput=0;
    }
    public void MoveVertical(CallbackContext context)
    {

        verticalInput = context.ReadValue<float>();
        if(verticalInput > 0.5f)
            vInput=1;
        else if(verticalInput < -0.5) 
            vInput=-1;
        else 
            vInput=0;
    }
    public void SetVelocityToZero() => rb.velocity = Vector2.zero;

    void CameraZoom()
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y > 0)
            scrollWheelInput = 1;
        else if (scroll.y < 0)
            scrollWheelInput = -1;
        else
            scrollWheelInput = 0;


	    if (scrollWheelInput != 0) 
        {
            zoomLevel = Mathf.Clamp((zoomLevel + zoomLevel/zoomSpeed * scrollWheelInput), maxZoomOut, maxZoomIn);
		    pixelPerfectCamera.assetsPPU = Mathf.RoundToInt(zoomLevel);
        }   
    }

}