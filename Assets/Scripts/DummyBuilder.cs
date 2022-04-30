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
            Debug.Log("Clicked");
            Ray ray = Camera.main.ScreenPointToRay (Mouse.current.position.ReadValue());
           
            hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
            
            if (hit.collider == null) 
            {
                selectedTile = null;
            }
            if (hit.collider != null)
            {
                selectedTile = hit.collider.GetComponent<Tile>();
                Debug.Log(selectedTile);
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
    
    
}
