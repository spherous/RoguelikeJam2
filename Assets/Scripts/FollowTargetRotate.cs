using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowTargetRotate : MonoBehaviour
{
    public Transform targetTransform;
    public Transform selfTransform;

    public float offset;

    private void Awake()
    {
        selfTransform = gameObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        FollowTarget();
    }
    void FollowTarget()
    {
        Vector2 directionFromSelfToTarget = (targetTransform.position - selfTransform.position).normalized;
        transform.position = offset * (Vector3)directionFromSelfToTarget + selfTransform.position;
        transform.rotation = Quaternion.Euler(Quaternion.LookRotation(transform.forward, directionFromSelfToTarget).eulerAngles);
    }
}
