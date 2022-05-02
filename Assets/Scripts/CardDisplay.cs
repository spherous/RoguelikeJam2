using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardDisplay : MonoBehaviour
{
    public TowerCard card;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image artworkImage;
    public TMP_Text costText;
    void Start()
    {
        nameText = transform.GetChild(1).GetComponent<TMP_Text>();
        descriptionText = transform.GetChild(2).GetComponent<TMP_Text>();
        artworkImage = transform.GetChild(3).GetComponent<Image>();
        costText = transform.GetChild(4).GetComponent<TMP_Text>();

        costText.text = card.threadCost.ToString();
        nameText.text = card.name;
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
    
    }

}
