using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;

    [SerializeField] Rigidbody2D rb;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.x < speed)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }
}
