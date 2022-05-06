using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeOutline : MonoBehaviour
{
    public bool exists;
    void Update()
    {
        if (exists) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = Vector3.zero;
    }

    public void Move(Transform tileTransform)
    {
        transform.position = tileTransform.position;
        exists = true;
    }
    public void Remove() => exists = false;
}
 