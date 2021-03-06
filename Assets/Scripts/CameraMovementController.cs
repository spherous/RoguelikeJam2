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
    [SerializeField] Camera cam;
    public float maxSpeed;
    [ShowInInspector, ReadOnly] private float horizontalInput;
    [ShowInInspector, ReadOnly] private float verticalInput;
    public float timeToMaxSpeed;
    private float velocityX;
    private float velocityY;
    private Vector2 mousePosition;

    private PixelPerfectCamera pixelPerfectCamera;
    private bool mmbHeld = false;
    private Vector3 mouseOrigin;
    private Vector3 transformOrigin;
    private Vector3 destination;
    private bool movingVertical;
    private bool movingHorizontal;
    private void Awake()
    {
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
        pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
    }
    private void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        if(!movingHorizontal && !movingVertical)
            mouseOnSideMove();

        float acceleration = maxSpeed / timeToMaxSpeed;
        float h = acceleration * horizontalInput;
        float v = acceleration * verticalInput;

        var boundingTiles = gridGenerator.GetBoundingBox();

        if(horizontalInput != 0)
            velocityX += h * Time.deltaTime; 
        else
            velocityX -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityX, 0);

        if(verticalInput != 0) 
            velocityY += v * Time.deltaTime;
        else 
            velocityY -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityY, 0);

        velocityY = Mathf.Clamp(velocityY, -maxSpeed, maxSpeed);
        velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));

        Vector2 lowerBound = boundingTiles.bottomLeft.transform.position;
        Vector2 upperBound = boundingTiles.topRight.transform.position;

        if(transform.position.y > upperBound.y)
            transform.position = new Vector3(transform.position.x, upperBound.y, transform.position.z);
        else if(transform.position.y < lowerBound.y)
            transform.position = new Vector3(transform.position.x, lowerBound.y, transform.position.z);

        if(transform.position.x > upperBound.x)
            transform.position = new Vector3(upperBound.x, transform.position.y, transform.position.z);
        else if(transform.position.x < lowerBound.x)
            transform.position = new Vector3(lowerBound.x, transform.position.y, transform.position.z);

        CameraZoom();

        if (mmbHeld)
            mmbMove();
    }

    private void FixedUpdate() => rb.velocity = new Vector2(velocityX, velocityY);
    public void MoveHorizontal(CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
        if (context.started)
            movingHorizontal = true;
        else if (context.canceled)
            movingHorizontal = false;
    }
    public void MoveVertical(CallbackContext context)
    {
        verticalInput = context.ReadValue<float>();
        if (context.started)
            movingVertical = true;
        else if (context.canceled)
            movingVertical = false;
    }
    public void SetVelocityToZero() => rb.velocity = Vector2.zero;

    void CameraZoom() => 
        pixelPerfectCamera.assetsPPU = Mathf.Clamp(pixelPerfectCamera.assetsPPU + (int)Mouse.current.scroll.ReadValue().y / 60, 32, 160);

    
    public void MMB(CallbackContext context)
    { 
        if (context.started)
        {
            mouseOrigin = mousePosition;
            transformOrigin = transform.position;
            mmbHeld = true;
        }
        if (context.canceled)
        {
            mmbHeld = false;
        }
    }

    void mmbMove()
    {
        destination = mousePosition;
        transform.position = transformOrigin + (mouseOrigin - destination) / pixelPerfectCamera.assetsPPU;
    }
    
    void mouseOnSideMove()
    {
        if (mousePosition.x > Screen.width-20)
            horizontalInput = 1;
        else if (mousePosition.x < 20)
            horizontalInput = -1;
        else
            horizontalInput = 0;


        if (mousePosition.y > Screen.height-20)
            verticalInput = 1;
        else if (mousePosition.y < 20)
            verticalInput = -1;
        else
            verticalInput = 0;
    }
}   