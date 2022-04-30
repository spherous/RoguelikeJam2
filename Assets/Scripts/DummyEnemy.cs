using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour
{

    public float speed;

    [SerializeField] EnemyList enemyList;
    [SerializeField] Rigidbody2D rb;

    private float lifeTime;
    public float deathRate;

    void Start()
    {
        lifeTime = Time.timeSinceLevelLoad + deathRate;
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad >= lifeTime)
        {
            DestroyThis();
        }
    }
    void FixedUpdate()
    {
        if (rb.velocity.x < speed)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }

    void DestroyThis()
    {
        enemyList.enemyList.Remove(gameObject);
        Destroy(gameObject);
    }
}
