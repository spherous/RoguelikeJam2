using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Web : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private BoxCollider2D boxCollider;
    public List<WebTower> connectedTowers = new List<WebTower>();

    public void Connect(WebTower a, WebTower b, Transform aPoint, Transform bPoint)
    {
        Vector3 webToA = aPoint.position - transform.position;
        Vector3 webToB = bPoint.position - transform.position;
        line.SetPosition(0, webToA);
        line.SetPosition(1, webToB);
        connectedTowers.Add(a);
        connectedTowers.Add(b);

        // This hack sucks on diagonal lines, but it works for now.
        Bounds bounds = line.bounds;
        boxCollider.offset = bounds.center - transform.position;
        boxCollider.size = bounds.size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<IEnemy>(out IEnemy enemy))
        {
            enemy.TakeDamage(100);
        }
    }
}