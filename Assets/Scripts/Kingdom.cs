using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Kingdom : MonoBehaviour
{
    private LoseScreen loseScreen;
    private GameManager gameManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayableDirector director;
    public List<Sprite> sprites = new List<Sprite>();
    public double loopStartTime;
    bool loseScreenOpened = false;

    private void Awake()
    {
        loseScreen = GameObject.FindObjectOfType<LoseScreen>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        gameManager.onHealthChanged += OnHealthChanged;    
    }

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

    private void OnHealthChanged(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP == 0)
        {
            GameOver();
            return;
        }

        int spriteIndexToUse = (15f - (15f * percent)).Floor();
        spriteRenderer.sprite = sprites[spriteIndexToUse];
    }
}