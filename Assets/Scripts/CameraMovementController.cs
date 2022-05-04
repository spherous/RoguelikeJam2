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
    [ShowInInspector, ReadOnly] private float horizontalInput;
    [ShowInInspector, ReadOnly] private float verticalInput;
    public float timeToMaxSpeed;
    private float velocityX;
    private float velocityY;

    private PixelPerfectCamera pixelPerfectCamera;
    
    private void Awake()
    {
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
        pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
    }
    private void Update()
    {
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
    }

    private void FixedUpdate() => rb.velocity = new Vector2(velocityX, velocityY);
    public void MoveHorizontal(CallbackContext context) => horizontalInput = context.ReadValue<float>();
    public void MoveVertical(CallbackContext context) => verticalInput = context.ReadValue<float>();
    public void SetVelocityToZero() => rb.velocity = Vector2.zero;

    void CameraZoom() => 
        pixelPerfectCamera.assetsPPU = Mathf.Clamp(pixelPerfectCamera.assetsPPU + (int)Mouse.current.scroll.ReadValue().y / 60, 32, 128);
}