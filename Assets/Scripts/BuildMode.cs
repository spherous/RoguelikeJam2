using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class BuildMode : MonoBehaviour
{
    public ICard card {get; set;}
    public bool buildModeOn;
    public bool canBuild;
    private MouseData mouseData;
    [SerializeField] BuildModeOutline buildModeOutline;
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
            card.TryPlay(mouseData.hoveredTile);
            buildModeOn = false;
            canBuild = false;
            buildModeOutline.Remove();
        }
    } 
    public void Cancelled()
    {
        buildModeOutline.Remove();
        buildModeOn = false;
    }
}