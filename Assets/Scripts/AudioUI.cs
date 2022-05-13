using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class AudioUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioSource source;
    public bool canPlay = true;
    public List<AudioClip> hoverClips = new List<AudioClip>();
    public List<AudioClip> clickClips = new List<AudioClip>();
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(canPlay && hoverClips.Count > 0)
            source.PlayOneShot(hoverClips.ChooseRandom());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(canPlay && clickClips.Count > 0)
            source.PlayOneShot(clickClips.ChooseRandom());
    }
}