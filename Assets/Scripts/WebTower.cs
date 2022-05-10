using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class WebTower : MonoBehaviour, ITower
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Web webPrefab;
    GridGenerator gridGenerator;
    [field:SerializeField] public float range {get; set;}
    public Index location {get; set;}
    public List<Index> rangeOffsets = new List<Index>();

    public List<Web> connectedWebs = new List<Web>();
    public List<Transform> connectionPoints = new List<Transform>();

    [SerializeField] private SpriteRenderer baseRenderer;
    public Sprite disabledBase;
    public Sprite enabledBase;

    [SerializeField] private SpriteRenderer circleRenderer;
    public Sprite disabledCircle;
    public Sprite enabledCircle;
    
    private void Awake()
    {
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
    }

    private void Start()
    {
        ConnectToNearbyWebTowers();
    }

    private void OnDestroy()
    {
        foreach(Web web in connectedWebs)
        {
            if(web != null)
                Destroy(web.gameObject);
        }
    }

    private void ConnectToNearbyWebTowers()
    {
        List<WebTower> nearbyWebTowers = GetNearbyWebTowers();

        foreach(WebTower webTower in nearbyWebTowers)
            Connect(webTower);
    }

    private void Connect(WebTower to)
    {
        Transform bestSelfPoint = connectionPoints.OrderBy(p => (p.position - to.transform.position).sqrMagnitude).First();
        Transform bestOtherPoint = to.connectionPoints.OrderBy(p => (p.position - transform.position).sqrMagnitude).First();

        Vector3 vec = bestOtherPoint.position - bestSelfPoint.position;
        Vector3 halfWay = vec / 2;

        Web web = Instantiate(webPrefab, transform.position, Quaternion.identity);

        web.Connect(this, to, bestSelfPoint, bestOtherPoint);
        connectedWebs.Add(web);
        to.ReceiveConnection(web);
    }

    public void ReceiveConnection(Web incomingConnection)
    {
        director.Play();
        connectedWebs.Add(incomingConnection);
    }

    private List<WebTower> GetNearbyWebTowers()
    {
        List<WebTower> validNearbyTowers = new List<WebTower>();
        if(gridGenerator.TryGetTile(location, out Tile localTile))
        {
            List<Tile> neighbors = gridGenerator.GetNeighbors(localTile);  
            List<Tile> tilesFromOffset = gridGenerator.TryGetTilesWithOffset(location, rangeOffsets);

            for(int i = 0; i < tilesFromOffset.Count; i++)
            {
                Tile nearbyTile = tilesFromOffset[i];
                if(nearbyTile == null)
                    continue;
                else if(nearbyTile.tower is WebTower webTower)
                {
                    List<int> passThrough = GetPassThroughIndex(i);
                    bool pathFound = false;
                    foreach(int index in passThrough)
                    {
                        if(index < neighbors.Count && neighbors[index].type == TileType.Path)
                        {
                            pathFound = true;
                            break;
                        }
                    }

                    if(!pathFound)
                        continue;
                    
                    validNearbyTowers.Add(webTower);
                }
            }
        }
        return validNearbyTowers;
    } 

    // HAAAAAAAAAAAACK
    // This relies on the order that GridGenerator.GetNeighbors returns as well as the order that rangeOffsets is serialized in. It just -happens- to work
    private List<int> GetPassThroughIndex(int sourceIndex) => sourceIndex switch {
        0 => new List<int> {0, 5},
        1 => new List<int> {0},
        2 => new List<int> {0, 4},
        3 => new List<int> {3, 4},
        4 => new List<int> {3},
        5 => new List<int> {3, 6},
        6 => new List<int> {1, 6},
        7 => new List<int> {1},
        8 => new List<int> {1, 7},
        9 => new List<int> {2, 7},
        10 => new List<int> {2},
        11 => new List<int> {2, 5},
        _ => null
    };

    public void Enable()
    {
        baseRenderer.sprite = enabledBase;
        circleRenderer.sprite = enabledCircle;
    }

    public void Disable()
    {
        baseRenderer.sprite = disabledBase;
        circleRenderer.sprite = disabledCircle;
    }
}