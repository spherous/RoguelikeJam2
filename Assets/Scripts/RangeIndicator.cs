using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private BuildMode buildMode;
    private MouseData mouseData;
    private bool exists = false;
    public float radius;

    private void Awake() {
        buildMode = GameObject.FindObjectOfType<BuildMode>();
        mouseData = GameObject.FindObjectOfType<MouseData>();
        if(buildMode == null|| mouseData == null) 
        {
            Debug.LogError("RangeIndicator: Missing build mode or mouse data");
        }
    }

    private void Update() {
        if(!exists)
            transform.localScale = Vector3.zero;
        if (mouseData.hoveredTile != null && mouseData.hoveredTile.tower != null && !buildMode.buildModeOn)
        {
            exists = true;
            ShowRange(mouseData.hoveredTile.tower);
        }
        else if (mouseData.hoveredTile != null && mouseData.hoveredTile.tower == null || mouseData.hoveredTile == null)
        {
            exists = false;
        }

        Move();
    }
    public void ShowRange(ITower tower)
    {

        radius = tower.range;
        if(buildMode.buildModeOn)
            if(mouseData.hoveredTile.isBuildable)
                exists = true;
            else
                exists = false;

    }
    private void Move()
    {
        if(mouseData.hoveredTile != null && exists)
        {
            transform.position = mouseData.hoveredTile.transform.position;
            transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }
        else
        {
            radius = 0;
            transform.localScale = Vector3.zero;
        }
    }

}
