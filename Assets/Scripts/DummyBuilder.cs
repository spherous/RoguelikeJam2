using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
public class DummyBuilder : MonoBehaviour
{
    private RaycastHit2D hit;
    private Tile selectedTile;
    [SerializeField] GameObject dummyTower;
    
    public void Click(CallbackContext context)
    {
        if (context.started)
        {
            Ray ray = Camera.main.ScreenPointToRay (Mouse.current.position.ReadValue());
           
            hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
            
            if (hit.collider != null)
            {
                selectedTile = hit.collider.GetComponent<Tile>();

                if (selectedTile.type != TileType.Buildable) 
                {
                    selectedTile = null;
                }
            }
        }
    }

    public void BuildButton()
    {
        if (selectedTile == null) return;
        Debug.Log("Build");
        Debug.Log(selectedTile.isBuildable);
        if (selectedTile.isBuildable)
        {
            Instantiate(dummyTower, selectedTile.transform.position, Quaternion.identity);
            selectedTile.isBuildable = false;
            selectedTile = null;
        }
    }
    private void Update() {
        if (selectedTile != null)
        {
            Debug.Log(selectedTile);
        }
    }
    public void Why()
    {
        Debug.Log("Why");
    }
    
    
}
