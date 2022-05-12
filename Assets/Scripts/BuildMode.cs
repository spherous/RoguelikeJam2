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
    private GameObject towerToMove;
    public bool movingTower;
    public GameObject movingOutlinePrefab;
    private GameObject movingOutline;
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
                if(!mouseData.hoveredTile.isBuildable && !movingTower || mouseData.hoveredTile.type != TileType.Buildable)
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
    private void SelectTower()
    {
        if(movingOutline != null)
            Destroy(movingOutline);
        towerToMove = ((MonoBehaviour)mouseData.hoveredTile.tower).gameObject;
        movingOutline = Instantiate(movingOutlinePrefab, mouseData.hoveredTile.transform.position, Quaternion.identity);
    }
    public void Click(CallbackContext context)
    {
        if (context.started && buildModeOn && mouseData.hoveredTile != null && movingTower)
        {
            if(towerToMove == null && mouseData.hoveredTile.tower != null)
            {
                SelectTower();
                return;
            }
            if (towerToMove != null)
            {
                if (mouseData.hoveredTile.transform.position == towerToMove.transform.position)
                {
                    towerToMove = null;
                    Destroy(movingOutline);
                    return;
                }
                if (mouseData.hoveredTile.tower != null)
                {
                    SelectTower();
                    return;
                }
            }
            if (mouseData.hoveredTile.tower != null || towerToMove != null && mouseData.hoveredTile.isBuildable)
            {
                if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
                {
                    towerToMove.transform.parent = mouseData.hoveredTile.transform;
                    towerToMove.transform.localPosition = Vector3.zero;
                    towerToMove = null;
                    movingTower = false;
                    buildModeOn = false;
                    buildModeOutline.Remove();
                    buildModeStateChange?.Invoke(buildModeOn);
                    GameObject.Destroy(movingOutline);
                }
            }
            return;
        }
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
    public void MoveATowerCancelled()
    {
        buildModeOutline.Remove();
        buildModeOn = false;
        movingTower = false;
        towerToMove = null;
        buildModeStateChange?.Invoke(buildModeOn);
        canBuild = false;
        cardDisplay.ReturnCard();
        cardDisplay.transform.localScale =  cardDisplay.scale;
        Destroy(movingOutline);
    }
    public void Escape(CallbackContext context)
    {
        if (context.started && buildModeOn)
        {
            Cancelled();
            MoveATowerCancelled();
        }
    }
    public void Open(bool on, CardDisplay cardDisplay)
    {
        buildModeOn = on;
        this.cardDisplay = cardDisplay;
        buildModeStateChange?.Invoke(buildModeOn);
    }
    public void MoveATowerOpen(bool on, CardDisplay cardDisplay)
    {
        this.cardDisplay = cardDisplay;
        buildModeOn = true;
        movingTower = true;
        buildModeStateChange?.Invoke(buildModeOn);
    }

}