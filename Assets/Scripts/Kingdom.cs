using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Kingdom : MonoBehaviour
{
    private LoseScreen loseScreen;
    [SerializeField] private PlayableDirector director;
    public double loopStartTime;
    bool loseScreenOpened = false;

    private void Awake() => loseScreen = GameObject.FindObjectOfType<LoseScreen>();

    public void StartLoop()
    {
        director.time = loopStartTime;
        if(!loseScreenOpened)
        {
            loseScreenOpened = true;
            loseScreen.Display();
        }
    }
    public void GameOver()
    {
        director.time = 0;
        director.Play();
    }
}