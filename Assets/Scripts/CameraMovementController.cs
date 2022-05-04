using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class CameraMovementController : MonoBehaviour
{
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
    private void Awake()
    {
        speedMax = maxSpeed;

    }
    private void Update()
    {

        float acceleration = maxSpeed / timeToMaxSpeed;
        float h = acceleration * hInput;
        float v = acceleration * vInput;

        if(hInput != 0) velocityX += h * Time.deltaTime; 
        else {
            velocityX -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityX, 0);
            }
        if(vInput != 0) velocityY += v * Time.deltaTime;
        else {
            velocityY -= Mathf.Clamp(-acceleration * Time.deltaTime, velocityY, 0);
            }

        velocityY = Mathf.Clamp(velocityY, -maxSpeed, maxSpeed);
        velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));


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
        if(verticalInput > 0.5f) vInput=1;
        else if(verticalInput < -0.5) vInput=-1;
        else vInput=0;
    }
    public void SetVelocityToZero() => rb.velocity = Vector2.zero;

}