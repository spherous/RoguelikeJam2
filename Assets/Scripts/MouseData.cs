using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseData : MonoBehaviour
{
    Mouse mouse => Mouse.current;
    [SerializeField] private Camera cam;
    public LayerMask layerMask;
    public Tile hoveredTile {get; private set;}

    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, layerMask);
        if(hit.collider != null)
        {
            if(hit.collider.TryGetComponent<Tile>(out Tile tile))
            {
                hoveredTile = tile;
            }
        }
        else
        {
            hoveredTile = null;
        }
    }
}