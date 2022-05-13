    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public enum BuildModeState {None = 0, Build = 1, Relocate = 2, PlayOnTower = 3, PlayOnTile = 4};
public static class BuildModeStateExtensions
{
    public static string GetMessage(this BuildModeState state) => state switch{
        BuildModeState.Build => "Click to build a tower",
        BuildModeState.Relocate => "Move a tower",
        BuildModeState.PlayOnTower => "Click on a tower",
        BuildModeState.PlayOnTile => "Click on a tile",
        _ => "",
    };
}

public class BuildMode : MonoBehaviour
{
    public delegate void BuildModeStateChange(BuildModeState state);
    public BuildModeStateChange buildModeStateChange;
    public BuildModeState state {get; private set;} = BuildModeState.None;
    public bool buildModeOn => state != BuildModeState.None;
    public bool canBuild;
    private MouseData mouseData;
    [SerializeField] BuildModeOutline buildModeOutline;
    ProcGen procGen;
    GridGenerator gridGenerator;
    public CardDisplay cardDisplay;
    private GameObject towerToMove;
    public GameObject movingOutlinePrefab;
    private GameObject movingOutline;
    bool overTower = false;
    bool modificationValidOverTile = false;
    private RangeIndicator rangeIndicator;

    void Awake() 
    {
        rangeIndicator = FindObjectOfType<RangeIndicator>();
        mouseData = FindObjectOfType<MouseData>();
        if(mouseData == null)
            Debug.LogError("No MouseData found in scene");
        procGen = FindObjectOfType<ProcGen>();
        gridGenerator = FindObjectOfType<GridGenerator>();
    }
    void Update()
    {
        if(buildModeOn)
        {
            if(mouseData.hoveredTile != null)
            {
                if(mouseData.hoveredTile.tower != null && state == BuildModeState.Build || state == BuildModeState.Relocate || state == BuildModeState.PlayOnTower)
                {
                    buildModeOutline.Move(mouseData.hoveredTile.transform);
                    overTower = true;
                    return;
                }
                else
                {
                    overTower = false;
                    if(state == BuildModeState.PlayOnTile && cardDisplay.card is EnvironmentCard enviornmentCard)
                    {
                        // check if valid add/remove of buildable tile
                        if(enviornmentCard.enviroTypes.Any(type => type == EnvironmentType.CreateBuildableTile) && procGen.ChangeToBuildableIsValid(mouseData.hoveredTile))
                        {
                            // valid
                            modificationValidOverTile = true;
                            buildModeOutline.Move(mouseData.hoveredTile.transform);
                            return;
                        }
                        else if(enviornmentCard.enviroTypes.Any(type => type == EnvironmentType.RemoveBuildableTile) && procGen.ChangeToPathIsValid(mouseData.hoveredTile))
                        {
                            // valid
                            modificationValidOverTile = true;
                            buildModeOutline.Move(mouseData.hoveredTile.transform);
                            return;
                        }

                        // invalid
                        modificationValidOverTile = false;
                        buildModeOutline.Remove();
                        return;
                    }
                    modificationValidOverTile = false;

                    if(!mouseData.hoveredTile.isBuildable && state != BuildModeState.Relocate || mouseData.hoveredTile.type != TileType.Buildable)
                    {
                        buildModeOutline.Remove();
                        canBuild = false;
                        return;
                    }
                    buildModeOutline.Move(mouseData.hoveredTile.transform);
                    canBuild = true;
                }

            }
            else
            {
                buildModeOutline.Remove();
                canBuild = false;
                overTower = false;
                modificationValidOverTile = false;
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
        if(context.started && mouseData.hoveredTile != null)
            GetExecution()?.Invoke();
    }

    private Action GetExecution() => state switch{
        BuildModeState.Build => Build,
        BuildModeState.Relocate => Relocate,
        BuildModeState.PlayOnTower => PlayOnTower,
        BuildModeState.PlayOnTile => PlayOnTile,
        _ => null
    };

    void Build()
    {
        if(canBuild)
        {
            if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
            {
                Complete();
                return;
            }
            Cancelled();
        }
    }

    void PlayOnTile()
    {
        if(modificationValidOverTile)
        {
            if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
            {
                Complete();
                return;
            }
            Cancelled();
        }
    }

    void PlayOnTower()
    {
        if(overTower && mouseData?.hoveredTile?.tower != null)
        {
            if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
            {
                Complete();
                return;
            }
            Cancelled();
        }
    }

    void Relocate()
    {
        if(towerToMove == null && mouseData.hoveredTile.tower != null)
        {
            SelectTower();
            return;
        }
        if(towerToMove != null)
        {
            if(mouseData.hoveredTile.transform.position == towerToMove.transform.position)
            {
                towerToMove = null;
                Destroy(movingOutline);
                return;
            }
            if(mouseData.hoveredTile.tower != null)
            {
                SelectTower();
                return;
            }
        }
        if(mouseData.hoveredTile.tower != null || towerToMove != null && mouseData.hoveredTile.isBuildable)
        {
            if(cardDisplay.card.TryPlay(mouseData.hoveredTile))
            {
                towerToMove.transform.parent = mouseData.hoveredTile.transform;
                towerToMove.transform.localPosition = Vector3.zero;
                towerToMove = null;
                Destroy(movingOutline);
                Complete();
            }
            else
                MoveATowerCancelled();
        }
    }
    public void Complete()
    {
        Close();
        cardDisplay.RemoveFromHand();
    }
    public void Cancelled()
    {
        Close();
        cardDisplay.ReturnCard();
        cardDisplay.transform.localScale = cardDisplay.scale;
    }

    public void Open(CardDisplay cardDisplay, BuildModeState state)
    {
        this.cardDisplay = cardDisplay;
        StateChange(state);
    }
    private void Close()
    {
        canBuild = false;
        overTower = false;
        modificationValidOverTile = false;
        buildModeOutline.Remove();
        StateChange(BuildModeState.None);
    }

    public void MoveATowerCancelled()
    {
        Cancelled();
        towerToMove = null;
        Destroy(movingOutline);
    }
    public void Escape(CallbackContext context)
    {
        if(context.started && state != BuildModeState.None)
            MoveATowerCancelled();
    }
    private void StateChange(BuildModeState state)
    {
        this.state = state;
        buildModeStateChange?.Invoke(state);
    }
}