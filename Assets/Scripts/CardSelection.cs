using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardSelection : MonoBehaviour
{
    [SerializeField] Hand hand;
    public bool selecting;
    public List<CardDisplay> selectedToKeep = new List<CardDisplay>();
    public List<CardDisplay> toDiscard = new List<CardDisplay>();
    public delegate void CardSelectionStateChange(bool enabled, int holdCount);
    public CardSelectionStateChange cardSelectionStateChange;
    
    public void SelectCard(CardDisplay cardDisplay)
    {
        if (hand.holdCount > selectedToKeep.Count)
            selectedToKeep.Add(cardDisplay);

        if (hand.holdCount <= selectedToKeep.Count)
        {
            toDiscard = new List<CardDisplay>(hand.cardList);
            foreach (CardDisplay card in selectedToKeep)
                toDiscard.Remove(card);
            FinishSelecting();
        }
        
    }
    public void DeselectCard(CardDisplay cardDisplay) => selectedToKeep.Remove(cardDisplay);
    public void StartSelecting()
    {
        selecting = true;
        cardSelectionStateChange?.Invoke(selecting, hand.holdCount);
    }
    private void FinishSelecting()
    {
        foreach (CardDisplay card in selectedToKeep)
        {
            card.Outline(Color.clear);
        }
        selecting = false;
        cardSelectionStateChange?.Invoke(selecting, hand.holdCount);
        hand.OnWaveStartDiscard(toDiscard);
        selectedToKeep.Clear();
        toDiscard.Clear();
    }

}

