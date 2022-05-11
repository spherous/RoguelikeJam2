using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Kingdom : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    public double loopStartTime;

    public void StartLoop()
    {
        director.time = loopStartTime;
    }
    public void GameOver()
    {
        director.time = 0;
        director.Play();
    }
}
