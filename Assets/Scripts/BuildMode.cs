    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class BuildMode : MonoBehaviour
{
    public delegate void BuildModeStateChange(bool enabled);
    public BuildModeStateChange buildModeStateChange;
    public bool buildModeOn;
    public bool canBuild;
    private MouseData mouseData;
    [SerializeField] BuildModeOutline buildModeOutline;
    public CardDisplay cardDisplay;
    void Awake() 
    {
        mouseData = FindObjectOfType<MouseData>();
        if(mouseData == null)
            Debug.LogError("No MouseData found in scene");
    }
    void Update()
    {
        if(buildModeOn)
        {
            if(mouseData.hoveredTile != null)
            {
                if(!mouseData.hoveredTile.isBuildable)
                {
                    buildModeOutline.Remove();
                    canBuild = false;
                    return;
                }
                buildModeOutline.Move(mouseData.hoveredTile.transform);
                canBuild = true;
            }
            else
            {
                buildModeOutline.Remove();
                canBuild = false;
            }
        }
    }
    public void Click(CallbackContext context)
    {
        if(context.started && canBuild && buildModeOn && mouseData.hoveredTile != null)
        {
            if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
            {
                buildModeOn = false;
                buildModeStateChange?.Invoke(buildModeOn);
                canBuild = false;
                buildModeOutline.Remove();
                cardDisplay.RemoveFromHand();
                return;
            }
            Cancelled();

        }
    } 
    public void Cancelled()
    {
        buildModeOutline.Remove();
        buildModeOn = false;
        buildModeStateChange?.Invoke(buildModeOn);
        canBuild = false;
        cardDisplay.ReturnCard();
        cardDisplay.transform.localScale =  cardDisplay.scale;
    }
    public void Escape(CallbackContext context)
    {
        if (context.started && buildModeOn)
            Cancelled();
    }
    public void Open(bool on, CardDisplay cardDisplay)
    {
        buildModeOn = on;
        this.cardDisplay = cardDisplay;
        buildModeStateChange?.Invoke(buildModeOn);
    }
}