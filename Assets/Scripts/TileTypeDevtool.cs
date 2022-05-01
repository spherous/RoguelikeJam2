using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
public class TileTypeDevtool : MonoBehaviour
{
    // Start is called before the first frame update
    public int DevtoolTileType;

    public void PathButton()
    {
        DevtoolTileType = 0;
    }
    public void HomeButton()
    {
        DevtoolTileType = 1;
    }
    public void EnemySpawnButton()
    {
        DevtoolTileType = 2;
    }
    public void BuildableButton()
    {
        DevtoolTileType = 3;
    }
    public void NoneButton(){
        DevtoolTileType = 4;
    }
    private RaycastHit2D hit;

    public void Click(CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Clicked");
            Ray ray = Camera.main.ScreenPointToRay (Mouse.current.position.ReadValue());
           
            hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
                       
            if (hit.collider != null)
            {
                
                if (DevtoolTileType == 4) return;
                hit.collider.GetComponent<Tile>().SetType((TileType)DevtoolTileType);
            }
            if (hit.collider == null)
            {
                
            }
        }
    }
}
