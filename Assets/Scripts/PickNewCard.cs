using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickNewCard : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private GroupFader fader;
    [SerializeField] private ProgressionText progressionText;
    public List<CardDisplay> cards = new List<CardDisplay>();
    public List<CardChoiceInputHandler> cardChoiceInputHandlers = new List<CardChoiceInputHandler>();
    private ICard[] allCards;

    private void Awake() => allCards = AssetLoader.GetAllCards();

    public void OfferChoice(Action toPerformOnChoice)
    {
        progressionText.ForceShow("Choose a card");

        for(int i = 0; i < cards.Count; i++)
        {
            ICard randomCard = allCards.ChooseRandom();
            cards[i].SetCard(randomCard);
            cardChoiceInputHandlers[i].clickAction = () => {
                deck.AddToDeck(randomCard);
                fader.FadeOut();
                progressionText.Hide();
                toPerformOnChoice?.Invoke();
            };
        }
        
        fader.FadeIn();
    }
}