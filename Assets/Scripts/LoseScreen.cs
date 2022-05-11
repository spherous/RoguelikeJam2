using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private GroupFader fader;
    [SerializeField] private PlayableDirector director;

    public void Display()
    {
        if(!fader.visible)
            fader.FadeIn();
        
        director.Play();
    }
}