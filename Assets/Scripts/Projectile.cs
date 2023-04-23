using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    public float speed;
    private float damage;
    private int remainingChains;
    public float chainRadius;
    public List<Collider2D> hitCos = new List<Collider2D>();
    public LayerMask enemyMask;
    public float aliveTime;
    private float dieAtTime;

    private void Update()
    {
        if(Time.timeSinceLevelLoad >= dieAtTime)
            Destroy(gameObject);
    }

    public void Fire(Transform targetTransform, float damage, int chainCount = 0)
    {
        this.damage = damage;
        this.remainingChains = chainCount;
        dieAtTime = Time.timeSinceLevelLoad + aliveTime;
        Vector2 directionFromSelfToTarget = (targetTransform.position - transform.position).normalized;
        Debug.DrawLine(transform.position, targetTransform.position, Color.red, 1);
        transform.up = directionFromSelfToTarget;
        rb.velocity = Vector2.zero;
        rb.AddForce(directionFromSelfToTarget * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<IHealth>(out IHealth otherHP))
        {
            otherHP.TakeDamage(damage);

            if(remainingChains == 0)
            {
                Destroy(gameObject);
                return;
            }

            TryChain(other);
        }
    }

    void TryChain(Collider2D lastHitCol)
    {
        Collider2D[] allNearby = Physics2D.OverlapCircleAll(transform.position, chainRadius, enemyMask);

        if(!allNearby.Any())
        {
            Destroy(gameObject);
            return;
        }

        if(lastHitCol != null)
        {
            hitCos.Add(lastHitCol);
            Physics2D.IgnoreCollision(col, lastHitCol);
        }

        foreach(Collider2D nearby in allNearby)
        {
            if(nearby == col || hitCos.Contains(nearby) || nearby == lastHitCol)
                continue;

            Fire(nearby.transform, damage, remainingChains - 1);
            return;
        }

        // didn't find a target
        Destroy(gameObject);
        return;
    }
}