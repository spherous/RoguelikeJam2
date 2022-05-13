using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

public class CardDisplay : SerializedMonoBehaviour
{
    public ICard card {get; private set;}

    [OdinSerialize] private Dictionary<CardType, List<Sprite>> cardSprites = new Dictionary<CardType, List<Sprite>>();

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI name2Text;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI cost2Text;
    [SerializeField] private Image bg;
    [SerializeField] private Image details;
    [SerializeField] private Image artworkImage;
    [SerializeField] private Image frame;
    [SerializeField] private Image outline;
    [SerializeField] private float playDistance;
    public Vector3 scale;
    
    public bool isDragging;
    private GameObject cardPosition;
    private Hand hand;
    private MouseData mouseData;

    public bool disableAutoOutline = false;

    void Start()
    {
        mouseData = GameObject.FindObjectOfType<MouseData>();
        hand = GameObject.FindObjectOfType<Hand>();
        cardPosition = GameObject.Find("Card Position");
        outline.color = Color.clear;
        playDistance = playDistance*playDistance;
        scale = transform.localScale;
    }

    public void SetCard(ICard newCard)
    {
        card = newCard;

        if(cardSprites.TryGetValue(newCard.type, out List<Sprite> sprites))
        {
            bg.sprite = sprites[0];
            details.sprite = newCard.threadUseDuration > 0 ? sprites[2] : sprites[1];
            frame.sprite = sprites[3];
        }

        nameText.text = newCard.threadUseDuration > 0 ? "" : card.name;
        name2Text.text = newCard.threadUseDuration > 0 ? card.name : "";

        costText.text = $"{card.threadCost}";
        cost2Text.text = newCard.threadUseDuration > 0 ? $"{newCard.threadUseDuration}" : "";
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
    }

    public void PlayCard()
    {
        BuildMode buildMode = GameObject.FindObjectOfType<BuildMode>();
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();
        if(card.threadCost <= threadPool.availableThreads)
        {
            if(card is TowerCard towerCard && !buildMode.buildModeOn)
            {
                transform.localScale = Vector3.zero;
                buildMode.Open(this, BuildModeState.Build);
                return;
            }
            else if(card is TechCard techCard)
            {
                if(techCard.techCardTypes.Contains(TechCardType.MoveATower))
                {
                    transform.localScale = Vector3.zero;
                    buildMode.Open(this, BuildModeState.Relocate);
                    return;
                }
                else if(techCard.playOnTower)
                {
                    transform.localScale = Vector3.zero;
                    buildMode.Open(this, BuildModeState.PlayOnTower);
                    return;
                }
            }
            else if(card is BuffCard buffCard && buffCard.playOnTower)
            {
                transform.localScale = Vector3.zero;
                buildMode.Open(this, BuildModeState.PlayOnTower);
                return;
            }
            else if(card is EnviornmentCard enviornmentCard)
            {
                if(enviornmentCard.playOnTower)
                {
                    transform.localScale = Vector3.zero;
                    buildMode.Open(this, BuildModeState.PlayOnTower);
                    return;
                }
                else if(enviornmentCard.enviroTypes.Any(type => type == EnvironmentType.CreateBuildableTile || type == EnvironmentType.RemoveBuildableTile))
                {
                    transform.localScale = Vector3.zero;
                    buildMode.Open(this, BuildModeState.PlayOnTile);
                    return;
                }
            }
        }
        
        if(card != null && mouseData != null && mouseData.hoveredTile != null && card.TryPlay(mouseData.hoveredTile))
        {
            RemoveFromHand();
            return;
        }
        ReturnCard();
    }

    public void RemoveFromHand()
    {
        if(card.singleUse == true)
        {
            hand.TrashCard(this);
            Destroy(gameObject);
            return;
        }
        hand.RemoveCard(this);
        Destroy(gameObject);
    }

    public void ReturnCard()
    {
        transform.SetSiblingIndex(hand.cardList.IndexOf(this));
        transform.localScale = scale;
        hand.FanCards();
    }

    public void Outline(Color color) => outline.color = color;

    void Update() 
    {
        Vector3 offset = cardPosition.transform.position - transform.position;
        float sqrLen = offset.sqrMagnitude;

        if(disableAutoOutline)
            return;
        if(isDragging)
        {
            if(sqrLen > playDistance && isDragging)
                Outline(Color.green);
            else if(sqrLen < playDistance && isDragging)
                Outline(Color.white);
            return;
        }
        if(!isDragging && !GameObject.FindObjectOfType<CardSelection>().selecting)
            Outline(Color.clear);
    }
}