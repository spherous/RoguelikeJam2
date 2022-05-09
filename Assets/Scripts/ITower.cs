using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    Index location {get; set;}
    float range {get; set;}

    void Disable();
    void Enable();
}